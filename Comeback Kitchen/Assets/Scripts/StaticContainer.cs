using UnityEngine;

// A static container has a list of predefined positions and rotations
// which may be occupied by the objects themselves
public abstract class StaticContainer : Container
{
    [SerializeField] protected StaticContainerDataAsset containerDataAsset;

    private void Awake()
    {
        // Add all objects in the hierarchy
        foreach (Transform child in ObjectHolder)
        {
            InitializeObject(child.GetComponent<ContainerObject>());
        }
    }

    protected override void ReceiveObject(ContainerObject obj)
    {
        base.ReceiveObject(obj);

        // Objects in static containers cannot move
        obj.Rigidbody.linearVelocity = Vector3.zero;
        obj.Rigidbody.angularVelocity = Vector3.zero;
        obj.Rigidbody.isKinematic = true;
    }

    protected virtual void InitializeObject(ContainerObject obj)
    {
        Objects.Add(obj);
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
    protected abstract int GetNewObjectIndex(ContainerObject obj);

    protected virtual void RestoreObject(ContainerObject obj)
    {
        // Objects in static containers cannot move
        obj.Rigidbody.linearVelocity = Vector3.zero;
        obj.Rigidbody.angularVelocity = Vector3.zero;
        obj.Rigidbody.isKinematic = true;

        obj.transform.SetParent(ObjectHolder);

        ObjectData objectData = containerDataAsset.objectData[GetNewObjectIndex(obj)];
        obj.transform.SetLocalPositionAndRotation(objectData.position, objectData.rotation);

        obj.RequestRestore.Clear();
        obj.RequestTransfer.Clear();

        obj.OnRestore();
    }
}
