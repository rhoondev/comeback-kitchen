using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SeasoningWeight : MonoBehaviour
{
    public event Action<Vector3, Vector3> OnCollisionWithTop;
    public event Action<Vector3> OnCollisionWithWall;

    private Rigidbody _rigidbody;
    private Vector3 _linearVelocity;
    private bool _isTouchingTop;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        transform.SetParent(null);
    }

    private void FixedUpdate()
    {
        _linearVelocity = _rigidbody.linearVelocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.name == "Top")
        {
            // Debug.Log("Collision with top detected");
            OnCollisionWithTop?.Invoke(_linearVelocity, collision.relativeVelocity);
            _isTouchingTop = true;
        }
        else if (collision.collider.gameObject.name == "Wall")
        {
            if (_isTouchingTop)
            {
                // Debug.Log("Collision with wall while touching top detected");
                OnCollisionWithWall?.Invoke(collision.relativeVelocity);
            }
            else
            {
                // Debug.Log("Collision with wall detected without touching top");
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.gameObject.name == "Top")
        {
            _isTouchingTop = false;
        }
    }
}
