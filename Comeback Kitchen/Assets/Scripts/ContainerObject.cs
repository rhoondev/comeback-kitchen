using System;
using System.Collections;
using UnityEngine;

public class ContainerObject : MonoBehaviour
{
    [field: SerializeField] public Rigidbody Rigidbody { get; private set; }

    public Container Container { get; set; } = null;
    public SmartAction<ContainerObject> RequestRestore = new SmartAction<ContainerObject>();
    public SmartAction<ContainerObject, Container> RequestTransfer = new SmartAction<ContainerObject, Container>();

    private bool _hasCollided = false;

    private const int environmentLayer = 7;

    public virtual void OnRelease()
    {

    }

    public virtual void OnRestore()
    {
        _hasCollided = false;
    }

    public virtual void OnRestoreDenied()
    {
        // If restore request is denied, re-request after a short delay
        StartCoroutine(RequestRestoreRoutine());
    }

    public virtual void OnTransfer()
    {
        _hasCollided = false;
    }

    public virtual void OnTransferDenied()
    {
        _hasCollided = true;

        // Freeze the object for performance reasons
        Rigidbody.linearVelocity = Vector3.zero;
        Rigidbody.angularVelocity = Vector3.zero;
        Rigidbody.isKinematic = true;

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

            // Freeze the object for performance reasons
            Rigidbody.linearVelocity = Vector3.zero;
            Rigidbody.angularVelocity = Vector3.zero;
            Rigidbody.isKinematic = true;
            Debug.Log("Froze object on collision");

            StartCoroutine(RequestRestoreRoutine());
        }
    }

    private IEnumerator RequestRestoreRoutine()
    {
        yield return new WaitForSeconds(2f);
        RequestRestore.Invoke(this);
    }
}