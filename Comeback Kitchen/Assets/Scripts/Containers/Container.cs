using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// A class for tracking ownership of objects such as grains of rice or pieces of vegetables
public abstract class Container<TObject, TContainer> : MonoBehaviour
    where TObject : ContainerObject<TObject, TContainer>
    where TContainer : Container<TObject, TContainer>
{
    [field: SerializeField] public Transform ObjectHolder { get; private set; } // The transform that holds the objects in the container
    [SerializeField] protected MeshRenderer triggerMeshRenderer; // The mesh renderer that shows where the trigger collider is
    [SerializeField] protected GameObject indicatorArrow; // Visual indicator that draws the user's attention to the container when it is receiving objects
    [SerializeField] protected bool showTriggerMesh; // If true, the indicator zone is used to show the area where objects can be placed

    public HashSet<TObject> Objects { get; set; } = new HashSet<TObject>(); // All objects which are owned by this container
    public SmartAction<TObject> OnObjectReceived = new SmartAction<TObject>(); // Invoked when an object is added to the container
    // public SmartAction<TObject> OnObjectRemoved = new SmartAction<TObject>(); // Invoked when an object is removed from the container

    private HashSet<TObject> _targetObjects = new HashSet<TObject>(); // If not null, this is the only object that can be received by the container. If null, any object can be received.

    // Sets the target object that the container can receive to the given object
    public void SetTarget(TObject obj)
    {
        SetTargets(new HashSet<TObject> { obj });
    }

    // Sets the target objects that the container can receive to a shallow copy of the given set
    public void SetTargets(HashSet<TObject> objects)
    {
        _targetObjects = new HashSet<TObject>(objects);

        indicatorArrow.SetActive(true);

        if (showTriggerMesh)
        {
            triggerMeshRenderer.enabled = true;
        }
    }

    // Clears the target objects that the container can receive (will not accept any objects)
    public void ClearTargets()
    {
        _targetObjects.Clear();

        indicatorArrow.SetActive(false);
        triggerMeshRenderer.enabled = false;
    }

    public void RequestTransfer(TObject obj)
    {
        if (CanReceiveObject(obj))
        {
            obj.OnTransferApproved();
            OnReceiveObject(obj);
        }
        else
        {
            obj.OnTransferDenied();
        }
    }

    protected virtual bool CanReceiveObject(TObject obj)
    {
        bool legalTarget = _targetObjects.Contains(obj);
        bool objectNotAlreadyInContainer = !Objects.Contains(obj); // Check if the object is already in the container
        Debug.Log($"Legal target: {legalTarget}, object not already in container: {objectNotAlreadyInContainer}");

        return legalTarget && objectNotAlreadyInContainer;
    }

    protected virtual void OnReceiveObject(TObject obj)
    {
        Objects.Add(obj);
        obj.Container = this; // Set the container reference on the object
        obj.TransferApproved.Add(OnRemoveObject); // Make sure the object is removed from the container when it is transferred

        if (obj.TryGetComponent<XRGrabInteractable>(out var interactable))
        {
            EndInteraction(interactable);
        }

        obj.OnReceived();

        OnObjectReceived.Invoke(obj);
    }

    // Called right before OnReceiveObject to remove the object from the old container
    protected virtual void OnRemoveObject(TObject obj)
    {
        Objects.Remove(obj);
        obj.Container = null; // Remove the reference to the container from the object

        // De-couple the object's events from the container
        obj.RestoreRequested.Clear();
        obj.TransferApproved.Clear();
    }

    protected void OnRestoreRequested(TObject obj)
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

    // Cause the player to let go of the object if they are holding it
    protected void EndInteraction(XRBaseInteractable interactable)
    {
        var interactor = interactable.firstInteractorSelecting;

        if (interactor != null)
        {
            // Let go of the object
            interactable.interactionManager.SelectExit(interactor, interactable);
        }
    }
}
