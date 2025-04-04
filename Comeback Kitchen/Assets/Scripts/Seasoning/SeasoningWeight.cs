using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SeasoningWeight : MonoBehaviour
{
    public event Action<Vector3> OnCollisionWithTop;
    public event Action OnCollisionWithWall;

    private Rigidbody _rigidbody;
    private Vector3 _velocityBeforeCollision;
    private bool _touchingTop = false;
    private bool _touchingWall = false;
    private bool _alreadyTouchingTop = false;
    private bool _alreadyTouchingWall = false;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        transform.SetParent(null);
    }

    private void FixedUpdate()
    {
        _alreadyTouchingTop = _touchingTop;
        _alreadyTouchingWall = _touchingWall;

        _velocityBeforeCollision = _rigidbody.linearVelocity;

        _touchingTop = false;
        _touchingWall = false;
    }

    // private void OnCollisionEnter(Collision collision)
    // {
    //     if (collision.collider.gameObject.name == "Top")
    //     {
    //         OnCollisionWithTop?.Invoke(_velocityBeforeCollision, collision.relativeVelocity);
    //     }
    //     else if (collision.collider.gameObject.name == "Wall")
    //     {
    //         Debug.Log("Wall collision detected with seasoning weight.");
    //         OnCollisionWithWall?.Invoke();
    //     }
    // }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.gameObject.name == "Velocity Trigger")
    //     {

    //     }
    // }

    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.otherCollider.gameObject.name == "Top")
            {
                if (!_alreadyTouchingTop)
                {
                    Debug.Log($"Top collision detected with velocity {_velocityBeforeCollision}.");
                    OnCollisionWithTop?.Invoke(_velocityBeforeCollision);
                }

                _touchingTop = true;

            }
            else if (contact.otherCollider.gameObject.name == "Wall")
            {
                if (!_alreadyTouchingWall)
                {
                    Debug.Log("Wall collision detected with seasoning weight.");
                    OnCollisionWithWall?.Invoke();
                }

                _touchingWall = true;
            }
        }
    }
}
