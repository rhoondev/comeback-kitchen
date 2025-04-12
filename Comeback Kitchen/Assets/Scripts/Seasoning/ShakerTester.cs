using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShakerTester : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private bool _isMoving = false;
    private Vector3 _position;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _position = _rigidbody.position;
    }

    public void Shake()
    {
        if (!_isMoving)
        {
            StartCoroutine(ShakeRoutine());
        }
    }

    public void Sprinkle()
    {
        if (!_isMoving)
        {
            StartCoroutine(SprinkleRoutine());
        }
    }

    private void FixedUpdate()
    {
        _rigidbody.MovePosition(_position);
    }

    private IEnumerator ShakeRoutine()
    {
        _isMoving = true;

        Vector3 axis = -transform.up;
        Vector3 startPos = _rigidbody.position;
        Vector3 endPos = _rigidbody.position + axis * 0.05f;
        float duration = 0.25f;
        float startTime = Time.time;
        float endTime = Time.time + duration;

        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / duration;
            float a = t - 0.5f;
            _position = Vector3.Lerp(startPos, endPos, 1f - 4f * a * a);

            yield return null;
        }

        _position = startPos;
        _isMoving = false;
    }

    private IEnumerator SprinkleRoutine()
    {
        _isMoving = true;

        float duration = 2f;
        int backAndForthCount = 6;

        float startTime = Time.time;
        float endTime = Time.time + duration;
        Vector3 startPos = _rigidbody.position;
        Vector3 endPos = _rigidbody.position + transform.right * 0.02f;

        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / duration;
            float a = 1 - Mathf.Abs(2f * backAndForthCount * (t % (1f / backAndForthCount)) - 1f);
            _position = Vector3.Lerp(startPos, endPos, a);

            yield return null;
        }

        _position = startPos;
        _isMoving = false;
    }
}
