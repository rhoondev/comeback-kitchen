using UnityEngine;

public class Cookable : MonoBehaviour
{
    [SerializeField] private float thermalConductivity;
    [SerializeField] private float cookedTemperature;
    [SerializeField] private float burnedTemperature;

    private float _temperature = 0f;
    private bool _cooked = false;
    private bool _burned = false;

    public void Cook(float panTemp)
    {
        float rate = thermalConductivity * (panTemp - _temperature);
        _temperature += rate * Time.deltaTime;

        if (!_cooked && _temperature >= cookedTemperature)
        {
            Debug.Log($"{gameObject.name} is cooked!");
            _cooked = true;
        }
        else if (!_burned && _temperature >= burnedTemperature)
        {
            Debug.Log($"{gameObject.name} is burned!");
            _burned = true;
        }
    }
}
