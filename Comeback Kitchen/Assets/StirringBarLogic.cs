using UnityEngine;
using UnityEngine.UI;

public class StirringBarLogic : MonoBehaviour
{
    [Header("Zone References")]
    public RectTransform yellowZone;
    public RectTransform greenZone;
    public RectTransform redZone;
    public RectTransform ticker;
    public RectTransform bar; // The full bar container




    [Header("Ticker Movement Settings")]
    private float moveSpeed;
    private float userMovesBarSpeed;
    private float requiredGreenTime;
    private float redFailureTime;

    private float greenStartX;
    private float greenEndX;

    private float timeInGreen = 0f;
    private float timeInRed = 0f;

    private float barWidth;

    public bool success = false;
    public bool failed = false;



    private float distanceToMove;



    // If ticker does not start in green, then it will start in the yellow (far left)
    /*
    * float yellow - stores the ratio for how large the yellow section of the progress bar will be
    * float green - stores the ratio for how large the green section of the progress bar will be
    * float red - stores the ratio for how large the red section of the progress bar will be
    * float moveSpeed - determines how fast the bar will move to the right when something is not happening (stirring or action)
    * float userMovesBarSpeed - determines how far the bar will move to the left when an action occurs (stirring or action)
    * float requiredGreenTime - stores how much time in the green area is needed before the bar is considered succeeded (and finished)
    * float redFailureTime - stores how much time in the red area must pass before the bar is considered failed (and finished)
    * bool startsInGreen - if positive, has the ticker start in the middle of the green, and if false has the ticker start in the middle of the yellow of the bar

    This function creates a bar according to the specifications provided
    */
    public void SetUp(float yellow, float green, float red, float moveSpeed, float userMovesBarSpeed, float requiredGreenTime, float redFailureTime, bool startsInGreen)
    {
        this.moveSpeed = moveSpeed;
        this.userMovesBarSpeed = userMovesBarSpeed;
        this.requiredGreenTime = requiredGreenTime;
        this.redFailureTime = redFailureTime;



        float total = yellow + green + red;
        if (total <= 0)
        {
            Debug.LogError("Zone sizes must be positive and non-zero.");
            return;
        }

        float yellowNorm = yellow / total;
        float greenNorm = green / total;
        float redNorm = red / total;

        barWidth = bar.rect.width;

        float yellowWidth = barWidth * yellowNorm;
        float greenWidth = barWidth * greenNorm;
        float redWidth = barWidth * redNorm;

        // Apply zone widths
        yellowZone.sizeDelta = new Vector2(yellowWidth, yellowZone.sizeDelta.y);
        greenZone.sizeDelta = new Vector2(greenWidth, greenZone.sizeDelta.y);
        redZone.sizeDelta = new Vector2(redWidth, redZone.sizeDelta.y);

        // Position them left to right
        yellowZone.anchoredPosition = new Vector2(0, 0);
        greenZone.anchoredPosition = new Vector2(yellowWidth, 0);
        redZone.anchoredPosition = new Vector2(yellowWidth + greenWidth, 0);

        // Cache green zone range for checking
        greenStartX = yellowWidth;
        greenEndX = yellowWidth + greenWidth;

        // Reset ticker to center of green
        Vector2 tickerPos = ticker.anchoredPosition;
        if(startsInGreen)
            tickerPos.x = (greenStartX + greenEndX) / 2;
        else
            tickerPos.x = 0;
        ticker.anchoredPosition = tickerPos;
    }





    // This function handles the movement of the ticker and checking if the bar should still beworking
    private void Update()
    {
        if (success || failed || barWidth <= 0) return;

        float delta = moveSpeed * Time.deltaTime;

        delta -= distanceToMove;    // add the amount the bar is supposed to move
        distanceToMove = 0;         // reset the amount that the bar is supposed to move (move amount is global so must reset manually)

        // Move ticker
        Vector2 tickerPos = ticker.anchoredPosition;
        tickerPos.x += delta;
        tickerPos.x = Mathf.Clamp(tickerPos.x, 0, barWidth);
        ticker.anchoredPosition = tickerPos;

        // Check green zone
        if (tickerPos.x >= greenStartX && tickerPos.x <= greenEndX)
        {
            timeInGreen += Time.deltaTime;
            if (timeInGreen >= requiredGreenTime)
            {
                success = true;
                Debug.Log("✅ Success! Stayed in green long enough.");
                // Trigger success event here
            }
        }
        else if(tickerPos.x < greenStartX)
        {
            // In yellow area so do something
            // TODO -- trigger failure based on which bad area the ticker is on the bar
        }
        else
        {
            //In red area so fail after X amount of time
            timeInRed += Time.deltaTime;
            if(timeInRed >= redFailureTime)
            {
                failed = true;
                Debug.Log("❌ Failed! Spent too much time in the red.");
            }
        }

        // Keep bar on edges if it reaches the edges (do not go out of bounds)
        if(tickerPos.x < 0)
            tickerPos.x = 0;
        else if(tickerPos.x > barWidth)
            tickerPos.x = barWidth;
    }



    public void MoveTickerToLeft()
    {
        distanceToMove = userMovesBarSpeed * Time.deltaTime;
    }




    public void DestroyBar()
    {
        Destroy(this);
    }


    public bool IsDone()
    {
        return failed || success;
    }
}
