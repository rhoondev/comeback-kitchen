using System.Collections.Generic;
using UnityEngine;

public class LoadedRice : MonoBehaviour
{
    [SerializeField] private Collider myCollider;
    [SerializeField] private Rigidbody myRigidbody;

    private bool _hasExitedContainer = false;
    private bool _hasCollided = false;

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out RiceContainer _))
        {
            _hasExitedContainer = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_hasExitedContainer && !_hasCollided)
        {
            myRigidbody.isKinematic = true;
            myCollider.enabled = false;
            _hasCollided = true;
        }
    }
}
