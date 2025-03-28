using System.Collections.Generic;
using UnityEngine;

public class RiceContainer : MonoBehaviour
{
    private List<GameObject> _grains;

    private void Awake()
    {
        _grains = new List<GameObject>();
    }

    public void AddGrain(GameObject grain)
    {
        _grains.Add(grain);
    }

    public GameObject RemoveGrain()
    {
        if (_grains.Count == 0)
        {
            return null;
        }

        GameObject grain = _grains[^1];
        _grains.RemoveAt(_grains.Count - 1);
        return grain;
    }

    public IEnumerable<GameObject> GetRiceGrains()
    {
        return _grains.AsReadOnly();
    }
}
