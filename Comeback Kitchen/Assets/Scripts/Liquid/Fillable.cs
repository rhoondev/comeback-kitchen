using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Liquid)), RequireComponent(typeof(BoxCollider))]
public class Fillable : MonoBehaviour
{
    private Liquid _liquid;
    private BoxCollider _boxCollider;

    private void Start()
    {
        _liquid = GetComponent<Liquid>();
        _boxCollider = GetComponent<BoxCollider>();
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.TryGetComponent(out Stream stream) && _liquid.Type == stream.Type)
        {
            if (!_liquid.IsFull)
            {
                var collisionEvents = new List<ParticleCollisionEvent>();
                other.GetComponent<ParticleSystem>().GetCollisionEvents(other, collisionEvents);
                _liquid.Fill(collisionEvents.Count);

                float fillAmount = (float)_liquid.FillCount / _liquid.MaxFillCount;
                float fillHeight = _liquid.MaxFillHeight * fillAmount;

                _boxCollider.size = new Vector3(_boxCollider.size.x, fillHeight, _boxCollider.size.z);
                _boxCollider.center = new Vector3(_boxCollider.center.x, fillHeight / 2f, _boxCollider.center.z);
            }
        }
    }
}
