using System.Collections.Generic;
using UnityEngine;

public enum VegetableType
{
    Tomato,
    BellPepper,
    Onion
}

public class VegetableBasket : MonoBehaviour
{
    [SerializeField] private InfiniteGrabSpawner tomatoSpawner;
    [SerializeField] private InfiniteGrabSpawner bellPepperSpawner;
    [SerializeField] private InfiniteGrabSpawner onionSpawner;
    // [SerializeField] private InfiniteGrabSpawner distraction1;
    // [SerializeField] private InfiniteGrabSpawner distraction2;
    // [SerializeField] private InfiniteGrabSpawner distraction3;

    public SmartAction<GameObject> OnVegetableGrabbed = new SmartAction<GameObject>();
    public SmartAction OnVegetableGrabAttempt = new SmartAction();

    private Dictionary<VegetableType, InfiniteGrabSpawner> _vegetableDictionary;
    private InfiniteGrabSpawner _activeVegetableSpawner;

    private void Start()
    {
        _vegetableDictionary = new Dictionary<VegetableType, InfiniteGrabSpawner>
        {
            { VegetableType.Tomato, tomatoSpawner },
            { VegetableType.BellPepper, bellPepperSpawner },
            { VegetableType.Onion, onionSpawner }
        };

        tomatoSpawner.OnGrabbed.Add(VegetableGrabbed);
        bellPepperSpawner.OnGrabbed.Add(VegetableGrabbed);
        onionSpawner.OnGrabbed.Add(VegetableGrabbed);

        tomatoSpawner.OnGrabAttempt.Add(VegetableGrabAttempt);
        bellPepperSpawner.OnGrabAttempt.Add(VegetableGrabAttempt);
        onionSpawner.OnGrabAttempt.Add(VegetableGrabAttempt);

        // distraction1.OnGrabAttempt.Add(VegetableGrabAttempt);
        // distraction2.OnGrabAttempt.Add(VegetableGrabAttempt);
        // distraction3.OnGrabAttempt.Add(VegetableGrabAttempt);
    }

    public void SetActiveVegetableType(VegetableType vegetableType)
    {
        if (_activeVegetableSpawner != null)
        {
            _activeVegetableSpawner.IsGrabbable = false; // Disable the current spawner
        }

        _activeVegetableSpawner = _vegetableDictionary[vegetableType]; // Set the new spawner
        _activeVegetableSpawner.IsGrabbable = true; // Enable the new spawner

        Debug.Log($"Target vegetable type set to: {vegetableType}");
    }

    private void VegetableGrabbed(GameObject vegetable)
    {
        Debug.Log("Vegetable grabbed successfully.");
        OnVegetableGrabbed.Invoke(vegetable);
    }

    private void VegetableGrabAttempt()
    {
        Debug.Log("Attempted to grab a vegetable, but the grab is not allowed.");
        OnVegetableGrabAttempt.Invoke();
    }
}
