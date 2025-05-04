using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// A class for tracking ownership of objects such as grains of rice or pieces of vegetables
public abstract class Container<TObject, TContainer> : MonoBehaviour
    where TObject : ContainerObject<TObject, TContainer>
    where TContainer : Container<TObject, TContainer>
{
    [field: SerializeField] public Transform ObjectHolder { get; private set; } // The transform that holds the objects in the container
    [SerializeField] private MeshRenderer triggerMeshRenderer; // The mesh renderer that shows where the trigger collider is
    [SerializeField] private GameObject indicatorArrow; // Visual indicator that draws the user's attention to the container when it is receiving objects
    [SerializeField] private bool showTriggerMesh; // If true, the indicator zone is used to show the area where objects can be placed
    [SerializeField] private bool enableReceivingObjectsOnAwake; // If true, the container will be able to receive objects when it is created

    public HashSet<TObject> Objects { get; set; } = new HashSet<TObject>(); // All objects which are owned by this container
    public SmartAction<TObject> OnObjectReceived = new SmartAction<TObject>(); // Invoked when an object is added to the container
    // public SmartAction<TObject> OnObjectRemoved = new SmartAction<TObject>(); // Invoked when an object is removed from the container

    private HashSet<TObject> _targetObjects = null; // If not null, this is the only object that can be received by the container. If null, any object can be received.
    protected bool _isReceivingObjects = false; // Whether the container is currently able to receive objects

    // WARNING: If objects in the object holder of a StaticContainer do not match up with the data asset, the container will not work properly
    protected virtual void Awake()
    {
        if (enableReceivingObjectsOnAwake)
        {
            EnableReceivingObjects();
        }
    }

    public void SetTargetObject(TObject obj)
    {
        _targetObjects = obj ? new HashSet<TObject> { obj } : null; // If obj is null, any object can be received
    }

    public void SetTargetObjects(HashSet<TObject> objects)
    {
        _targetObjects = objects;
    }

    public virtual void EnableReceivingObjects()
    {
        _isReceivingObjects = true;

        indicatorArrow.SetActive(true);

        if (showTriggerMesh)
        {
            triggerMeshRenderer.enabled = true;
        }
    }

    public virtual void DisableReceivingObjects()
    {
        _isReceivingObjects = false;

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
        // Three conditions must be met to receive an object:
        // 1. The container must be able to receive objects
        // 2. The object must be the target object (if one exists)
        // 3. The object must not already be in the container
        return _isReceivingObjects && (_targetObjects == null || _targetObjects.Contains(obj)) && !Objects.Contains(obj);
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

    protected virtual void OnRemoveObject(TObject obj)
    {
        Objects.Remove(obj);
        obj.Container = null; // Remove the reference to the container from the object

        // De-couple the object's events from the container
        obj.RestoreRequested.Clear();
        obj.TransferApproved.Clear();
        obj.ReEntered.Clear();
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

    private void EndInteraction(XRBaseInteractable interactable)
    {
        var interactor = interactable.firstInteractorSelecting;

        if (interactor != null)
        {
            // if (interactor is XRBaseInteractor baseInteractor)
            // {
            //     baseInteractor.EndManualInteraction();
            // }

            // Let go of the object
            interactable.interactionManager.SelectExit(interactor, interactable);
        }
    }
}
