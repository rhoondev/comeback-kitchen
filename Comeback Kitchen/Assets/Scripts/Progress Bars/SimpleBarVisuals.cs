using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class SimpleBarVisuals : MonoBehaviour
{
    
    [Header("Bar Event Subscription")]
    public SmartAction OnTimerFinished = new SmartAction();
    
    [Header("Bar Variables")]
    [SerializeField] private float totalBarTime;
    [SerializeField] private float timeElapsed;
    [SerializeField] private Image pBar;
    [SerializeField] private Color barColor;
    [SerializeField] private GameObject barPosition;

    private bool timerCounting = false;


    


    // Update is called once per frame
    void Update()
    {
        if(timeElapsed < totalBarTime && timerCounting)
        {
            timeElapsed += Time.deltaTime;
            pBar.fillAmount = timeElapsed / totalBarTime;
        }
        else if(timeElapsed >= totalBarTime)
            OnTimerFinished.Invoke();
            
    }



    public void StartTimer()
    {
        timerCounting = true;
    }


    public void ResetTimer()
    {
        timeElapsed = 0;
        pBar.fillAmount = timeElapsed;
        timerCounting = false;
    }

    

}
