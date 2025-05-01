using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

// A class for tracking ownership of objects such as grains of rice or pieces of vegetables
public abstract class Container<TObject, TContainer> : MonoBehaviour
    where TObject : ContainerObject<TObject, TContainer>
    where TContainer : Container<TObject, TContainer>
{
    [field: SerializeField] public Transform ObjectHolder { get; private set; } // The transform that holds the container objects
    [SerializeField] private GameObject indicatorArrow; // Visual indicator that draws the user's attention to the container when it is receiving objects
    [SerializeField] private bool showTriggerMesh; // If true, the indicator zone is used to show the area where objects can be placed

    public HashSet<TObject> Objects { get; set; } = new HashSet<TObject>(); // All objects which are owned by this container
    public SmartAction<TObject> OnObjectAdded = new SmartAction<TObject>(); // Invoked when an object is added to the container
    // public SmartAction<TObject> OnObjectRemoved = new SmartAction<TObject>(); // Invoked when an object is removed from the container

    private Collider _triggerCollider; // The collider that triggers the transfer request
    private MeshRenderer _triggerMeshRenderer; // The mesh renderer that shows where the trigger collider

    protected virtual void Awake()
    {
        // Get the trigger collider and mesh renderer components
        _triggerCollider = GetComponent<Collider>();
        _triggerMeshRenderer = GetComponent<MeshRenderer>();

        // Disable the trigger collider and mesh renderer by default
        _triggerCollider.enabled = false;
        _triggerMeshRenderer.enabled = false;

        // Set the indicator arrow to be inactive by default
        indicatorArrow.SetActive(false);
    }

    public void SetTargetObject(GameObject obj)
    {
        // TODO: Remove functionality to set target object. This is not needed anymore.
    }

    public void EnableReceivingObjects()
    {
        _triggerCollider.enabled = true;
        indicatorArrow.SetActive(true);

        if (showTriggerMesh)
        {
            _triggerMeshRenderer.enabled = true;
        }
    }

    public void DisableReceivingObjects()
    {
        _triggerCollider.enabled = false;
        indicatorArrow.SetActive(false);
        _triggerMeshRenderer.enabled = false;
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

        OnObjectAdded.Invoke(obj);
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
