using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class StirringSpoon : MonoBehaviour
{
    [SerializeField] private Collider panFoodItemContainerCollider;
    [SerializeField] private StirringSystem stirringSystem;
    [SerializeField] private Transform tip;

    private Rigidbody _rigidbody;
    private bool _isStirring = false;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (_isStirring)
        {
            Vector3 r = tip.position - transform.position;
            Vector3 tipVelocity = _rigidbody.linearVelocity + Vector3.Cross(_rigidbody.angularVelocity, r);
            stirringSystem.ApplyStir(tip.position, tipVelocity);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == panFoodItemContainerCollider)
        {
            _isStirring = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == panFoodItemContainerCollider)
        {
            _isStirring = false;
        }
    }
}
