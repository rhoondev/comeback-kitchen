using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class StirringTester : MonoBehaviour
{
    [SerializeField] private float radius;
    [SerializeField] private float speed;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Vector3 panPosition = transform.parent.position;
        float x = panPosition.x + Mathf.Sin(Time.time * speed) * radius;
        float z = panPosition.z + Mathf.Cos(Time.time * speed) * radius;
        Vector3 targetPosition = new Vector3(x, _rigidbody.position.y, z);
        _rigidbody.MovePosition(targetPosition);
    }
}
