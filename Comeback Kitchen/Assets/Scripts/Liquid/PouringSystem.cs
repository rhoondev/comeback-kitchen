using UnityEngine;

public class PouringSystem : MonoBehaviour
{
    [SerializeField] private PanLiquid panLiquid;
    [SerializeField] private MulticolorProgressBar pouringBar;

    public SmartAction OnPouringComplete = new SmartAction();
    public SmartAction OnPouringFailed = new SmartAction();

    private int _amountPoured;
    private float _lastTimePoured;

    private void Awake()
    {
        panLiquid.OnLiquidAdded.Add(OnLiquidAdded);
        pouringBar.OnEnterRed.Add(PouringFailed);
    }

    public void StartPouring(int targetAmount, int acceptableVariation)
    {
        pouringBar.gameObject.SetActive(true);
        pouringBar.Configure(0, targetAmount - acceptableVariation, targetAmount + acceptableVariation, targetAmount * 2, 0);
        _amountPoured = 0;
    }

    private void Update()
    {
        if (Time.time - _lastTimePoured > 2f && pouringBar.GetState() == ProgressBarState.Green)
        {
            Debug.Log("Pouring complete!");

            pouringBar.gameObject.SetActive(false);

            OnPouringComplete.Invoke();
        }
    }

    private void OnLiquidAdded(LiquidType type, int amount)
    {
        _amountPoured += amount;
        pouringBar.SetValue(_amountPoured);
        _lastTimePoured = Time.time;
    }

    private void PouringFailed()
    {
        Debug.Log("Pouring bar entered the red zone! Pouring failed!");

        pouringBar.gameObject.SetActive(false);

        OnPouringFailed.Invoke();
    }
}


