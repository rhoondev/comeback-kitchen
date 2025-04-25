using System;
using System.Collections.Generic;
using UnityEngine;

// Ordered containers release objects in descending order and restore objects in ascending order
// Objects may be transferred to an ordered container, and these objects are assigned to positions in ascending order
public class OrderedStaticContainer : StaticContainer
{
    private Dictionary<int, ContainerObject> _unreleasedObjects = new Dictionary<int, ContainerObject>();

    protected override bool CanAcceptTransfer(ContainerObject obj, Container sender)
    {
        return _unreleasedObjects.Count < containerDataAsset.objectData.Count; // Accept transfer requests only if the container is not full
    }

    protected override void ReceiveObject(ContainerObject obj)
    {
        base.ReceiveObject(obj);
        _unreleasedObjects.Add(_unreleasedObjects.Count, obj);
    }

    protected override void InitializeObject(ContainerObject obj)
    {
        base.InitializeObject(obj);
        _unreleasedObjects.Add(_unreleasedObjects.Count, obj);
    }

    protected override bool CanRestoreObject(ContainerObject obj)
    {
        return _unreleasedObjects.Count < containerDataAsset.objectData.Count; // Accept restore requests only if the container is not full
    }

    protected override int GetNewObjectIndex(ContainerObject obj)
    {
        return _unreleasedObjects.Count;
    }

    protected override void RestoreObject(ContainerObject obj)
    {
        base.RestoreObject(obj);

        _unreleasedObjects.Add(GetNewObjectIndex(obj), obj);
    }

    public void ReleaseObject()
    {
        if (_unreleasedObjects.Count == 0)
        {
            return; // Container is empty
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