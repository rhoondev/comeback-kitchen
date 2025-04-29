using UnityEngine;
using UnityEngine.InputSystem;

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
    [SerializeField] private float stirringVerticalOffset;
    [SerializeField] private float notStirringVerticalOffset;
    [SerializeField] private float radius;
    [SerializeField] private float circleSpeed;
    [SerializeField] private float backAndForthSpeed;

    private Rigidbody _rigidbody;
    private bool _isStirring = false;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            _isStirring = !_isStirring;
        }
    }

    private void FixedUpdate()
    {
        if (_isStirring)
        {
            if (stirringType == StirringType.Circular)
            {
                Vector3 offset = new Vector3(
                    Mathf.Cos(Time.time * circleSpeed) * radius,
                    stirringVerticalOffset,
                    Mathf.Sin(Time.time * circleSpeed) * radius
                );

                _rigidbody.MovePosition(pan.position + offset);
            }
            else if (stirringType == StirringType.BackAndForth)
            {
                Vector3 offset = new Vector3(Mathf.PingPong(Time.time * backAndForthSpeed, radius * 2f) - radius, stirringVerticalOffset, 0f);
                _rigidbody.MovePosition(pan.position + offset);
            }
        }
        else
        {
            _rigidbody.MovePosition(pan.position + Vector3.up * notStirringVerticalOffset);
        }
    }
}
