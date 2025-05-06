using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public abstract class ContainerObject<TObject, TContainer> : MonoBehaviour
    where TObject : ContainerObject<TObject, TContainer>
    where TContainer : Container<TObject, TContainer>
{
    [field: SerializeField] public Rigidbody Rigidbody { get; private set; }

    public Container<TObject, TContainer> Container { get; set; } // The container that the object is in
    public bool AllowTransfer { get; set; } = true; // If true, the object can be transferred to a container

    public SmartAction<TObject> RestoreRequested = new SmartAction<TObject>();
    public SmartAction<TObject> TransferApproved = new SmartAction<TObject>();
    public SmartAction<TObject> ReEnteredContainer = new SmartAction<TObject>();

    private bool _waitingToBeRestored = false;
    protected bool _hasLeftContainer { get; private set; } = false;

    public virtual void OnTransferApproved()
    {
        // Invoke the transfer approved event to make sure that the current container can remove the object
        TransferApproved.Invoke((TObject)this);
    }

    public void OnTransferDenied()
    {
        // Don't need to do anything if the transfer is denied, as the object should collide with something and be restored anyway
        Debug.Log($"{gameObject.name} transfer denied.");
    }

    public virtual void OnReceived()
    {
        _hasLeftContainer = false;
        Debug.Log($"{gameObject.name} has been received by {Container.gameObject.name}.");
    }

    public virtual void OnReleased()
    {
        // Debug.Log($"{gameObject.name} has been released from {Container.gameObject.name}.");
    }

    public virtual void OnRestored()
    {
        _waitingToBeRestored = false;
        // Debug.Log($"{gameObject.name} has been restored to {Container.gameObject.name}.");
    }

    public void OnRestoreDenied()
    {
        // If restore request is denied, re-request after a short delay
        StartCoroutine(RequestRestoreRoutine());
    }

    protected virtual void OnWaitForRestore()
    {
        _waitingToBeRestored = true;
    }

    // This method is what is causing the object to be restored over and over again
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent != null && other.transform.parent.TryGetComponent<TContainer>(out var container))
        {
            if (container == Container)
            {
                if (_hasLeftContainer)
                {
                    ReEnteredContainer.Invoke((TObject)this);
                    _hasLeftContainer = false;
                }
            }
            else if (AllowTransfer)
            {
                string currentContainerName = Container != null ? Container.gameObject.name : "null";
                Debug.Log($"{gameObject.name} is requesting a transfer to {container.gameObject.name}. Current container is {currentContainerName}.");
                container.RequestTransfer((TObject)this);
            }
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        // We only enable the ability to be restored after the object has exited the container trigger
        if (other.transform.parent != null && other.transform.parent.TryGetComponent<TContainer>(out var container) && container == Container)
        {
            _hasLeftContainer = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Debug.Log($"{gameObject.name} has collided with {collision.gameObject.name}.");

        // Ignore collisions if the object is inside the trigger or is already waiting to be restored
        if (!_hasLeftContainer || _waitingToBeRestored)
        {
            return;
        }

        // Debug.Log($"{gameObject.name} can be resotred and is not waiting to be restored.");

        // Ignore potential collisions with container that is holding the object currently as well as any other objects that are children of the container
        if (Container != null && collision.transform.IsChildOf(Container.transform))
        {
            return;
        }

        // Debug.Log($"{gameObject.name} is not colliding with its own container.");

        // Ignore collisions if the object is being held
        if (TryGetComponent<XRGrabInteractable>(out var interactable) && interactable.isSelected)
        {
            return;
        }

        // Debug.Log($"{gameObject.name} is not being held.");


        // Debug.Log($"{gameObject.name} has been successfully collided with another object and is requesting a restore.");

        // If all conditions are met, it will request to be restored
        StartCoroutine(RequestRestoreRoutine());
        OnWaitForRestore();
    }

    private IEnumerator RequestRestoreRoutine()
    {
        yield return new WaitForSeconds(2f);

        RestoreRequested.Invoke((TObject)this);
    }
}