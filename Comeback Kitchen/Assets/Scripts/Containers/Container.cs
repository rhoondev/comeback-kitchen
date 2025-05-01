using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// A class for tracking ownership of objects such as grains of rice or pieces of vegetables
public abstract class Container<TObject, TContainer> : MonoBehaviour
    where TObject : ContainerObject<TObject, TContainer>
    where TContainer : Container<TObject, TContainer>
{
    [field: SerializeField] public Transform ObjectHolder { get; private set; } // The transform that holds the container objects
    [SerializeField] private Collider triggerCollider; // Optional trigger used to detect when objects enter the container (don't call EnableTrigger() or DisableTrigger() if you don't want to use this)
    [SerializeField] private GameObject visualPlacementIndicator; // Optional visual indicator to show where objects can be placed
    [SerializeField] private bool useVisualPlacementIndicator;

    public HashSet<TObject> Objects { get; set; } = new HashSet<TObject>(); // All objects which are owned by this container
    public SmartAction<TObject> OnReceiveObject = new SmartAction<TObject>();

    public void EnableTrigger()
    {
        triggerCollider.enabled = true;

        if (useVisualPlacementIndicator)
        {
            visualPlacementIndicator.SetActive(true);
        }
    }

    public void DisableTrigger()
    {
        triggerCollider.enabled = false;
        visualPlacementIndicator.SetActive(false);
    }

    protected void HandleTransferRequest(TObject obj, TContainer receiver)
    {
        if (receiver.CanReceiveTransfer(obj))
        {
            SendObject(obj);
            receiver.ReceiveObject(obj);
        }
        else
        {
            obj.OnTransferDenied();
        }
    }

    protected virtual bool CanReceiveTransfer(TObject obj)
    {
        // Check if the object is already in the container
        return !Objects.Contains(obj);
    }

    protected virtual void SendObject(TObject obj)
    {
        Objects.Remove(obj);

        // De-couple the object's events from the container
        obj.RestoreRequested.Clear();
        obj.TransferRequested.Clear();
    }

    protected virtual void ReceiveObject(TObject obj)
    {
        Objects.Add(obj);

        if (obj.TryGetComponent<XRGrabInteractable>(out var interactable))
        {
            DropInteractable(interactable);
        }

        obj.OnTransfer();

        OnReceiveObject.Invoke(obj);
    }

    protected void HandleRestoreRequest(TObject obj)
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

    protected virtual bool CanRestoreObject(TObject obj)
    {
        // Restore only if the object belongs the container
        return Objects.Contains(obj);
    }

    protected abstract void RestoreObject(TObject obj);

    private void DropInteractable(XRBaseInteractable interactable)
    {
        var interactor = interactable.firstInteractorSelecting;

        if (interactor != null)
        {
            // Drop the object
            interactable.interactionManager.SelectExit(interactor, interactable);
        }
    }
}
