using System.Collections.Generic;

// Unordered containers support random releasing and restoring of objects
// However, they cannot receive transferred objects
public class UnorderedStaticContainer : StaticContainer
{
    private Dictionary<ContainerObject, int> objectIndices = new Dictionary<ContainerObject, int>();

    protected override bool CanAcceptTransfer(ContainerObject obj, Container sender)
    {
        return false; // An unordered container has no way to accept new objects
    }

    protected override bool CanRestoreObject(ContainerObject obj)
    {
        return true;
    }

    protected override int GetNewObjectIndex(ContainerObject obj)
    {
        return objectIndices[obj];
    }

    public void ReleaseObject(ContainerObject obj)
    {
        if (obj.transform.parent != ObjectHolder)
        {
            return; // Object has already been released
        }

        obj.transform.SetParent(null);
        obj.Rigidbody.isKinematic = false;

        obj.RequestRestore.Add(HandleRestoreRequest);
        obj.RequestTransfer.Add(HandleTransferRequest);

        obj.OnRelease();
    }
}