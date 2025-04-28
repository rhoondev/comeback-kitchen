using System.Collections;
using UnityEngine;

public class Stirrable : MonoBehaviour
{
    [SerializeField] private ParticleSystem smokeParticles;
    [SerializeField] private float minTimeToStartBurning;
    [SerializeField] private float maxTimeToStartBurning;
    [SerializeField] private float burningToBurntTime;

    public SmartAction<Stirrable> OnStartBurning = new SmartAction<Stirrable>();
    public SmartAction<Stirrable> OnStopBurning = new SmartAction<Stirrable>();
    public SmartAction OnBurnt = new SmartAction();

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
            OnStopBurning.Invoke(this);
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

        smokeParticles.Play();
        OnStartBurning.Invoke(this);

        yield return new WaitForSeconds(burningToBurntTime);

        OnBurnt.Invoke();
    }
}