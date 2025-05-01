using System.Collections.Generic;
using UnityEngine;

// Static containers release objects in descending order and restore objects in ascending order
// Static containers are not allowed to have dynamic data, all data must be set in the data asset
public class StaticContainer : Container<StaticObject, StaticContainer>
{
    [SerializeField] protected ContainerDataAsset containerDataAsset;

    private readonly Dictionary<int, StaticObject> _unreleasedObjects = new Dictionary<int, StaticObject>();

    private bool IsEmpty => _unreleasedObjects.Count == 0;
    private bool IsFull => _unreleasedObjects.Count == containerDataAsset.objectData.Count;

    protected override void Awake()
    {
        base.Awake();

        // WARNING: GameObjects which start in the object holder on awake must also exist in the data asset, or issues may occur
        foreach (Transform child in ObjectHolder)
        {
            StaticObject obj = child.GetComponent<StaticObject>();
            Objects.Add(obj);
            _unreleasedObjects.Add(_unreleasedObjects.Count, obj);
        }
    }

    protected override bool CanReceiveTransfer(StaticObject obj)
    {
        // Do not accept transfer request if the container is full because new data cannot be added
        return base.CanReceiveTransfer(obj) && !IsFull;
    }

    protected override void ReceiveObject(StaticObject obj)
    {
        base.ReceiveObject(obj);

        // Force the object to follow the motion of the container
        obj.transform.SetParent(ObjectHolder);

        // Assign the object to the next available position and rotation
        ObjectData objectData = containerDataAsset.objectData[_unreleasedObjects.Count];
        obj.transform.SetLocalPositionAndRotation(objectData.position, objectData.rotation);

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

        // Prevent the object from being transferred or restored again until it is released
        obj.RestoreRequested.Clear();
        obj.TransferRequested.Clear();

        _unreleasedObjects.Add(_unreleasedObjects.Count, obj);

        obj.OnRestore();
    }

    public void ReleaseObject()
    {
        if (IsEmpty)
        {
            return;
        }

        int index = _unreleasedObjects.Count - 1;
        StaticObject obj = _unreleasedObjects[index];

        obj.transform.SetParent(null);

        obj.RestoreRequested.Add(HandleRestoreRequest);
        obj.TransferRequested.Add(HandleTransferRequest);
        _unreleasedObjects.Remove(index);

        obj.OnRelease();
    }
}