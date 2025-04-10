using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class GameManager : MonoBehaviour
{

    
    [SerializeField] private GameObject horizontalBarPrefab;
    [SerializeField] private GameObject circleBarPrefab;
    [SerializeField] private GameObject worldSpotTest;




    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
        
    // }

    // Update is called once per frame
    void Update()
    {
        // Code used to debug the intializeBar method and ProgressBar class
        if(Keyboard.current.eKey.wasPressedThisFrame)
        {
            StartCoroutine(testMethod());
        }
    }



    // called when a bar is created (sets values up for the bar and also lets the bar update method be called)

    // Returns - a gameobject, which is the bar, so he bar can be deleted by the invoking function when necessary

    // completionTime is time in seconds from start until the bar is full
    // checkTime is the time in seconds that the checkmark will be active (showing the player that the cooking is done)
    // warningTime is the time in seconds that the burn warning image will be active and flashing before the warning becomes permanent
    // worldPosition is a gameobject which the bar becomes a child under (so position is translated)
    // isHorizontalBar is a boolean value for if the progress bar is horizontal or a circle
    // hasWarningTmer is a boolean value for if the progress bar has a warning image (for burning and stuff) or just ends with the checkmark (washing food)
    GameObject intitializeProgressBar(float completionTime, float checkTime, float warningTime, GameObject worldPosition, bool isHorizontalBar, bool hasWarningTimer)
    {
        GameObject bar;
        // Debug.Log("This worked");
        if(isHorizontalBar) 
            bar = Instantiate(horizontalBarPrefab, worldPosition.transform);
        else
            bar = Instantiate(circleBarPrefab, worldPosition.transform);

        // Debug.Log("Here above setting up bar");
        bar.GetComponent<ProgressBar>().setUp(completionTime, checkTime, warningTime, hasWarningTimer);
        return bar;
    }


    IEnumerator testMethod()
    {
        Debug.Log("Got Here - testMethod");
        GameObject currBar;
        // Circle with no warnings
        currBar = intitializeProgressBar(5.0f, 2.0f, 3.0f, worldSpotTest, false, false);
        yield return new WaitForSeconds(11);
        currBar.GetComponent<ProgressBar>().endBar();

        // Horizontal Bar with no warnings
        currBar = intitializeProgressBar(5.0f, 2.0f, 3.0f, worldSpotTest, true, false);
        yield return new WaitForSeconds(11);
        currBar.GetComponent<ProgressBar>().endBar();

        // Circle with Warning
        currBar = intitializeProgressBar(5.0f, 2.0f, 3.0f, worldSpotTest, false, true);
        yield return new WaitForSeconds(11);
        currBar.GetComponent<ProgressBar>().endBar();

        // Bar with no Warnings
        currBar = intitializeProgressBar(5.0f, 2.0f, 3.0f, worldSpotTest, true, true);
        yield return new WaitForSeconds(11);
        currBar.GetComponent<ProgressBar>().endBar();
    }




}
