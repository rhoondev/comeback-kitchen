using System.Collections;
using UnityEngine;

public abstract class ContainerObject<TObject, TContainer> : MonoBehaviour
    where TObject : ContainerObject<TObject, TContainer>
    where TContainer : Container<TObject, TContainer>
{
    [field: SerializeField] public Rigidbody Rigidbody { get; private set; }

    public Container<TObject, TContainer> Container { get; set; } // The container that the object is in
    public bool AllowTransfer { get; set; } = true; // If true, the object can be transferred to a container

    public SmartAction<TObject> RestoreRequested = new SmartAction<TObject>();
    public SmartAction<TObject> TransferApproved = new SmartAction<TObject>();

    private bool _waitingToBeRestored = false;
    private bool _canBeRestored = false;

    public virtual void OnTransferApproved()
    {
        // Invoke the transfer approved event to make sure that the current container can remove the object
        TransferApproved.Invoke((TObject)this);
    }

    public void OnTransferDenied()
    {
        StartCoroutine(RequestRestoreRoutine());
        OnWaitForRestore();
    }

    public virtual void OnReceived()
    {
        _canBeRestored = false;
        Debug.Log($"{gameObject.name} has been received by {Container.gameObject.name}.");
    }

    public virtual void OnReleased()
    {
        Debug.Log($"{gameObject.name} has been released from {Container.gameObject.name}.");
    }

    public virtual void OnRestored()
    {
        _waitingToBeRestored = false;
        _canBeRestored = false;
        Debug.Log($"{gameObject.name} has been restored to {Container.gameObject.name}.");
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

    private void OnTriggerEnter(Collider other)
    {
        // If the object is allowed to be transferred and it enters another container trigger (which is always a child of the container), request a transfer
        if (AllowTransfer && other.transform.parent != null && other.transform.parent.TryGetComponent<TContainer>(out var container) && container != Container)
        {
            container.RequestTransfer((TObject)this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Only after the object exits a container trigger (which is always a child of the container) should the ability to be restored be enabled
        if (other.transform.parent != null && other.transform.parent.TryGetComponent<TContainer>(out var container) && container == Container)
        {
            _canBeRestored = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If the object collides with the environment and is not already waiting to be restored, request a restore
        if (_canBeRestored && !_waitingToBeRestored && collision.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            Debug.Log($"{gameObject.name} has collided with the environment ({collision.gameObject.name}) and is requesting a restore.");

            StartCoroutine(RequestRestoreRoutine());
            OnWaitForRestore();
        }
    }

    private IEnumerator RequestRestoreRoutine()
    {
        yield return new WaitForSeconds(3f);

        RestoreRequested.Invoke((TObject)this);
    }
}