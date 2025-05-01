using System.Collections.Generic;

// Static containers release objects in descending order and restore objects in ascending order
// Static containers are not allowed to have dynamic data, all data must be set in the data asset
public class StaticContainer : Container<StaticObject, StaticContainer>
{
    private readonly Dictionary<int, StaticObject> _unreleasedObjects = new Dictionary<int, StaticObject>();

    private bool IsEmpty => _unreleasedObjects.Count == 0;
    private bool IsFull => _unreleasedObjects.Count == containerDataAsset.objectData.Count;

    protected override void TrackObject(StaticObject obj)
    {
        base.TrackObject(obj);
        _unreleasedObjects.Add(_unreleasedObjects.Count, obj);
    }

    protected override bool CanAcceptTransfer(StaticObject obj, Container<StaticObject, StaticContainer> sender)
    {
        return !IsFull; // Only accept transfer requests if the container is not full, as new data cannot be added
    }

    protected override void ReceiveObject(StaticObject obj)
    {
        base.ReceiveObject(obj);

        // Assign the object to the next available data slot
        ObjectData objectData = containerDataAsset.objectData[_unreleasedObjects.Count];
        obj.transform.SetLocalPositionAndRotation(objectData.position, objectData.rotation);

        _unreleasedObjects.Add(_unreleasedObjects.Count, obj);
    }

    protected override bool CanRestoreObject(StaticObject obj)
    {
        return !IsFull;
    }

    protected override void RestoreObject(StaticObject obj)
    {
        obj.transform.SetParent(ObjectHolder);

        ObjectData data = containerDataAsset.objectData[_unreleasedObjects.Count];
        obj.transform.SetLocalPositionAndRotation(data.position, data.rotation);

        obj.RequestRestore.Clear();
        obj.RequestTransfer.Clear();

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

        obj.RequestRestore.Add(HandleRestoreRequest);
        obj.RequestTransfer.Add(HandleTransferRequest);
        _unreleasedObjects.Remove(index);

        obj.OnRelease();
    }
}