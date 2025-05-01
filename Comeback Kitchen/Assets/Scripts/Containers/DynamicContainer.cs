using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Dynamic containers support random releasing and restoring of objects, as well as adding and modifying ObjectData at runtime
public class DynamicContainer : Container<DynamicObject, DynamicContainer>
{
    [SerializeField] private bool freezeObjectsAfterSettling;

    private readonly Dictionary<DynamicObject, int> _objectIndices = new Dictionary<DynamicObject, int>();
    private List<ObjectData> _objectData;

    protected override void Awake()
    {
        // Create a copy of the object data at runtime to avoid permanently modifying the data asset
        // Or if there is no data asset, just create an empty list
        _objectData = containerDataAsset ? containerDataAsset.objectData.Select(obj => obj.Copy()).ToList() : new List<ObjectData>();

        base.Awake();
    }

    protected override void TrackObject(DynamicObject obj)
    {
        base.TrackObject(obj);
        _objectIndices.Add(obj, _objectIndices.Count);
    }

    protected override bool CanAcceptTransfer(DynamicObject obj, Container<DynamicObject, DynamicContainer> sender)
    {
        return true; // Always accept transfer requests
    }

    protected override void ReceiveObject(DynamicObject obj)
    {
        base.ReceiveObject(obj);
        obj.OnSettle.Add(SaveNewObjectData);
    }

    protected override bool CanRestoreObject(DynamicObject obj)
    {
        return true; // Never full, always allow restore
    }

    protected override void RestoreObject(DynamicObject obj)
    {
        obj.transform.SetParent(ObjectHolder);

        ObjectData data = _objectData[_objectIndices[obj]];
        obj.transform.SetLocalPositionAndRotation(data.position, data.rotation);

        obj.RequestRestore.Clear();
        obj.RequestTransfer.Clear();
        obj.OnSettle.Add(SaveNewObjectData);

        obj.OnRestore();
    }

    public void ReleaseObject(DynamicObject obj)
    {
        if (obj.transform.parent != ObjectHolder)
        {
            return; // Object has already been released
        }

        obj.transform.SetParent(null);

        obj.RequestRestore.Add(HandleRestoreRequest);
        obj.RequestTransfer.Add(HandleTransferRequest);
        obj.OnSettle.Clear();

        obj.OnRelease();
    }

    private void SaveNewObjectData(DynamicObject obj)
    {
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

        if (freezeObjectsAfterSettling)
        {
            obj.Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            obj.OnSettle.Clear();
        }
    }
}