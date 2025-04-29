using System;
using UnityEngine;

public enum ProgressBarState
{
    Yellow,
    Green,
    Red
}

public class MulticolorProgressBar : MonoBehaviour
{
    [SerializeField] private RectTransform yellowZone;
    [SerializeField] private RectTransform greenZone;
    [SerializeField] private RectTransform redZone;
    [SerializeField] private RectTransform marker; // The marker that moves across the bar

    private float _yellowValue;
    private float _greenValue;
    private float _redValue;
    private float _maxValue;
    private float _startValue;

    public SmartAction OnEnterRed = new SmartAction();

    private float _barWidth;

    private float _markerValue = 0f;
    private bool _hasEnteredRedZone = false;

    private void Awake()
    {
        _barWidth = GetComponent<RectTransform>().rect.width;
    }

    public void Configure(float yellowValue, float greenValue, float redValue, float maxValue, float startValue)
    {
        _yellowValue = yellowValue;
        _greenValue = greenValue;
        _redValue = redValue;
        _maxValue = maxValue;
        _startValue = startValue;
        _hasEnteredRedZone = false;

        float yellowWidth = (greenValue - yellowValue) / maxValue * _barWidth;
        float greenWidth = (redValue - yellowValue) / maxValue * _barWidth;

        // Apply zone widths
        yellowZone.sizeDelta = new Vector2(yellowWidth, yellowZone.sizeDelta.y);
        greenZone.sizeDelta = new Vector2(greenWidth, greenZone.sizeDelta.y);

        SetValue(startValue);
    }

    public void Reset()
    {
        SetValue(_startValue);
        _hasEnteredRedZone = false;
    }

    public void SetValue(float value)
    {
        _markerValue = Mathf.Clamp(value, 0f, _maxValue);
        marker.anchoredPosition = new Vector2(_markerValue / _maxValue * _barWidth, 0f);

        if (_markerValue >= _redValue && !_hasEnteredRedZone)
        {
            _hasEnteredRedZone = true;
            OnEnterRed.Invoke();
            Debug.Log("Entered red zone!");
        }
    }

    public ProgressBarState GetState()
    {
        if (_markerValue >= _yellowValue && _markerValue < _greenValue)
        {
            return ProgressBarState.Yellow;
        }
        else if (_markerValue >= _greenValue && _markerValue < _redValue)
        {
            return ProgressBarState.Green;
        }
        else if (_markerValue >= _redValue && _markerValue <= _maxValue)
        {
            return ProgressBarState.Red;
        }
        else
        {
            throw new Exception("Invalid marker value: " + _markerValue);
        }
    }

    // private void Update()
    // {
    //     if (success || failed || _barWidth <= 0) return;

    //     float delta = moveSpeed * Time.deltaTime;

    //     delta -= distanceToMove;    // add the amount the bar is supposed to move
    //     distanceToMove = 0;         // reset the amount that the bar is supposed to move (move amount is global so must reset manually)

    //     // Move ticker
    //     Vector2 tickerPos = marker.anchoredPosition;
    //     tickerPos.x += delta;
    //     tickerPos.x = Mathf.Clamp(tickerPos.x, 0, _barWidth);
    //     marker.anchoredPosition = tickerPos;

    //     // Check green zone
    //     if (tickerPos.x >= _greenStartX && tickerPos.x <= _greenEndX)
    //     {
    //         timeInGreen += Time.deltaTime;
    //         if (timeInGreen >= requiredGreenTime)
    //         {
    //             success = true;
    //             Debug.Log("✅ Success! Stayed in green long enough.");
    //             // Trigger success event here
    //         }
    //     }
    //     else if (tickerPos.x < _greenStartX)
    //     {
    //         // In yellow area so do something
    //         // TODO -- trigger failure based on which bad area the ticker is on the bar
    //     }
    //     else
    //     {
    //         //In red area so fail after X amount of time
    //         timeInRed += Time.deltaTime;
    //         if (timeInRed >= redFailureTime)
    //         {
    //             failed = true;
    //             Debug.Log("❌ Failed! Spent too much time in the red.");
    //         }
    //     }

    //     // Keep bar on edges if it reaches the edges (do not go out of bounds)
    //     if (tickerPos.x < 0)
    //         tickerPos.x = 0;
    //     else if (tickerPos.x > _barWidth)
    //         tickerPos.x = _barWidth;
    // }

    // public void MoveTickerToLeft()
    // {
    //     distanceToMove = userMovesBarSpeed * Time.deltaTime;
    // }
}
