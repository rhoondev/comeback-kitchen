using System.Collections.Generic;

// Ordered containers release objects in descending order and restore objects in ascending order
// Objects may be transferred to an ordered container, and these objects are assigned to positions in ascending order
public class OrderedStaticContainer : StaticContainer
{
    private readonly Dictionary<int, ContainerObject> _unreleasedObjects = new Dictionary<int, ContainerObject>();

    private bool IsEmpty => _unreleasedObjects.Count == 0;
    private bool IsFull => _unreleasedObjects.Count == containerDataAsset.objectData.Count;

    protected override bool CanAcceptTransfer(ContainerObject obj, Container sender)
    {
        return !IsFull || allowDynamicData;
    }

    protected override void ReceiveObject(ContainerObject obj)
    {
        base.ReceiveObject(obj);

        if (IsFull)
        {
            // Add new data dynamically if the container is full
            containerDataAsset.objectData.Add(new ObjectData(obj.transform.localPosition, obj.transform.localRotation));
        }
        else
        {
            // Assign the object to the next available data if the container is not full
            ObjectData objectData = containerDataAsset.objectData[GetRestoreIndex(obj)];
            obj.transform.SetLocalPositionAndRotation(objectData.position, objectData.rotation);
        }

        _unreleasedObjects.Add(_unreleasedObjects.Count, obj);
    }

    protected override void TrackObject(ContainerObject obj)
    {
        base.TrackObject(obj);
        _unreleasedObjects.Add(_unreleasedObjects.Count, obj);
    }

    protected override bool CanRestoreObject(ContainerObject obj)
    {
        return !IsFull;
    }

    protected override int GetRestoreIndex(ContainerObject obj)
    {
        return _unreleasedObjects.Count;
    }

    protected override void RestoreObject(ContainerObject obj)
    {
        base.RestoreObject(obj);

        _unreleasedObjects.Add(GetRestoreIndex(obj), obj);
    }

    public void ReleaseObject()
    {
        if (IsEmpty)
        {
            return;
        }

        int index = _unreleasedObjects.Count - 1;
        ContainerObject obj = _unreleasedObjects[index];

        obj.transform.SetParent(null);
        obj.Rigidbody.isKinematic = false;

        obj.RequestRestore.Add(HandleRestoreRequest);
        obj.RequestTransfer.Add(HandleTransferRequest);
        _unreleasedObjects.Remove(index);

        obj.OnRelease();
    }
}