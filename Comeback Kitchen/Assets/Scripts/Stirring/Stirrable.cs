using System.Collections;
using UnityEngine;

public class Stirrable : MonoBehaviour
{
    [SerializeField] private ParticleSystem smokeParticles;
    [SerializeField] private float timeToStartBurning;
    [SerializeField] private float burningToBurntTime;

    public SmartAction<Stirrable> OnStartBurning = new SmartAction<Stirrable>();
    public SmartAction<Stirrable> OnStopBurning = new SmartAction<Stirrable>();
    public SmartAction OnBurnt = new SmartAction();

    public void StartCooking()
    {
        StartCoroutine(BurnRoutine());
    }

    public void FinishCooking()
    {
        StopBurning();
    }

    public void ResetBurnTimer()
    {
        StopBurning();
        StartCoroutine(BurnRoutine());
    }

    private void StopBurning()
    {
        StopAllCoroutines();

        if (smokeParticles.isPlaying)
        {
            smokeParticles.Stop();
            OnStopBurning.Invoke(this);
        }
    }

    private IEnumerator BurnRoutine()
    {
        yield return new WaitForSeconds(timeToStartBurning);

        smokeParticles.Play();
        OnStartBurning.Invoke(this);

        yield return new WaitForSeconds(burningToBurntTime);

        OnBurnt.Invoke();
    }
}