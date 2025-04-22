using UnityEngine.UI;
using UnityEngine;
using System.Collections;


// Some code was taken from here -- https://youtu.be/J1ng1zA3-Pk?si=I6SN2J0TGOpc6FbE






// --------------------------------    Functions you can call for this script (Public Function)    --------------------------------

// - setUp(float barDuration, float checkmarkDuration, float warningDuration) - This function is what you call when you want to create a progress bar
// - endBar() - This function is called when you want to destroy the progress bar (maybe object is deleted)









public class TimeProgressBar : MonoBehaviour
{
    
    [SerializeField] private Image pBar;
    [SerializeField] private GameObject burningWarningImage;
    [SerializeField] private GameObject checkmarkImage;
    [SerializeField] private Color barColor;
    [SerializeField] private Color warningColor;


    private float elapsedTime;



    // These 2 variables are solely for filling up the progress bar, and keeping the bar at the same % even when paused
    private float totalElapsedBarTime;      // total time that the progress bar has been ticking for (for progress bar fill %)
    private float originalBarTotalTime;     // original time number for how long it would take the bar to fill up (if bar duration was originally set to 20, this is 20)


    private float barDuration;
    private float checkmarkDuration;
    private float warningDuration;

    private float warningFinish;



    private bool barFilled = false;
    private bool checkmarkDone = false;
    private bool burningDone = false;
    private bool notPaused = true;

    private bool hasWarningTimer = false;


    void Awake()
    {
        burningWarningImage.SetActive(false);
        checkmarkImage.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        if(notPaused)
        {
            elapsedTime += Time.deltaTime;     //set the new amount of elapsed time to the amount of time that has passed since the last frame (useful for pausing)
            totalElapsedBarTime += Time.deltaTime;
            pBar.fillAmount = totalElapsedBarTime / originalBarTotalTime;
        }
    }





    // barDuration is the time in seconds (10.0f for 10 seconds) a user wants the progress bar to take
    // checkmarkDuration is the time in seconds (1.5f for 1.5 seconds) that the checkmark will be displayed to the user
    // warningDuration is the amount of time the warning symbol will flash before the food burns (2.25f for 2.25 seconds)

    public void setUp(float barDuration, float checkmarkDuration, float warningDuration, bool hasWarningTimer)
    {
        this.hasWarningTimer = hasWarningTimer;

        pBar.color = barColor;
        // current = 0;
        this.barDuration = barDuration;
        this.checkmarkDuration = checkmarkDuration;
        this.warningDuration = warningDuration;

        originalBarTotalTime = barDuration;

        Debug.Log("Time = " + Time.time);
        // warningFinish is the time at which the warning sign will become permanent (food will be burned)
        warningFinish = (Time.time + barDuration + checkmarkDuration) + warningDuration;
        Debug.Log("Got here");


        
        // Upon being created, the coroutine barFunction will be called so that the progress bar works correctly
        Debug.Log("Coroutine Started");
        StartCoroutine(barFunction());
        Debug.Log("Coroutine Ended");


    }



    public void endBar()
    {
        Destroy(this.gameObject);
    }

    public bool isDone()
    {
        return burningDone;
    }
    


    // Function that is called when a progress bar must be paused
    // Saves the time at which the bar was paused, and saves the remaining durations that need to happen
    public void pauseBar()
    {
        StopAllCoroutines();
        notPaused = false;

        //change the durations remaining on each timer depending on how long the progress had already gone on for
        // i.e. if the bar had already reached 100%, then do not restart the bar and instead only care about the checkmark symbol or the warning sign
        // can start with if the bar is not filled since if the bar is not filled, then the checkmark timer could not possibly be filled and same with the burning timer
        if(!barFilled)
        {
            // if the bar was not filled, then save the amount of time left by subtracting the current time from the done time and putting that difference in the barDuration variable
            // time remaining (duration of next bar) =  barFinish - Time.time, so if I set the barDuration to barFinish - Time.time, I get the duration of the rest of the bar
            barDuration = barDuration - elapsedTime;
            elapsedTime = 0;        // set the elapsed time to 0 so that if the bar is quickly paused twice, we do not have double the time removed
        }
        else if(!checkmarkDone)
        {  
            // if the bar was filled, but the checkmark timer has not fully expired, then change the checkmark timer to the time remaining and make sure the bar's fill duration is 0 
            // (bar is filled)
            barDuration = 0;
            checkmarkDuration = checkmarkDuration - elapsedTime;
            elapsedTime = 0;
        }
        else if(!burningDone)
        {
            barDuration = 0;
            checkmarkDuration = 0;
            warningDuration = warningDuration - elapsedTime;;
            elapsedTime = 0;
            pBar.color = barColor;  //if it has not burned yet, then set the bar's color to green just in case the object was taken off when the warning was about being displayed
            burningWarningImage.SetActive(false);
        }
    }







    public void resumeBar()
    {
        warningFinish = (Time.time + barDuration + checkmarkDuration) + warningDuration;
        notPaused = true;
        StartCoroutine(barFunction());
    }











    IEnumerator barFunction()
    {
        //wait until the food is done cooking
        yield return new WaitForSeconds(barDuration);
        barFilled = true;
        // Debug.Log("Filling the Progress Bar Elapsed Time: " + elapsedTime);
        elapsedTime = 0;        //reset elapsed time since now elapsed time is tracking the time elapsed since the checkmark timer started

        // once the food is done cooking, display the done checkmark for the desired amount of time
        checkmarkImage.SetActive(true);
        yield return new WaitForSeconds(checkmarkDuration);
        checkmarkDone = true;
        // Debug.Log("Checkmark Elapsed Time: " + elapsedTime);
        if(hasWarningTimer)
        {
            elapsedTime = 0;        //reset elapsed time since now elapsed time is tracking the time elapsed since the warning timer started
            checkmarkImage.SetActive(false);



            // once the food is done cooking, but has still been on the burner/cooking for too long, start to warn the user that the food is about to burn
            float firstFullBlinkSpeed = 0.8f;
            float currentBlinkSpeed = firstFullBlinkSpeed;
            float minFullBlinkSpeed = 0.25f;

            // TODO - Figure out why I am not using pBar here
            // Image childImage = transform.GetChild(0).GetComponent<Image>();     // No idea why I am not using pBar here

            while(Time.time < warningFinish)
            {
                pBar.color = warningColor;
                burningWarningImage.SetActive(true);
                yield return new WaitForSeconds(currentBlinkSpeed / 2f);        //first half of blink is when warning is on
                pBar.color = barColor;
                burningWarningImage.SetActive(false);
                yield return new WaitForSeconds(currentBlinkSpeed / 2f);        //second half of blink is when warning is off

                if(currentBlinkSpeed != minFullBlinkSpeed)
                {
                    currentBlinkSpeed *= 0.75f;
                    if(currentBlinkSpeed < minFullBlinkSpeed)
                        currentBlinkSpeed = minFullBlinkSpeed;
                }
            }

            // Once the object has exceeded the burn timer, tell the user that it has burned (set progress bar color to red and display the warning image)
            burningDone = true;
            // Debug.Log("Burning Elapsed Time: " + elapsedTime);
            burningWarningImage.SetActive(true);
            pBar.color = warningColor;
        }

    }



}


