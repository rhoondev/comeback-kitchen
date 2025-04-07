using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Shaker : MonoBehaviour
{
    [SerializeField] private ParticleSystem seasoningParticles;
    [SerializeField] private SeasoningWeight seasoningWeight;
    [SerializeField] private Rigidbody[] barriers;
    [SerializeField] private float dashParticleProbability;
    [SerializeField] private float sprinkleMaxParticleProbability;

    private Rigidbody _rigidbody;
    private int _burstCount;
    private int _burstCycleCount;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        seasoningWeight.OnCollisionWithTop += Dash;
        seasoningWeight.OnCollisionWithWall += Sprinkle;

        foreach (var barrier in barriers)
        {
            barrier.transform.SetParent(null);
        }

        _burstCount = seasoningParticles.emission.GetBurst(0).maxCount;
        _burstCycleCount = seasoningParticles.emission.GetBurst(0).maxCount;
    }

    private void FixedUpdate()
    {
        foreach (var barrier in barriers)
        {
            barrier.MovePosition(_rigidbody.position);
            barrier.MoveRotation(_rigidbody.rotation);
        }
    }

    private void Dash(Vector3 linearVelocity, Vector3 relativeVelocity)
    {
        if (relativeVelocity.magnitude > 0.3f && Vector3.Dot(linearVelocity, transform.up) > 0.5f)
        {
            var main = seasoningParticles.main;
            main.startSpeed = relativeVelocity.magnitude * 0.5f;

            var emission = seasoningParticles.emission;
            emission.SetBurst(0, new ParticleSystem.Burst(0.0f, _burstCount, _burstCycleCount, dashParticleProbability));

            seasoningParticles.Play();
        }
    }

    private void Sprinkle(Vector3 relativeVelocity)
    {
        float dot = Vector3.Dot(transform.up, Vector3.down);

        if (relativeVelocity.magnitude > 0.1f && dot > 0f)
        {
            var main = seasoningParticles.main;
            main.startSpeed = 0.0f;

            var emission = seasoningParticles.emission;
            emission.SetBurst(0, new ParticleSystem.Burst(0.0f, _burstCount, _burstCycleCount, dot * sprinkleMaxParticleProbability));

            seasoningParticles.Play();
        }
    }
}
