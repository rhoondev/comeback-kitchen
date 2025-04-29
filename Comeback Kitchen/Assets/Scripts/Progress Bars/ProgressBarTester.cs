// using UnityEngine;
// using UnityEngine.InputSystem;
// using System.Collections;
// using Unity.XR.CoreUtils;

// public class ProgressBarTester : MonoBehaviour
// {


//     [SerializeField] private GameObject horizontalBarPrefab;
//     [SerializeField] private GameObject circleBarPrefab;
//     [SerializeField] private GameObject scoreBarHorizontalPrefab;
//     [SerializeField] private GameObject scoreBarCirclePrefab;
//     [SerializeField] private GameObject tickerBarPrefab;
//     [SerializeField] private GameObject worldSpotTest;
//     float timeEnd;

//     GameObject currBar;
//     StirringBarLogic currStirBar;
//     bool keyboardWasPressed;


//     // // Start is called once before the first execution of Update after the MonoBehaviour is created
//     void Start()
//     {
//         timeEnd = Time.time + 15f;
//         keyboardWasPressed = false;
//         currStirBar = RunTickerTest().transform.GetChild(0).GetComponent<StirringBarLogic>();
//         currStirBar.SetUp(200f, 100f, 200f, 40f, 400f, 10f, 15f, false);
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         // Code used to debug the intializeBar method and ProgressBar class
//         if(Keyboard.current.eKey.wasPressedThisFrame)
//         {
//             currStirBar.MoveTickerToLeft();
//         }

//         Debug.Log("Done? - " + currStirBar.IsDone());
//         if(currStirBar.IsDone())
//             currStirBar.DestroyBar();



//         // if(Keyboard.current.eKey.wasPressedThisFrame)
//         // {
//         //     // Testing out the time Progress Bars
//         //     StartCoroutine(testMethod());






//         //     // Testing out the Score Progress Bars
//         //     currBar = intitializeScoreProgressBar(1500, 200, 300, worldSpotTest, false, true);
//         //     Debug.Log("Current Bar: " + currBar);
//         //     keyboardWasPressed = true;
//         // }

//         // if(keyboardWasPressed)
//         // {
//         //     currBar.GetComponent<ScoreProgressBar>().addToScore(20f);
//         // }
//     }



//     // called when a bar is created (sets values up for the bar and also lets the bar update method be called)

//     // Returns - a gameobject, which is the bar, so he bar can be deleted by the invoking function when necessary

//     // completionTime is time in seconds from start until the bar is full
//     // checkTime is the time in seconds that the checkmark will be active (showing the player that the cooking is done)
//     // warningTime is the time in seconds that the burn warning image will be active and flashing before the warning becomes permanent
//     // worldPosition is a gameobject which the bar becomes a child under (so position is translated)
//     // isHorizontalBar is a boolean value for if the progress bar is horizontal or a circle
//     // hasWarningTmer is a boolean value for if the progress bar has a warning image (for burning and stuff) or just ends with the checkmark (washing food)
//     GameObject intitializeTimeProgressBar(float completionTime, float checkmarkCompleteAmount, float warningCompleteAmount, GameObject worldPosition, bool isHorizontalBar, bool hasWarningTimer)
//     {
//         GameObject bar;
//         // Debug.Log("This worked");
//         if(isHorizontalBar) 
//             bar = Instantiate(horizontalBarPrefab, worldPosition.transform);
//         else
//             bar = Instantiate(circleBarPrefab, worldPosition.transform);

//         // Debug.Log("Here above setting up bar");
//         bar.GetComponent<TimeProgressBar>().setUp(completionTime, checkmarkCompleteAmount, warningCompleteAmount, hasWarningTimer);
//         return bar;
//     }



//     GameObject intitializeScoreProgressBar(float fullBarNum, float checkTime, float warningTime, GameObject worldPosition, bool isHorizontalBar, bool hasWarningTimer)
//     {
//         GameObject bar;
//         // Debug.Log("This worked");
//         if(isHorizontalBar) 
//             bar = Instantiate(scoreBarHorizontalPrefab, worldPosition.transform);
//         else
//             bar = Instantiate(scoreBarCirclePrefab, worldPosition.transform);

//         // Debug.Log("Here above setting up bar");
//         bar.GetComponent<ScoreProgressBar>().setUp(fullBarNum, checkTime, warningTime, hasWarningTimer);
//         return bar;
//     }






//     IEnumerator testMethod()
//     {
//         Debug.Log("Got Here - testMethod");
//         GameObject currBar;

//         // Circle with no warnings
//         currBar = intitializeTimeProgressBar(5.0f, 2.0f, 3.0f, worldSpotTest, false, false);
//         yield return new WaitForSeconds(11);
//         currBar.GetComponent<TimeProgressBar>().endBar();

//         // Horizontal Bar with no warnings
//         currBar = intitializeTimeProgressBar(5.0f, 2.0f, 3.0f, worldSpotTest, true, false);
//         yield return new WaitForSeconds(11);
//         currBar.GetComponent<TimeProgressBar>().endBar();

//         // Circle with Warning
//         currBar = intitializeTimeProgressBar(5.0f, 2.0f, 3.0f, worldSpotTest, false, true);
//         yield return new WaitForSeconds(11);
//         currBar.GetComponent<TimeProgressBar>().endBar();

//         // Bar with no Warnings
//         currBar = intitializeTimeProgressBar(5.0f, 2.0f, 3.0f, worldSpotTest, true, true);
//         yield return new WaitForSeconds(11);
//         currBar.GetComponent<TimeProgressBar>().endBar();
//     }





//     GameObject RunTickerTest()
//     {
//         GameObject testBar = Instantiate(tickerBarPrefab);
//         return testBar;
//     }


// }
