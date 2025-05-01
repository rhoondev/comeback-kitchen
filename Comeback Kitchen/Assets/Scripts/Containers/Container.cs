using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// A class for tracking ownership of objects such as grains of rice or pieces of vegetables
public abstract class Container<TObject, TContainer> : MonoBehaviour
    where TObject : ContainerObject<TObject, TContainer>
    where TContainer : Container<TObject, TContainer>
{
    [field: SerializeField] public Transform ObjectHolder { get; private set; } // The transform that holds the objects
    [SerializeField] private Collider triggerCollider;
    [SerializeField] private GameObject visualPlacementIndicator;
    [SerializeField] private bool useVisualPlacementIndicator;
    [SerializeField] protected ContainerDataAsset containerDataAsset;

    public List<TObject> Objects { get; set; } = new List<TObject>(); // All objects which are owned by this container
    public SmartAction<TObject> OnReceiveObject = new SmartAction<TObject>();

    protected virtual void Awake()
    {
        // WARNING: GameObjects which start in the object holder on awake must also exist in the data asset, or issues may occur
        foreach (Transform child in ObjectHolder)
        {
            TrackObject(child.GetComponent<TObject>());
        }
    }

    protected virtual void TrackObject(TObject obj)
    {
        Objects.Add(obj);
        obj.Container = this;
    }

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

    protected abstract bool CanAcceptTransfer(TObject obj, Container<TObject, TContainer> sender);

    protected virtual void SendObject(TObject obj)
    {
        Objects.Remove(obj);
        obj.Container = null;

        // De-couple the object's events from the container
        obj.RequestRestore.Clear();
        obj.RequestTransfer.Clear();
    }

    protected virtual void ReceiveObject(TObject obj)
    {
        obj.transform.SetParent(ObjectHolder);

        Objects.Add(obj);
        obj.Container = this;

        obj.OnTransfer();

        if (obj.TryGetComponent<XRGrabInteractable>(out var interactable))
        {
            ReleaseInteractable(interactable);
        }

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

    protected abstract bool CanRestoreObject(TObject obj);
    protected abstract void RestoreObject(TObject obj);

    private void ReleaseInteractable(XRBaseInteractable interactable)
    {
        var interactor = interactable.firstInteractorSelecting;

        if (interactor != null)
        {
            // Drop the object
            interactable.interactionManager.SelectExit(interactor, interactable);
        }
    }
}
