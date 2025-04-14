using System.Collections.Generic;
using UnityEngine;

public class VegetableBasket : MonoBehaviour
{
    [SerializeField] private GameObject tomato;
    [SerializeField] private GameObject bellPepper;
    [SerializeField] private GameObject onion;

    private Dictionary<string, GameObject> _vegetableDictionary;
    private GameObject _targetVegetable;

    private void Start()
    {
        _vegetableDictionary = new Dictionary<string, GameObject>
        {
            { "Tomato", tomato },
            { "Bell Pepper", bellPepper },
            { "Onion", onion }
        };
    }

    public void SetTargetVegetable(string vegetableName)
    {
        _targetVegetable = _vegetableDictionary[vegetableName];
        Debug.Log($"Target vegetable set to: {vegetableName}");
    }

    public GameObject GrabVegetable(GameObject vegetable)
    {
        if (_vegetableDictionary.ContainsValue(vegetable))
        {
            if (vegetable == _targetVegetable)
            {
                GameObject vegetableCopy = Instantiate(vegetable, vegetable.transform.position, vegetable.transform.rotation);
                // TODO: Place the vegetable copy into the player's hand
                Debug.Log($"Correct vegetable grabbed: {vegetable.name}");
                return vegetableCopy;
            }
            else
            {
                Debug.LogError($"Incorrect vegetable grabbed: {vegetable.name}. Expected: {_targetVegetable.name}");
            }
        }
        else
        {
            Debug.LogError("Attempted to grab the an object which is not recognized by the vegetable basket.");
        }

        return null;
    }
}
