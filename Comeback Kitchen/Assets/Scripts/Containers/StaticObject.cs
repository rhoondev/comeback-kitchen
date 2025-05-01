using System.Collections;
using UnityEngine;

public class StaticObject : ContainerObject<StaticObject, StaticContainer>
{
    public override void OnTransfer()
    {
        // Freeze the object in place
        Rigidbody.linearVelocity = Vector3.zero;
        Rigidbody.angularVelocity = Vector3.zero;
        Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    protected override void OnWaitForRestore()
    {
        StartCoroutine(FreezePhysicsRoutine());
    }

    // Freezes the object in place after a short delay
    private IEnumerator FreezePhysicsRoutine()
    {
        yield return new WaitForSeconds(1f);

        Rigidbody.linearVelocity = Vector3.zero;
        Rigidbody.angularVelocity = Vector3.zero;
        Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }
}