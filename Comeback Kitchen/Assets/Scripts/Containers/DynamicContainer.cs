using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

// Dynamic containers support random releasing and restoring of objects, as well as adding and modifying ObjectData at runtime
public class DynamicContainer : Container<DynamicObject, DynamicContainer>
{
    [SerializeField] private bool manualReleaseMode; // If true, objects are released manually by calling ReleaseObject(). If false, objects fall automatically out of the container
    [SerializeField] private Transform restorePoint; // When using automatic release mode, restore objects to this point
    [SerializeField] private XRSocketInteractor socketInteractor; // Optional socket interactor to use when receiving objects

    private readonly Dictionary<DynamicObject, ObjectData> _objectData = new Dictionary<DynamicObject, ObjectData>();

    protected override void ReceiveObject(DynamicObject obj)
    {
        base.ReceiveObject(obj);

        if (manualReleaseMode)
        {
            // Force the object to follow the motion of the container
            obj.transform.SetParent(ObjectHolder);

            // Once it settles, save the object's position and rotation and freeze it in place
            obj.OnSettled.Add(OnObjectSettled);
        }

        if (socketInteractor != null)
        {
            // Attach the object to the socket interactor
            socketInteractor.interactionManager.SelectEnter((IXRSelectInteractor)socketInteractor, (IXRSelectInteractable)obj.GetComponent<XRBaseInteractable>());
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
            obj.TransferRequested.Clear();
        }
        else
        {
            obj.transform.position = restorePoint.position;
        }

        obj.OnRestore();
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
        obj.RestoreRequested.Add(HandleRestoreRequest);
        obj.TransferRequested.Add(HandleTransferRequest);

        obj.OnRelease();
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