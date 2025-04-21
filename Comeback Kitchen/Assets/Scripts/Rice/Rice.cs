using System.Collections;
using UnityEngine;

public class Rice : MonoBehaviour
{
    [SerializeField] private Collider myCollider;
    [SerializeField] private Rigidbody myRigidbody;

    private bool _hasExitedContainer = false;
    private bool _hasCollided = false;

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Container _))
        {
            _hasExitedContainer = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_hasExitedContainer && !_hasCollided)
        {
            myRigidbody.linearVelocity = Vector3.zero;
            myRigidbody.isKinematic = true;
            myCollider.enabled = false;
            _hasCollided = true;
            StartCoroutine(DestroyAfterDelay(5f));
        }
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
