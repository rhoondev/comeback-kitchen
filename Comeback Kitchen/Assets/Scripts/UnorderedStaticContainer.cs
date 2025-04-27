using System.Collections.Generic;
using UnityEngine;

// Unordered containers support random releasing and restoring of objects
// However, they cannot receive transferred objects
public class UnorderedStaticContainer : StaticContainer
{
    private readonly Dictionary<ContainerObject, int> _objectIndices = new Dictionary<ContainerObject, int>();

    protected override bool CanAcceptTransfer(ContainerObject obj, Container sender)
    {
        return allowDynamicData;
    }

    protected override void ReceiveObject(ContainerObject obj)
    {
        base.ReceiveObject(obj);

        ObjectData data = new ObjectData(obj.transform.localPosition, obj.transform.localRotation);

        if (_objectIndices.ContainsKey(obj))
        {
            // Object previously belonged to this container
            _objectData[_objectIndices[obj]] = data;
        }
        else
        {
            // Object has never belonged to this container
            _objectData.Add(data);
            _objectIndices.Add(obj, _objectIndices.Count);
        }
    }

    protected override void TrackObject(ContainerObject obj)
    {
        base.TrackObject(obj);
        _objectIndices.Add(obj, _objectIndices.Count);
    }

    protected override bool CanRestoreObject(ContainerObject obj)
    {
        return true;
    }

    protected override int GetRestoreIndex(ContainerObject obj)
    {
        return _objectIndices[obj];
    }

    public void ReleaseObject(ContainerObject obj)
    {
        if (obj.transform.parent != ObjectHolder)
        {
            return; // Object has already been released
        }

        obj.transform.SetParent(null);

        obj.RequestRestore.Add(HandleRestoreRequest);
        obj.RequestTransfer.Add(HandleTransferRequest);

        obj.OnRelease();
    }
}