using System;
using System.Collections;
using UnityEngine;

public class ContainerObject : MonoBehaviour
{
    [field: SerializeField] public Rigidbody Rigidbody { get; private set; }

    public Container Container { get; set; } = null;
    public SmartAction<ContainerObject> RequestRestore = new SmartAction<ContainerObject>();
    public SmartAction<ContainerObject, Container> RequestTransfer = new SmartAction<ContainerObject, Container>();

    private bool _freezePhysicsRequested = false;

    private const int environmentLayer = 7;

    public virtual void OnRelease()
    {

    }

    public virtual void OnRestore()
    {
        _freezePhysicsRequested = false;
    }

    public virtual void OnRestoreDenied()
    {
        // If restore request is denied, re-request after a short delay
        StartCoroutine(RequestRestoreRoutine());
    }

    public virtual void OnTransfer()
    {
        _freezePhysicsRequested = false;
    }

    public virtual void OnTransferDenied()
    {
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
        if (!_freezePhysicsRequested && collision.gameObject.layer == environmentLayer)
        {
            Debug.Log($"Collision with environmental object: {collision.gameObject.name}.");

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
        _freezePhysicsRequested = true;

        yield return new WaitForSeconds(1f);

        Rigidbody.linearVelocity = Vector3.zero;
        Rigidbody.angularVelocity = Vector3.zero;
        Rigidbody.isKinematic = true;
    }
}