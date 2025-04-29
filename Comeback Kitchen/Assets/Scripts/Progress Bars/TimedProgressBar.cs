using System.Collections;
using UnityEngine;

public class TimedProgressBar : SimpleProgressBar
{
    [SerializeField] private float duration;

    public SmartAction OnTimerFinished = new SmartAction();

    private float _timeElapsed;

    public void StartTimer()
    {
        StartCoroutine(TimerRoutine());
    }

    public void StopTimer()
    {
        StopCoroutine(TimerRoutine());
    }

    public void ResetTimer()
    {
        _timeElapsed = 0f;
        SetValue(0f);
    }

    private IEnumerator TimerRoutine()
    {
        _timeElapsed = 0f;

        while (_timeElapsed < duration)
        {
            SetValue(_timeElapsed / duration);
            _timeElapsed += Time.deltaTime;
            yield return null;
        }

        OnTimerFinished.Invoke();
    }
}