using System;
using System.Collections;
using UnityEngine;

public class ContainerObject : MonoBehaviour
{
    [field: SerializeField] public Rigidbody Rigidbody { get; private set; }

    public SmartAction<ContainerObject> RequestRestore = new SmartAction<ContainerObject>();
    public SmartAction<ContainerObject, Container> RequestTransfer = new SmartAction<ContainerObject, Container>();

    protected bool _hasExitedContainer = false;
    protected bool _hasCollided = false;

    public virtual void OnRelease()
    {

    }

    public virtual void OnRestore()
    {
        _hasExitedContainer = false;
        _hasCollided = false;
    }

    public virtual void OnRestoreDenied()
    {
        // If restore request is denied, re-request after a short delay
        StartCoroutine(RequestRestoreRoutine());
    }

    public virtual void OnTransfer()
    {
        _hasExitedContainer = false;
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

    private void OnTriggerExit(Collider other)
    {
        _hasExitedContainer = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_hasExitedContainer)
        {
            return; // Object is likely detecting trigger on its own container
        }

        if (other.TryGetComponent<Container>(out var container))
        {
            RequestTransfer.Invoke(this, container);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_hasCollided)
        {
            _hasCollided = true;

            // Freeze the object for performance reasons
            Rigidbody.linearVelocity = Vector3.zero;
            Rigidbody.angularVelocity = Vector3.zero;
            Rigidbody.isKinematic = true;

            StartCoroutine(RequestRestoreRoutine());
        }
    }

    private IEnumerator RequestRestoreRoutine()
    {
        yield return new WaitForSeconds(2f);
        RequestRestore.Invoke(this);
    }
}