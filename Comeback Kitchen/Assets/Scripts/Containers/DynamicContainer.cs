using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

// Dynamic containers support random releasing and restoring of objects, as well as adding and modifying ObjectData at runtime
public class DynamicContainer : Container<DynamicObject, DynamicContainer>
{
    [SerializeField] private Transform restorePoint; // When using automatic release mode, restore objects to this point
    [SerializeField] private XRSocketInteractor socketInteractor; // Socket interactor to use when snapping objects
    [SerializeField] private bool manualReleaseMode; // If true, objects are released manually by calling ReleaseObject(). If false, objects fall automatically out of the container
    [SerializeField] private bool snapToSocket; // If true, the object will snap to the socket interactor when received

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

    public override void EnableReceivingObjects()
    {
        base.EnableReceivingObjects();

        // Enable the socket interactor if snapToSocket is enabled
        if (snapToSocket)
        {
            socketInteractor.enabled = true;
        }
    }

    public override void DisableReceivingObjects()
    {
        base.DisableReceivingObjects();

        socketInteractor.enabled = false;
    }

    protected override bool CanReceiveObject(DynamicObject obj)
    {
        // Do not accept transfer request if the socket is already occupied
        return base.CanReceiveObject(obj) && !(snapToSocket && socketInteractor.hasSelection);
    }

    protected override void OnReceiveObject(DynamicObject obj)
    {
        base.OnReceiveObject(obj);

        if (manualReleaseMode)
        {
            // Force the object to follow the motion of the container
            obj.transform.SetParent(ObjectHolder);

            // Prevent the object from being transferred until it is released
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

        // If it's an interactable object, prevent the player from interacting with the object 
        if (obj.TryGetComponent<InteractionLocker>(out var interactionLocker))
        {
            interactionLocker.LockInteraction();
        }
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
            obj.Rigidbody.constraints = RigidbodyConstraints.FreezeAll;

            // Prevent the object from being transferred or restored again until it is released
            obj.RestoreRequested.Clear();
            obj.AllowTransfer = false;
        }
        else
        {
            obj.transform.position = restorePoint.position;
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
        obj.Rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        // Object is now settled, so it cannot re-settle in a new orientation
        obj.OnSettled.Clear();

        ObjectData data = new ObjectData(obj.transform.localPosition, obj.transform.localRotation);

        if (_objectData.ContainsKey(obj))
        {
            // Object previously belonged to this container
            _objectData[obj] = data;
        }
        else
        {
            // Object has never belonged to this container
            _objectData.Add(obj, data);
        }
    }
}