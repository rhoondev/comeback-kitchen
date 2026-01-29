using System.Collections;
using UnityEngine;

public class StaticObject : ContainerObject<StaticObject, StaticContainer>
{
    private Coroutine _freezeCoroutine = null;

    protected override void OnWaitForRestore()
    {
        base.OnWaitForRestore();

        // Cancel any existing freeze coroutine before starting a new one
        if (_freezeCoroutine != null)
        {
            StopCoroutine(_freezeCoroutine);
        }
        _freezeCoroutine = StartCoroutine(FreezePhysicsRoutine());
    }

    public override void OnRestored()
    {
        base.OnRestored();

        // Cancel the freeze coroutine if it's still running (restore happened before freeze)
        if (_freezeCoroutine != null)
        {
            StopCoroutine(_freezeCoroutine);
            _freezeCoroutine = null;
        }
    }

    public override void OnReceived()
    {
        base.OnReceived();

        // Cancel the freeze coroutine if object was transferred to a new container
        if (_freezeCoroutine != null)
        {
            StopCoroutine(_freezeCoroutine);
            _freezeCoroutine = null;
        }
    }

    // Freezes the object in place after a short delay
    private IEnumerator FreezePhysicsRoutine()
    {
        yield return new WaitForSeconds(1f);

        Rigidbody.linearVelocity = Vector3.zero;
        Rigidbody.angularVelocity = Vector3.zero;
        Rigidbody.useGravity = false;
        Rigidbody.isKinematic = true;
        Rigidbody.interpolation = RigidbodyInterpolation.None;

        _freezeCoroutine = null;
    }
}