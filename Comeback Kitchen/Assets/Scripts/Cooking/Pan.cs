using System.Collections.Generic;
using UnityEngine;

public class Pan : MonoBehaviour
{
    [SerializeField] private Flame activeFlame;

    private float _temperature = 0f;
    private List<Cookable> _contents;

    private void Update()
    {
        _temperature = Mathf.Lerp(_temperature, activeFlame.Temperature, Time.deltaTime * 0.1f);

        float cookAmount = _temperature * Time.deltaTime;

        foreach (var item in _contents)
        {
            item.Cook(cookAmount);
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
}
