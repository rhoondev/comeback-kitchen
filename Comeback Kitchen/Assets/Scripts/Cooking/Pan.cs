using System.Collections.Generic;
using UnityEngine;

public class Pan : MonoBehaviour
{
    private float _temperature = 0f;
    private List<Cookable> _contents;

    public void Heat(float amount)
    {
        _temperature += amount;
    }

    private void Update()
    {
        float heat = _temperature * Time.deltaTime;

        foreach (var item in _contents)
        {
            item.Cook(heat);
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
