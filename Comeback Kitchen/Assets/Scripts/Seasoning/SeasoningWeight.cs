using System;
using System.Collections.Generic;
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
            Debug.Log($"Top collision detected with velocity {_linearVelocity}.");
            OnCollisionWithTop?.Invoke(_linearVelocity, collision.relativeVelocity);
            _isTouchingTop = true;
        }
        else if (collision.collider.gameObject.name == "Wall" && _isTouchingTop)
        {
            Debug.Log("Wall collision detected with seasoning weight.");
            OnCollisionWithWall?.Invoke(collision.relativeVelocity);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.gameObject.name == "Top")
        {
            Debug.Log("Exiting top collision.");
            _isTouchingTop = false;
        }
    }
}
