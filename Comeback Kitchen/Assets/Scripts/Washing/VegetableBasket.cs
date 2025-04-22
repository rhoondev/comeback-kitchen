using System.Collections.Generic;
using UnityEngine;

public class VegetableBasket : MonoBehaviour
{
    [field: SerializeField] public GameObject tomato { get; private set; }
    [field: SerializeField] public GameObject bellPepper { get; private set; }
    [field: SerializeField] public GameObject onion { get; private set; }

    public SmartAction<GameObject> OnVegetableGrabbed = new SmartAction<GameObject>();

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

                // TODO: Place the vegetable copy into the player's hand in VR

                Debug.Log($"Correct vegetable grabbed: {vegetable.name}");
                OnVegetableGrabbed.Invoke(vegetableCopy);
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
