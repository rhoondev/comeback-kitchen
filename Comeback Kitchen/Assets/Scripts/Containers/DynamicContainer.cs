using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

// Dynamic containers support random releasing and restoring of objects, as well as adding and modifying ObjectData at runtime
public class DynamicContainer : Container<DynamicObject, DynamicContainer>
{
    [SerializeField] private Transform restorePoint; // When using automatic release mode, restore objects to this point
    [SerializeField] private XRSocketInteractor socketInteractor; // Socket interactor to use when snapping objects
    [SerializeField] private bool manualReleaseMode; // If true, objects are frozen once settled and must be released manually by calling ReleaseObject()
    [SerializeField] private bool lockObjectInteractionOnReception; // If true, the object will be locked from being interacted with by the player when received
    [SerializeField] private bool snapToSocket; // If true, the object will snap to the socket interactor when received

    public SmartAction<DynamicObject> OnObjectReReceived = new SmartAction<DynamicObject>(); // Invoked when an object enters the container's trigger collider but already belongs to the container

    // Uses of dynamic containers
    // 1. Zones (automatic release, snap to socket, disables interaction, manually enable interaction, only hold one object at a time--taken care of by the socket interactor)
    // 2. Plates/bowls/mussel strainer (manual release, no interaction)
    // 3. Vegetable strainer (automatic release, disables interaction, manually enable interaction on a particular object)
    // 4. Pan (automatic release, no interaction)

    private readonly Dictionary<DynamicObject, ObjectData> _objectData = new Dictionary<DynamicObject, ObjectData>();

    protected override void Awake()
    {
        base.Awake();

        // Add all of the objects in the object holder to the container (ignoring the value of _isReceivingObjects)
        // It is important to go in REVERSE ORDER so that if the objects are unparented (automatic release mode), the index of the next object to be added is not changed
        for (int i = ObjectHolder.childCount - 1; i >= 0; i--)
        {
            OnReceiveObject(ObjectHolder.GetChild(i).GetComponent<DynamicObject>());
        }
    }

    public void EnableReReceivingMode()
    {
        // Enable the indicator arrow and the trigger mesh renderer (if applicable)
        EnableReceivingObjects();

        // Prevent other objects besides the current objects from being received
        SetTargetObjects(new HashSet<DynamicObject>());
    }

    public override void EnableReceivingObjects()
    {
        base.EnableReceivingObjects();

        // Enable the socket interactor if snapToSocket is enabled
        if (snapToSocket)
        {
            socketInteractor.socketActive = true;
        }
    }

    public override void DisableReceivingObjects()
    {
        base.DisableReceivingObjects();

        socketInteractor.socketActive = false;
    }

    private void OnObjectReEntered(DynamicObject obj)
    {
        Debug.Log($"{obj.gameObject.name} re-entered the container trigger.");

        if (_isReceivingObjects)
        {
            OnObjectReReceived.Invoke(obj);
        }
    }

    protected override bool CanReceiveObject(DynamicObject obj)
    {
        // Do not accept transfer request if the socket is already occupied
        return base.CanReceiveObject(obj) && !(snapToSocket && socketInteractor.hasSelection);
    }

    protected override void OnReceiveObject(DynamicObject obj)
    {
        base.OnReceiveObject(obj);

        obj.ReEntered.Add(OnObjectReEntered);

        if (manualReleaseMode)
        {
            // Force the object to follow the motion of the container
            obj.transform.SetParent(ObjectHolder);

            // Prevent the object from being transferred or restored until it is released
            obj.RestoreRequested.Clear();
            obj.AllowTransfer = false;

            // Once it settles, save the object's position and rotation and freeze it in place
            obj.OnSettled.Add(OnObjectSettled);
        }
        else
        {
            // Let the object move independently of the container
            obj.transform.SetParent(null);

            // Enable restore and transfer requests
            obj.RestoreRequested.Add(OnRestoreRequested);
            obj.AllowTransfer = true;
        }

        if (lockObjectInteractionOnReception)
        {
            // If it's an interactable object, prevent the player from interacting with the object 
            if (obj.TryGetComponent<InteractionLocker>(out var interactionLocker))
            {
                interactionLocker.LockInteraction();
            }
        }
    }

    protected override void OnRemoveObject(DynamicObject obj)
    {
        base.OnRemoveObject(obj);

        obj.ReEntered.Clear();
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
        if (!manualReleaseMode)
        {
            return; // Manual release is not enabled
        }

        if (obj.transform.parent != ObjectHolder)
        {
            return; // Object has already been released
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

    private void OnObjectSettled(DynamicObject obj)
    {
        // Freeze the object in place
        obj.Rigidbody.linearVelocity = Vector3.zero;
        obj.Rigidbody.angularVelocity = Vector3.zero;
        obj.Rigidbody.useGravity = false;
        obj.Rigidbody.isKinematic = true;
        obj.Rigidbody.interpolation = RigidbodyInterpolation.None; // Very important (otherwise weird jittery behavior occurs)

        // Object is now settled, so it cannot re-settle in a new orientation
        obj.OnSettled.Clear();

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
}