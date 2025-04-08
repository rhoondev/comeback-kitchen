using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pan : MonoBehaviour
{
    [SerializeField] private Flame activeFlame;
    [SerializeField] private PanLiquid panLiquid;
    [SerializeField] private float maxTemperature;
    [SerializeField] private float thermalConductivity;

    private float _temperature = 70f;
    private List<Cookable> _contents;

    private void Awake()
    {
        _contents = new List<Cookable>();
    }

    private void Update()
    {
        float ambient = maxTemperature * activeFlame.Size;
        float rate = thermalConductivity * (ambient - _temperature);
        _temperature += rate * Time.deltaTime;

        foreach (var item in _contents)
        {
            item.Cook(_temperature);
        }

        panLiquid.Heat(_temperature);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Cookable>(out var cookable))
        {
            _contents.Add(cookable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Cookable>(out var cookable))
        {
            _contents.Remove(cookable);
        }
    }
}
