using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pan : MonoBehaviour
{
    [SerializeField] private Flame activeFlame;
    [SerializeField] private float maxTemperature;
    [SerializeField] private float thermalConductivity;

    private float _temperature = 0f;
    private List<Cookable> _contents;

    private void Awake()
    {
        _contents = new List<Cookable>();
        StartCoroutine(PrintTempRoutine());
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

    private IEnumerator PrintTempRoutine()
    {
        while (true)
        {
            Debug.Log($"Pan temperature: {_temperature}");
            yield return new WaitForSeconds(1f);
        }
    }
}
