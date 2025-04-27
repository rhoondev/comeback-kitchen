using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// A static container has a list of predefined positions and rotations
// which may be occupied by the objects themselves
public abstract class StaticContainer : Container
{
    [SerializeField] private StaticContainerDataAsset containerDataAsset; // Do not use or modify directly
    [SerializeField] protected bool allowDynamicData;

    protected List<ObjectData> _objectData; // Use and modify this instead

    private void Awake()
    {
        // Create a copy of the object data at runtime if necessary, to avoid permanently modifying the data
        _objectData = allowDynamicData ? containerDataAsset.objectData.Select(obj => obj.Copy()).ToList() : containerDataAsset.objectData;

        // Objects which start in the object holder must also exist in the data asset, or issues may occur
        foreach (Transform child in ObjectHolder)
        {
            TrackObject(child.GetComponent<ContainerObject>());
        }
    }

    protected override void ReceiveObject(ContainerObject obj)
    {
        // Freeze object upon reception
        obj.Rigidbody.linearVelocity = Vector3.zero;
        obj.Rigidbody.angularVelocity = Vector3.zero;
        obj.Rigidbody.isKinematic = true;

        base.ReceiveObject(obj);
    }

    protected virtual void TrackObject(ContainerObject obj)
    {
        Objects.Add(obj);
        obj.Container = this;
    }

    protected void HandleRestoreRequest(ContainerObject obj)
    {
        if (CanRestoreObject(obj))
        {
            RestoreObject(obj);
        }
        else
        {
            obj.OnRestoreDenied();
        }
    }

    protected abstract bool CanRestoreObject(ContainerObject obj);
    protected abstract int GetRestoreIndex(ContainerObject obj);

    protected virtual void RestoreObject(ContainerObject obj)
    {
        // Objects should already be frozen
        // It either collided with the environment or was denied transfer to another container
        // In either case, the object should freeze itself

        obj.transform.SetParent(ObjectHolder);

        ObjectData data = _objectData[GetRestoreIndex(obj)];
        obj.transform.SetLocalPositionAndRotation(data.position, data.rotation);

        obj.RequestRestore.Clear();
        obj.RequestTransfer.Clear();

        obj.OnRestore();
    }
}
