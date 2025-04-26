using UnityEngine;

// A static container has a list of predefined positions and rotations
// which may be occupied by the objects themselves
public abstract class StaticContainer : Container
{
    [SerializeField] protected StaticContainerDataAsset containerDataAsset;
    [SerializeField] protected bool allowDynamicData;

    private void Awake()
    {
        // Objects which start in the object holder must also exist in the data asset, or issues may occur
        foreach (Transform child in ObjectHolder)
        {
            TrackObject(child.GetComponent<ContainerObject>());
        }
    }

    protected override void ReceiveObject(ContainerObject obj)
    {
        // Objects in static containers cannot move
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
        // Objects in static containers cannot move
        obj.Rigidbody.linearVelocity = Vector3.zero;
        obj.Rigidbody.angularVelocity = Vector3.zero;
        obj.Rigidbody.isKinematic = true;

        obj.transform.SetParent(ObjectHolder);

        ObjectData objectData = containerDataAsset.objectData[GetRestoreIndex(obj)];
        obj.transform.SetLocalPositionAndRotation(objectData.position, objectData.rotation);

        obj.RequestRestore.Clear();
        obj.RequestTransfer.Clear();

        obj.OnRestore();
    }
}
