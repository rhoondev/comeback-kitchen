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
        TransferApproved.Invoke((TObject)this);
    }

    public void OnTransferDenied()
    {
        _waitingToBeRestored = true;

        StartCoroutine(RequestRestoreRoutine());
        OnWaitForRestore();
    }

    public virtual void OnReceived()
    {
        _canBeRestored = false;
    }

    public void OnReleased()
    {
        // Re-enable motion on the Rigidbody
        Rigidbody.constraints = RigidbodyConstraints.None;
    }

    public virtual void OnRestored()
    {
        _waitingToBeRestored = false;
        _canBeRestored = false;
    }

    public void OnRestoreDenied()
    {
        // If restore request is denied, re-request after a short delay
        StartCoroutine(RequestRestoreRoutine());
    }

    protected abstract void OnWaitForRestore();

    private void OnTriggerEnter(Collider other)
    {
        if (!AllowTransfer)
        {
            return;
        }

        // If the object enters a container trigger (which is always a child of the container), request a transfer
        if (other.transform.parent != null && other.transform.parent.TryGetComponent<TContainer>(out var container) && container != Container)
        {
            Debug.Log($"{gameObject.name} entered container trigger: {other.transform.parent.name}.");

            container.RequestTransfer((TObject)this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Only after the object exits a container trigger (which is always a child of the container) should the ability to be restored be enabled
        if (other.transform.parent != null && other.transform.parent.TryGetComponent<TContainer>(out var container) && container == Container)
        {
            Debug.Log($"{gameObject.name} exited container trigger: {other.transform.parent.name}.");

            _canBeRestored = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If the object collides with the environment and is not already waiting to be restored, request a restore
        if (_canBeRestored && !_waitingToBeRestored && collision.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            _waitingToBeRestored = true;

            Debug.Log($"{gameObject.name} collided with environmental object: {collision.gameObject.name}.");

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