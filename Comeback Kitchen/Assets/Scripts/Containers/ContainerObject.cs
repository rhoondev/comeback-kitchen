using System.Collections;
using UnityEngine;

public class ContainerObject : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    public Container Container { get; set; } = null;
    public SmartAction<ContainerObject> RequestRestore = new SmartAction<ContainerObject>();
    public SmartAction<ContainerObject, Container> RequestTransfer = new SmartAction<ContainerObject, Container>();

    private bool _hasCollided = false;

    private const int environmentLayer = 7;

    public virtual void OnRelease()
    {
        rb.isKinematic = false;
    }

    public virtual void OnRestore()
    {
        _hasCollided = false;

        // Object should already be frozen when restored
        // It either collided with the environment or was denied transfer to another container
        // In either case, the object is frozen
    }

    public virtual void OnRestoreDenied()
    {
        // If restore request is denied, re-request after a short delay
        StartCoroutine(RequestRestoreRoutine());
    }

    public virtual void OnTransfer()
    {
        if (Container is StaticContainer)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }

    public virtual void OnTransferDenied()
    {
        _hasCollided = true;

        StartCoroutine(FreezePhysicsRoutine());
        StartCoroutine(RequestRestoreRoutine());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Container>(out var otherContainer) && otherContainer != Container)
        {
            RequestTransfer.Invoke(this, otherContainer);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_hasCollided && collision.gameObject.layer == environmentLayer)
        {
            _hasCollided = true;

            Debug.Log($"{gameObject.name} collided with environmental object: {collision.gameObject.name}.");

            StartCoroutine(FreezePhysicsRoutine());
            StartCoroutine(RequestRestoreRoutine());
        }
    }

    private IEnumerator RequestRestoreRoutine()
    {
        yield return new WaitForSeconds(3f);
        RequestRestore.Invoke(this);
    }

    private IEnumerator FreezePhysicsRoutine()
    {
        yield return new WaitForSeconds(1f);

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
    }
}