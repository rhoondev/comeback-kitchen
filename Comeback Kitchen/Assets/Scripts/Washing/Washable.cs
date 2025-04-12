using System.Collections.Generic;
using UnityEngine;

public class Washable : MonoBehaviour
{
    [SerializeField] private int washQuota;

    public bool IsWashed { get => _amountWashed >= washQuota; }

    private int _amountWashed = 0;

    private void Wash(int amount)
    {
        bool wasWashed = IsWashed;

        _amountWashed += amount;

        if (IsWashed && !wasWashed)
        {
            // Handle the object being washed
            Debug.Log($"{gameObject.name} is now clean!");
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.TryGetComponent<Stream>(out var _))
        {
            var collisionEvents = new List<ParticleCollisionEvent>();
            other.GetComponent<ParticleSystem>().GetCollisionEvents(other, collisionEvents);
            Wash(collisionEvents.Count);
        }
    }
}
