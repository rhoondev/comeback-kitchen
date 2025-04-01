using UnityEngine;

public class PanLiquid : MonoBehaviour
{
    [SerializeField] private float thermalConductivity;
    [SerializeField] private float boilingPoint;

    public bool IsBoiling => _temperature >= boilingPoint;

    private float _temperature = 0f;

    public void Heat(float panTemp)
    {
        float rate = thermalConductivity * (panTemp - _temperature);
        _temperature += rate * Time.deltaTime;

        if (IsBoiling)
        {
            Debug.Log($"{gameObject.name} is boiling!");
        }
    }
}
