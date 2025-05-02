using System.Collections.Generic;
using UnityEngine;

// Static containers release objects in descending order and restore objects in ascending order
// Static containers are not allowed to have dynamic data, all data must be set in the data asset
public class StaticContainer : Container<StaticObject, StaticContainer>
{
    [SerializeField] private ContainerDataAsset containerDataAsset; // The asset that contains the positions and rotations of the objects in the container

    private readonly Dictionary<int, StaticObject> _unreleasedObjects = new Dictionary<int, StaticObject>();

    private bool IsEmpty => _unreleasedObjects.Count == 0;
    private bool IsFull => _unreleasedObjects.Count == containerDataAsset.objectData.Count;

    protected override void Awake()
    {
        base.Awake();

        // Add all of the objects in the object holder to the container (ignoring the value of _isReceivingObjects)
        // It is important to go IN ORDER so that the align with the data asset
        // WARNING: If objects in the object holder of a StaticContainer do not match up with the data asset, the container will not work as expected
        foreach (Transform child in ObjectHolder)
        {
            OnReceiveObject(child.GetComponent<StaticObject>());
        }
    }

    protected override bool CanReceiveObject(StaticObject obj)
    {
        // Do not accept transfer request if the container is full because new data cannot be added
        return base.CanReceiveObject(obj) && !IsFull;
    }

    protected override void OnReceiveObject(StaticObject obj)
    {
        base.OnReceiveObject(obj);

        // Force the object to follow the motion of the container
        obj.transform.SetParent(ObjectHolder);

        // Assign the object to the next available position and rotation
        ObjectData objectData = containerDataAsset.objectData[_unreleasedObjects.Count];
        obj.transform.SetLocalPositionAndRotation(objectData.position, objectData.rotation);

        // Freeze the object in place
        obj.Rigidbody.isKinematic = false;
        obj.Rigidbody.useGravity = true;
        obj.Rigidbody.linearVelocity = Vector3.zero;
        obj.Rigidbody.angularVelocity = Vector3.zero;

        // Prevent the object from being transferred or restored until it is released
        obj.RestoreRequested.Clear();
        obj.AllowTransfer = false;

        _unreleasedObjects.Add(_unreleasedObjects.Count, obj);
    }

    protected override bool CanRestoreObject(StaticObject obj)
    {
        // Do not accept restore request if the container is full because new data cannot be added
        return base.CanRestoreObject(obj) && !IsFull;
    }

    protected override void RestoreObject(StaticObject obj)
    {
        // Force the object to follow the motion of the container
        obj.transform.SetParent(ObjectHolder);

        // Assign the object to the next available position and rotation
        ObjectData data = containerDataAsset.objectData[_unreleasedObjects.Count];
        obj.transform.SetLocalPositionAndRotation(data.position, data.rotation);

        // Freeze the object in place
        obj.Rigidbody.isKinematic = false;
        obj.Rigidbody.useGravity = true;
        obj.Rigidbody.linearVelocity = Vector3.zero;
        obj.Rigidbody.angularVelocity = Vector3.zero;

        // Prevent the object from being transferred or restored again until it is released
        obj.RestoreRequested.Clear();
        obj.AllowTransfer = false;

        _unreleasedObjects.Add(_unreleasedObjects.Count, obj);

        obj.OnRestored();
    }

    public void ReleaseObject()
    {
        if (IsEmpty)
        {
            return;
        }

        int index = _unreleasedObjects.Count - 1;
        StaticObject obj = _unreleasedObjects[index];

        // Let the object move independently of the container
        obj.transform.SetParent(null);

        // Enable motion of the object
        obj.Rigidbody.isKinematic = false;
        obj.Rigidbody.useGravity = true;
        obj.Rigidbody.linearVelocity = Vector3.zero;
        obj.Rigidbody.angularVelocity = Vector3.zero;

        // Allow the object to be transferred or restored
        obj.RestoreRequested.Add(OnRestoreRequested);
        obj.AllowTransfer = true;

        _unreleasedObjects.Remove(index);

        obj.OnReleased();
    }
}