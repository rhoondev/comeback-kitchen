using System.Collections;
using UnityEngine;

public abstract class ContainerObject<TObject, TContainer> : MonoBehaviour
    where TObject : ContainerObject<TObject, TContainer>
    where TContainer : Container<TObject, TContainer>
{
    [field: SerializeField] public Rigidbody Rigidbody { get; private set; }

    public SmartAction<TObject> RestoreRequested = new SmartAction<TObject>();
    public SmartAction<TObject, TContainer> TransferRequested = new SmartAction<TObject, TContainer>();

    private bool _waitingToBeRestored = false;

    private const int environmentLayer = 7; // WARNING: This is hardcoded to the environment layer in the Unity editor. If you change the layer, you must also change this value.

    public abstract void OnTransfer();

    public void OnTransferDenied()
    {
        _waitingToBeRestored = true;

        StartCoroutine(RequestRestoreRoutine());
        OnWaitForRestore();
    }

    public void OnRelease()
    {
        // Re-enable motion on the Rigidbody
        Rigidbody.constraints = RigidbodyConstraints.None;
    }

    public virtual void OnRestore()
    {
        _waitingToBeRestored = false;
    }

    public void OnRestoreDenied()
    {
        // If restore request is denied, re-request after a short delay
        StartCoroutine(RequestRestoreRoutine());
    }

    protected abstract void OnWaitForRestore();

    private void OnTriggerEnter(Collider other)
    {
        // If the object enters a container, request a transfer
        if (other.TryGetComponent<TContainer>(out var container))
        {
            TransferRequested.Invoke((TObject)this, container);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If the object collides with the environment and is not already waiting to be restored, request a restore
        if (!_waitingToBeRestored && collision.gameObject.layer == environmentLayer)
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