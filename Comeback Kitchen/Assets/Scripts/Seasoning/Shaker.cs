using System.Collections;
using UnityEngine;

public class Shaker : MonoBehaviour
{
    [SerializeField] private ParticleSystem dashParticles;
    [SerializeField] private ParticleSystem sprinkleParticles;
    [SerializeField] private SeasoningWeight seasoningWeight;

    private float _sprinkleUntil;

    private void Start()
    {
        seasoningWeight.OnCollisionWithTop += Dash;
        seasoningWeight.OnCollisionWithWall += Sprinkle;
    }

    private void Update()
    {
        if (Time.time > _sprinkleUntil)
        {
            sprinkleParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    private void Dash(Vector3 velocity)
    {
        if (velocity.magnitude > 0.25f && Vector3.Dot(velocity, transform.up) > 0.5f)
        {
            var main = dashParticles.main;
            main.startSpeed = velocity.magnitude * 0.5f;
            dashParticles.Play();
        }
    }

    private void Sprinkle()
    {
        if (Vector3.Dot(transform.up, Vector3.down) > 0.1f)
        {
            if (!sprinkleParticles.isPlaying)
            {
                sprinkleParticles.Play();
            }

            _sprinkleUntil = Time.time + 0.5f;
        }
    }
}
