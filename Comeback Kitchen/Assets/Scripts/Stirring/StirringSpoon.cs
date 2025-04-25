using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class StirringSpoon : MonoBehaviour
{
    [SerializeField] private StirringManager pan;
    [SerializeField] private Transform tip;

    private Rigidbody _rigidbody;
    private bool _inPan = false;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (_inPan)
        {
            Vector3 r = tip.position - transform.position;
            Vector3 tipVelocity = _rigidbody.linearVelocity + Vector3.Cross(_rigidbody.angularVelocity, r);
            pan.ApplyStir(tip.position, tipVelocity);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == pan.gameObject)
        {
            _inPan = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == pan.gameObject)
        {
            _inPan = false;
        }
    }
}
