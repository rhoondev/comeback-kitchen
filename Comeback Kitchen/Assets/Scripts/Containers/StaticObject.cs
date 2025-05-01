using System.Collections;
using UnityEngine;

public class StaticObject : ContainerObject<StaticObject, StaticContainer>
{
    public override void OnTransfer()
    {
        Rigidbody.linearVelocity = Vector3.zero;
        Rigidbody.angularVelocity = Vector3.zero;
        Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    protected override void OnWaitForRestore()
    {
        // Freeze the object after a short delay to improve performance
        // Will also mean that the object is frozen when it is restored
        StartCoroutine(FreezePhysicsRoutine());
    }

    private IEnumerator FreezePhysicsRoutine()
    {
        yield return new WaitForSeconds(1f);

        Rigidbody.linearVelocity = Vector3.zero;
        Rigidbody.angularVelocity = Vector3.zero;
        Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }
}