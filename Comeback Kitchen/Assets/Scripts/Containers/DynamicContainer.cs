using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

// Dynamic containers support random releasing and restoring of objects, as well as adding and modifying ObjectData at runtime
public class DynamicContainer : Container<DynamicObject, DynamicContainer>
{
    [SerializeField] private Transform restorePoint; // When using automatic release mode, restore objects to this point
    [SerializeField] private XRSocketInteractor socketInteractor; // Socket interactor to use when snapping objects
    [SerializeField] private bool manualReleaseMode; // If true, objects are frozen once settled and must be released manually by calling ReleaseObject()
    [SerializeField] private bool lockInteractionOnReceive; // If true, the object will be locked from being interacted with by the player when received
    [SerializeField] private bool snapToSocket; // If true, the object will snap to the socket interactor when received

    public SmartAction<DynamicObject> OnObjectReReceived = new SmartAction<DynamicObject>(); // Invoked when an object enters the container's trigger collider but already belongs to the container

    // Uses of dynamic containers
    // 1. Zones (automatic release, snap to socket, disables interaction, manually enable interaction, only hold one object at a time--taken care of by the socket interactor)
    // 2. Plates/bowls/mussel strainer (manual release, no interaction)
    // 3. Vegetable strainer (automatic release, disables interaction, manually enable interaction on a particular object)
    // 4. Pan (automatic release, no interaction)

    private readonly Dictionary<DynamicObject, ObjectData> _objectData = new Dictionary<DynamicObject, ObjectData>();
    private bool _isReReceiving = false; // If true, the container is in re-receiving mode and can re-receive objects that are already in the container, triggering OnObjectReReceived

    // Must receive the objects in Start() because we need to wait for each InteractionLocker to be initialized
    private void Start()
    {
        // Add all of the objects in the object holder to the container (ignoring the value of _isReceivingObjects)
        // It is important to go in REVERSE ORDER so that if the objects are unparented (automatic release mode), the index of the next object to be added is not changed
        for (int i = ObjectHolder.childCount - 1; i >= 0; i--)
        {
            OnReceiveObject(ObjectHolder.GetChild(i).GetComponent<DynamicObject>());
        }
    }

    protected override bool CanReceiveObject(DynamicObject obj)
    {
        // Do not accept transfer request if the socket is already occupied
        bool baseCanReceive = base.CanReceiveObject(obj);
        bool emptySocket = !snapToSocket || Objects.Count == 0;
        Debug.Log($"Base can receive: {baseCanReceive}, empty socket: {emptySocket}");
        return baseCanReceive && emptySocket;
    }

    protected override void OnReceiveObject(DynamicObject obj)
    {
        base.OnReceiveObject(obj);

        obj.ReEnteredContainer.Add(OnObjectReEntered);

        if (manualReleaseMode)
        {
            // Force the object to follow the motion of the container
            obj.transform.SetParent(ObjectHolder);

            // Prevent the object from being transferred or restored until it is released
            obj.RestoreRequested.Clear();
            obj.AllowTransfer = false;

            // Once it settles, save the object's position and rotation and freeze it in place
            obj.OnSleep.Add(OnObjectSettled);
        }
        else
        {
            // Let the object move independently of the container
            obj.transform.SetParent(null);

            // Enable restore and transfer requests
            obj.RestoreRequested.Add(OnRestoreRequested);
            obj.AllowTransfer = true;
        }

        if (lockInteractionOnReceive)
        {
            // Debug.Log($"Wants to lock interaction on {obj.gameObject.name}.");

            // If it's an interactable object, prevent the player from interacting with the object 
            if (obj.TryGetComponent<InteractionLocker>(out var interactionLocker))
            {
                // Debug.Log($"Locking interaction on {obj.gameObject.name}.");
                interactionLocker.LockInteraction();
            }
        }

        if (snapToSocket)
        {
            // Snap the object to the socket interactor
            StartCoroutine(SnapToSocketRoutine(obj.GetComponent<IXRSelectInteractable>()));
        }
    }

    protected override void OnRemoveObject(DynamicObject obj)
    {
        base.OnRemoveObject(obj);

        obj.ReEnteredContainer.Clear();
    }

    protected override void RestoreObject(DynamicObject obj)
    {
        if (manualReleaseMode)
        {
            // Force the object to follow the motion of the container
            obj.transform.SetParent(ObjectHolder);

            // Return the object to its original position and rotation
            ObjectData data = _objectData[obj];
            obj.transform.SetLocalPositionAndRotation(data.position, data.rotation);

            // Freeze the object in place
            obj.Rigidbody.linearVelocity = Vector3.zero;
            obj.Rigidbody.angularVelocity = Vector3.zero;
            obj.Rigidbody.useGravity = false;
            obj.Rigidbody.isKinematic = true;
            obj.Rigidbody.interpolation = RigidbodyInterpolation.None; // Very important (otherwise weird jittery behavior occurs)

            // Prevent the object from being transferred or restored again until it is released
            obj.RestoreRequested.Clear();
            obj.AllowTransfer = false;
        }
        else
        {
            // Teleport the object to the restore point
            obj.transform.SetPositionAndRotation(restorePoint.position, restorePoint.rotation);
        }

        obj.OnRestored();
    }

    // POTENTIAL BUG: If an object is released before it has settled, then it may settle outside of the container
    // or it may not settle at all, in which case a NullReferenceException will occur when trying to access the ObjectData dictionary when during restoration
    public void ReleaseObject(DynamicObject obj)
    {
        // Calling ReleaseObject() is only allowed in manual release mode
        if (!manualReleaseMode)
        {
            return;
        }

        // If the object has already been released, it cannot be released again
        if (obj.transform.parent != ObjectHolder)
        {
            return;
        }

        // Let the object move independently of the container
        obj.transform.SetParent(null);

        // Unfreeze the object
        obj.Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        obj.Rigidbody.isKinematic = false;
        obj.Rigidbody.useGravity = true;
        obj.Rigidbody.linearVelocity = Vector3.zero;
        obj.Rigidbody.angularVelocity = Vector3.zero;

        // Enable restore and transfer requests
        obj.RestoreRequested.Add(OnRestoreRequested);
        obj.AllowTransfer = true;

        obj.OnReleased();
    }

    public void EnableReReceivingMode()
    {
        _isReReceiving = true;

        indicatorArrow.SetActive(true);

        if (showTriggerMesh)
        {
            triggerMeshRenderer.enabled = true;
        }
    }

    public void DisableReReceivingMode()
    {
        _isReReceiving = false;

        indicatorArrow.SetActive(false);
        triggerMeshRenderer.enabled = false;
    }

    private void OnObjectReEntered(DynamicObject obj)
    {
        // Debug.Log($"{obj.gameObject.name} re-entered the container trigger.");

        if (_isReReceiving)
        {
            if (obj.TryGetComponent<XRGrabInteractable>(out var interactable))
            {
                EndInteraction(interactable);
            }

            if (lockInteractionOnReceive)
            {
                // If it's an interactable object, prevent the player from interacting with the object 
                if (obj.TryGetComponent<InteractionLocker>(out var interactionLocker))
                {
                    interactionLocker.LockInteraction();
                }
            }

            if (snapToSocket)
            {
                // Snap the object to the socket interactor
                StartCoroutine(SnapToSocketRoutine(obj.GetComponent<IXRSelectInteractable>()));
            }

            OnObjectReReceived.Invoke(obj);

            // Re-receiving mode is only to be enabled temporarily, so disable it after the object is received
            DisableReReceivingMode();
        }
    }

    private void OnObjectSettled(DynamicObject obj)
    {
        // Freeze the object in place
        obj.Rigidbody.linearVelocity = Vector3.zero;
        obj.Rigidbody.angularVelocity = Vector3.zero;
        obj.Rigidbody.useGravity = false;
        obj.Rigidbody.isKinematic = true;
        obj.Rigidbody.interpolation = RigidbodyInterpolation.None; // Very important (otherwise weird jittery behavior occurs)

        // Object is now settled, so it cannot re-settle in a new orientation
        obj.OnSleep.Clear();

        // Save the position and rotation of the object
        ObjectData data = new ObjectData(obj.transform.localPosition, obj.transform.localRotation);

        // Add or update the object data in the dictionary
        if (_objectData.ContainsKey(obj))
        {
            _objectData[obj] = data;
        }
        else
        {
            _objectData.Add(obj, data);
        }
    }

    private IEnumerator SnapToSocketRoutine(IXRSelectInteractable interactable)
    {
        // Activate the socket interactor
        socketInteractor.socketActive = true;

        // Wait for one frame to avoid errors
        yield return null;

        // Snap the object to the socket interactor
        socketInteractor.interactionManager.SelectEnter(socketInteractor, interactable);

        // Wait for a brief period to avoid not snapping the object
        yield return new WaitForSeconds(0.1f);

        // Deactivate the socket interactor
        socketInteractor.socketActive = false;
    }
}