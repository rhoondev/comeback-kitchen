using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class StirringTester : MonoBehaviour
{
    enum StirringType
    {
        Circular,
        BackAndForth
    }

    [SerializeField] private Transform pan;
    [SerializeField] private StirringType stirringType;
    [SerializeField] private float verticalOffset;
    [SerializeField] private float radius;
    [SerializeField] private float circleSpeed;
    [SerializeField] private float backAndForthSpeed;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (stirringType == StirringType.Circular)
        {
            Vector3 offset = new Vector3(
                Mathf.Cos(Time.time * circleSpeed) * radius,
                verticalOffset,
                Mathf.Sin(Time.time * circleSpeed) * radius
            );

            _rigidbody.MovePosition(pan.position + offset);
        }
        else if (stirringType == StirringType.BackAndForth)
        {
            Vector3 offset = new Vector3(Mathf.PingPong(Time.time * backAndForthSpeed, radius * 2f) - radius, verticalOffset, 0f);
            _rigidbody.MovePosition(pan.position + offset);
        }
    }
}
