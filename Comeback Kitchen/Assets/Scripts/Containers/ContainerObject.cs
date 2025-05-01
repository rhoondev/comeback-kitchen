using System.Collections;
using UnityEngine;

public abstract class ContainerObject<TObject, TContainer> : MonoBehaviour
    where TObject : ContainerObject<TObject, TContainer>
    where TContainer : Container<TObject, TContainer>
{
    [field: SerializeField] public Rigidbody Rigidbody { get; private set; }

    public Container<TObject, TContainer> Container { get; set; } = null;
    public SmartAction<TObject> RequestRestore = new SmartAction<TObject>();
    public SmartAction<TObject, TContainer> RequestTransfer = new SmartAction<TObject, TContainer>();

    private bool _waitingForRestore = false;

    private const int environmentLayer = 7;

    public void OnRelease()
    {
        Rigidbody.constraints = RigidbodyConstraints.None;
    }

    public virtual void OnRestore()
    {
        _waitingForRestore = false;
    }

    public void OnRestoreDenied()
    {
        // If restore request is denied, re-request after a short delay
        StartCoroutine(RequestRestoreRoutine());
    }

    public abstract void OnTransfer();
    protected abstract void OnWaitForRestore();

    public void OnTransferDenied()
    {
        _waitingForRestore = true;

        StartCoroutine(RequestRestoreRoutine());
        OnWaitForRestore();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<TContainer>(out var otherContainer) && otherContainer != Container)
        {
            RequestTransfer.Invoke((TObject)this, otherContainer);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_waitingForRestore && collision.gameObject.layer == environmentLayer)
        {
            _waitingForRestore = true;

            Debug.Log($"{gameObject.name} collided with environmental object: {collision.gameObject.name}.");

            StartCoroutine(RequestRestoreRoutine());
            OnWaitForRestore();
        }
    }

    private IEnumerator RequestRestoreRoutine()
    {
        yield return new WaitForSeconds(3f);

        RequestRestore.Invoke((TObject)this);
    }
}