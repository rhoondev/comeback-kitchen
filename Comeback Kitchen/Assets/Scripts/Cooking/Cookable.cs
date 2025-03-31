using UnityEngine;

public class Cookable : MonoBehaviour
{
    [SerializeField] private float cookingMultiplier;
    [SerializeField] private float coolingRate;
    [SerializeField] private float cookedTemperature;
    [SerializeField] private float burnedTemperature;

    private float _temperature = 0f;
    private bool _cooked = false;
    private bool _burned = false;

    public void Cook(float amount)
    {
        _temperature += amount * cookingMultiplier;

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

    private void Update()
    {
        _temperature -= coolingRate * Time.deltaTime;

        if (_temperature < 0)
        {
            _temperature = 0;
        }
    }
}
