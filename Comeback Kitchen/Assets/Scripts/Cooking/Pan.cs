using System.Collections.Generic;
using UnityEngine;

public class Pan : MonoBehaviour
{
    private float temperature;
    private List<Cookable> contents;

    public void Heat(float amount)
    {
        temperature += amount;
    }

    private void Update()
    {
        float heat = temperature * Time.deltaTime;

        foreach (var item in contents)
        {
            item.Cook(heat);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Cookable>(out var cookable))
        {
            contents.Add(cookable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Cookable>(out var cookable))
        {
            contents.Remove(cookable);
        }
    }
}
