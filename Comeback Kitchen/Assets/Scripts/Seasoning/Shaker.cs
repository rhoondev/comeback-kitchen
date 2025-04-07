using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Shaker : MonoBehaviour
{
    [SerializeField] private ParticleSystem seasoningParticles;
    [SerializeField] private SeasoningWeight seasoningWeight;
    [SerializeField] private Rigidbody[] barriers;
    [SerializeField] private short dashParticleCount;
    [SerializeField] private short maxSprinkleParticleCount;
    [SerializeField] private float minimumDashSpeed;
    [SerializeField] private float minimumSprinkleSpeed;

    private Rigidbody _rigidbody;

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
        if (relativeVelocity.magnitude > minimumDashSpeed && Vector3.Dot(linearVelocity, transform.up) > 0.5f)
        {
            var main = seasoningParticles.main;
            main.startSpeed = relativeVelocity.magnitude * 0.5f;

            var emission = seasoningParticles.emission;
            var burst = emission.GetBurst(0);
            burst.count = dashParticleCount;

            seasoningParticles.Play();
        }
    }

    private void Sprinkle(Vector3 relativeVelocity)
    {
        float orientation = Vector3.Dot(transform.up, Vector3.down);
        Vector3 horizontalVelocity = relativeVelocity - Vector3.Dot(relativeVelocity, transform.up) * transform.up;

        if (orientation > 0f && horizontalVelocity.magnitude > minimumSprinkleSpeed)
        {
            var main = seasoningParticles.main;
            main.startSpeed = 0.0f;

            var emission = seasoningParticles.emission;
            var burst = emission.GetBurst(0);
            burst.count = (short)(maxSprinkleParticleCount * orientation);

            seasoningParticles.Play();
        }
    }
}
