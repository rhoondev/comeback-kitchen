using System.Collections;
using UnityEngine;

public class SpawnedRice : MonoBehaviour
{
    [SerializeField] private float rigidbodyDelay;
    [SerializeField] private float colliderDelay;
    [SerializeField] private Collider myCollider;
    [SerializeField] private Rigidbody myRigidbody;

    private bool _hasCollided = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (!_hasCollided)
        {
            StartCoroutine(DisablePhysicsAfterDelay());
            _hasCollided = true;
        }
    }

    private IEnumerator DisablePhysicsAfterDelay()
    {
        yield return new WaitForSeconds(rigidbodyDelay);

        myRigidbody.isKinematic = true;

        yield return new WaitForSeconds(colliderDelay - rigidbodyDelay);

        myCollider.enabled = false;
    }
}
