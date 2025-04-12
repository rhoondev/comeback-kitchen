using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Shaker : MonoBehaviour
{
    [SerializeField] private ParticleSystem seasoningParticles;
    [SerializeField] private SeasoningWeight seasoningWeight;
    [SerializeField] private Rigidbody[] barriers;
    [SerializeField] private float minimumShakeSpeed;
    [SerializeField] private short shakeParticleCount;
    [SerializeField] private short shakeCycleCount;
    [SerializeField] private float shakeSpeedMultiplier;
    [SerializeField] private float minimumSprinkleSpeed;
    [SerializeField] private short sprinkleParticleCount;
    [SerializeField] private short maxSprinkleCycleCount;
    [SerializeField] private float sprinkleSpeedMultiplier;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        seasoningWeight.OnCollisionWithTop += Shake;
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

    private void Shake(Vector3 linearVelocity, Vector3 relativeVelocity)
    {
        if (relativeVelocity.magnitude > minimumShakeSpeed && Vector3.Dot(linearVelocity, transform.up) > 0.5f)
        {
            Debug.Log("Shake detected");

            var main = seasoningParticles.main;
            main.startSpeed = relativeVelocity.magnitude * shakeSpeedMultiplier;

            var emission = seasoningParticles.emission;
            emission.SetBurst(0, new ParticleSystem.Burst(0.0f, shakeParticleCount, shakeCycleCount, 0.01f));

            seasoningParticles.Play();
        }
    }

    private void Sprinkle(Vector3 relativeVelocity)
    {
        float orientation = Vector3.Dot(transform.up, Vector3.down);
        Vector3 horizontalVelocity = relativeVelocity - Vector3.Dot(relativeVelocity, transform.up) * transform.up;

        if (orientation > 0f && horizontalVelocity.magnitude > minimumSprinkleSpeed)
        {
            Debug.Log("Sprinkle detected");

            var main = seasoningParticles.main;
            main.startSpeed = relativeVelocity.magnitude * sprinkleSpeedMultiplier;

            var emission = seasoningParticles.emission;
            emission.SetBurst(0, new ParticleSystem.Burst(0.0f, sprinkleParticleCount, (short)(maxSprinkleCycleCount * orientation), 0.01f));

            seasoningParticles.Play();
        }
    }
}
