using System.Collections;
using UnityEngine;

public class Stirrable : MonoBehaviour
{
    [SerializeField] private ParticleSystem smokeParticles;
    [SerializeField] private float minTimeToStartBurning;
    [SerializeField] private float maxTimeToStartBurning;
    [SerializeField] private float burningToBurntTime;

    public SmartAction OnBurnt = new SmartAction();

    private float _maxSmokeEmissionRate;

    private void Awake()
    {
        _maxSmokeEmissionRate = smokeParticles.emission.rateOverTime.constantMax;
    }

    public void StartCooking()
    {
        StartCoroutine(BurnRoutine());
    }

    public void StopCooking()
    {
        StopAllCoroutines();

        if (smokeParticles.isPlaying)
        {
            smokeParticles.Stop();
        }
    }

    public void ResetBurnTimer()
    {
        StopCooking();
        StartCoroutine(BurnRoutine());
    }

    private IEnumerator BurnRoutine()
    {
        yield return new WaitForSeconds(Random.Range(minTimeToStartBurning, maxTimeToStartBurning));

        var emission = smokeParticles.emission;
        emission.rateOverTime = 0f;
        smokeParticles.Play();

        float timePassed = 0f;

        while (timePassed < burningToBurntTime)
        {
            float t = timePassed / burningToBurntTime;
            emission.rateOverTime = _maxSmokeEmissionRate * t;

            timePassed += Time.deltaTime;
            yield return null;
        }

        OnBurnt.Invoke();
    }
}