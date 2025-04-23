using UnityEngine.UI;
using UnityEngine;
using System.Collections;







// --------------------------------    Functions you can call for this script (Public Function)    --------------------------------

// - setUp(float barDuration, float checkmarkDuration, float warningDuration) - This function is what you call when you want to create a progress bar
// - endBar() - This function is called when you want to destroy the progress bar (maybe object is deleted)






public class ScoreProgressBar : MonoBehaviour
{
    
    [SerializeField] private Image pBar;
    [SerializeField] private GameObject burningWarningImage;
    [SerializeField] private GameObject checkmarkImage;
    [SerializeField] private Color barColor;
    [SerializeField] private Color warningColor;



    private float currentScore;
    private float fullBarNum = 0;
    private float checkmarkCompleteAmount;
    private float warningCompleteAmount;



    private bool hasWarningTimer;



    void Awake()
    {
        burningWarningImage.SetActive(false);
        checkmarkImage.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        pBar.fillAmount = currentScore / fullBarNum;
    }




    // This method allows you to add a number to the current score. The score will then be checked to see if it passed any threshold for progress bar progress displays
    public void addToScore(float addingToScore)
    {
        currentScore += addingToScore;

        // if it has a warning timer, then we want the flashing and burning of warning timer as well as the usual checkmark upon completion
        if(hasWarningTimer)
        {
            if(currentScore >= warningCompleteAmount)
            {
                // make i look burned
                pBar.color = warningColor;
                burningWarningImage.SetActive(true);
            }
            else if(currentScore >= checkmarkCompleteAmount)
            {
                // start the warning steps

                // if checkmark is up or last warning just happened, then stop the coroutine
                StopAllCoroutines();
                checkmarkImage.SetActive(false);

                // Flashes the warning sign for 0.5 seconds
                StartCoroutine(flashWarning(0.5f));

            }
            else if(currentScore >= fullBarNum)
            {
                StartCoroutine(activateCheck(2.0f));    // Check is activate for 2 seconds
            }
        }
        else        //if it does not have a warning timer, then we do not want any warning flashes or burning image and instead just want the completion checkmark
        {
            
            if(currentScore >= fullBarNum)
            {
                StartCoroutine(activateCheck(2.0f));    // Check is activate for 2 seconds
            }
        }
    }

    



    /*
     This function is designed to create a score progress bar. Unlike the time progress bars, the score progress bars are not put in 
     terms of time but rather a score. This causes the bars to periodically have their score increased, rather than a continual 
     increase like the time bars have.


     fullBarNum is the score a user needs to get to for the bar to be full (any number less will only fill up a portion of the bar)
     checkmarkAmount is the additional score a user needs to get to for the checkmark to disappear
     warningAmount is the additional score a user needs to get to for the warningMarker to stay (food is burnt or something)
     hasWarningTimer is a bool which determines if the bar needs a warning timer


     Example call for a circular bar which will be a full bar at 1500, that has a checkmark disappearing at 1700, and a warning that 
     becomes permanent at 2000 - setUp(1500, 200, 300, false, true)
    */

    public void setUp(float fullBarNum, float checkmarkCompleteAmount, float warningCompleteAmount, bool hasWarningTimer)
    {
        this.hasWarningTimer = hasWarningTimer;

        this.fullBarNum = fullBarNum;
        this.checkmarkCompleteAmount = checkmarkCompleteAmount + fullBarNum;
        this.warningCompleteAmount = warningCompleteAmount + this.checkmarkCompleteAmount;

        pBar.color = barColor;
        currentScore = 0;
    }



    public void endBar()
    {
        Destroy(this.gameObject);
    }

    public bool isDone()
    {
        return currentScore > fullBarNum;
    }





    IEnumerator activateCheck(float timeDesired)
    {
        checkmarkImage.SetActive(true);
        yield return new WaitForSeconds(timeDesired);
        checkmarkImage.SetActive(false);
    }

    IEnumerator flashWarning(float timeDesired)
    {
        burningWarningImage.SetActive(true);
        pBar.color = warningColor;
        yield return new WaitForSeconds(timeDesired);
        burningWarningImage.SetActive(false);
        pBar.color = barColor;
    }
}


