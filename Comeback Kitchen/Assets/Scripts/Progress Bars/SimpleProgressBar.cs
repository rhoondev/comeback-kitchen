using UnityEngine.UI;
using UnityEngine;

public class SimpleProgressBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    public void SetValue(float value)
    {
        fillImage.fillAmount = value;
    }
}
