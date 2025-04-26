using System.Collections.Generic;
using UnityEngine;

// A class for tracking ownership of individual objects such as grains of rice or pieces of vegetables
public class Container : MonoBehaviour
{
    [field: SerializeField] public Transform ObjectHolder { get; private set; } // The transform that holds the objects

    public List<ContainerObject> Objects { get; set; } = new List<ContainerObject>(); // All objects which are owned by this container
    public SmartAction<ContainerObject> OnReceiveObject = new SmartAction<ContainerObject>();

    protected void HandleTransferRequest(ContainerObject obj, Container receiver)
    {
        if (receiver.CanAcceptTransfer(obj, this))
        {
            SendObject(obj);
            receiver.ReceiveObject(obj);
        }
        else
        {
            obj.OnTransferDenied();
        }
    }

    protected virtual bool CanAcceptTransfer(ContainerObject obj, Container sender)
    {
        return true; // A simple container has no restrictions and will always accept transfer requests
    }

    protected virtual void SendObject(ContainerObject obj)
    {
        Objects.Remove(obj);

        // De-couple the object's events from the container
        obj.RequestRestore.Clear();
        obj.RequestTransfer.Clear();
    }

    protected virtual void ReceiveObject(ContainerObject obj)
    {
        Objects.Add(obj);
        obj.transform.SetParent(ObjectHolder);

        obj.OnTransfer();

        OnReceiveObject.Invoke(obj);
    }
}
