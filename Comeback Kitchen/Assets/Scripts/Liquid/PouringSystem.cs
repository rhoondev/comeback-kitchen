using UnityEngine;

public class PouringSystem : MonoBehaviour
{
    [SerializeField] private PanLiquid panLiquid;
    [SerializeField] private MulticolorProgressBar pouringBar;

    public SmartAction OnPouringComplete = new SmartAction();
    public SmartAction OnPouringFailed = new SmartAction();

    private int _amountPoured;
    private float _lastTimePoured;
    private bool _pouringComplete;

    private void Awake()
    {
        panLiquid.OnLiquidAdded.Add(OnLiquidAdded);
        pouringBar.OnEnterRed.Add(PouringFailed);
    }

    public void StartPouring(int targetAmount, int acceptableVariation)
    {
        // Configure the pouring bar without showing it yet
        pouringBar.gameObject.SetActive(true);
        pouringBar.Configure(0, targetAmount - acceptableVariation, targetAmount + acceptableVariation, targetAmount * 2, 0);
        pouringBar.gameObject.SetActive(false);

        // Reset the state
        _amountPoured = 0;
        _pouringComplete = false;

        // Respond to pouring events
        panLiquid.OnLiquidAdded.Add(OnLiquidAdded);
        pouringBar.OnEnterRed.Add(PouringFailed);
    }

    private void Update()
    {
        if (!_pouringComplete && pouringBar.GetState() == ProgressBarState.Green && Time.time - _lastTimePoured > 2f)
        {
            _pouringComplete = true;

            Debug.Log("Pouring complete!");

            panLiquid.OnLiquidAdded.Clear();

            pouringBar.OnEnterRed.Clear();
            pouringBar.gameObject.SetActive(false);

            OnPouringComplete.Invoke();
        }
    }

    private void OnLiquidAdded(LiquidType type, int amount)
    {
        if (!pouringBar.gameObject.activeSelf)
        {
            pouringBar.gameObject.SetActive(true);
        }

        _amountPoured += amount;
        pouringBar.SetValue(_amountPoured);

        _lastTimePoured = Time.time;
    }

    private void PouringFailed()
    {
        Debug.Log("Pouring bar entered the red zone! Pouring failed!");

        panLiquid.OnLiquidAdded.Clear();
        pouringBar.OnEnterRed.Clear();

        OnPouringFailed.Invoke();
    }
}


