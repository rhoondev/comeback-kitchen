using System.Collections;
using UnityEngine;

public class StaticObject : ContainerObject<StaticObject, StaticContainer>
{
    protected override void OnWaitForRestore()
    {
        base.OnWaitForRestore();

        StartCoroutine(FreezePhysicsRoutine());
    }

    // Freezes the object in place after a short delay
    private IEnumerator FreezePhysicsRoutine()
    {
        yield return new WaitForSeconds(1f);

        Rigidbody.linearVelocity = Vector3.zero;
        Rigidbody.angularVelocity = Vector3.zero;
        Rigidbody.useGravity = false;
        Rigidbody.isKinematic = true;
    }
}