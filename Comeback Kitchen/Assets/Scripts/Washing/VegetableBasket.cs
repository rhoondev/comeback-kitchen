using System.Collections.Generic;
using UnityEngine;

public enum VegetableType
{
    Tomato,
    BellPepper,
    Onion
}

// TODO: Add distraction vegetables
public class VegetableBasket : MonoBehaviour
{
    [SerializeField] private InfiniteGrabSpawner tomatoSpawner;
    [SerializeField] private InfiniteGrabSpawner bellPepperSpawner;
    [SerializeField] private InfiniteGrabSpawner onionSpawner;
    // [SerializeField] private InfiniteGrabSpawner distraction1;
    // [SerializeField] private InfiniteGrabSpawner distraction2;
    // [SerializeField] private InfiniteGrabSpawner distraction3;

    public SmartAction<DynamicObject> OnVegetableGrabbed = new SmartAction<DynamicObject>();
    public SmartAction<DynamicObject> OnVegetableGrabAttempt = new SmartAction<DynamicObject>();

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

    public void SetActiveVegetableType(VegetableType? vegetableType)
    {
        if (_activeVegetableSpawner != null)
        {
            _activeVegetableSpawner.IsGrabbable = false; // Disable the current spawner
        }

        if (vegetableType != null)
        {
            _activeVegetableSpawner = _vegetableDictionary[(VegetableType)vegetableType]; // Set the new spawner
            _activeVegetableSpawner.IsGrabbable = true; // Enable the new spawner

            Debug.Log($"Activated spawner for: {vegetableType}.");
        }
        else
        {
            _activeVegetableSpawner = null; // No spawner is active
            Debug.Log("Deactivated all spawners.");
        }
    }

    public void Unlock()
    {
        tomatoSpawner.GetComponent<InteractionLocker>().UnlockInteraction();
        bellPepperSpawner.GetComponent<InteractionLocker>().UnlockInteraction();
        onionSpawner.GetComponent<InteractionLocker>().UnlockInteraction();
    }

    public void Lock()
    {
        tomatoSpawner.GetComponent<InteractionLocker>().LockInteraction();
        bellPepperSpawner.GetComponent<InteractionLocker>().LockInteraction();
        onionSpawner.GetComponent<InteractionLocker>().LockInteraction();
    }

    private void VegetableGrabbed(DynamicObject vegetable)
    {
        Debug.Log("Vegetable grabbed successfully.");
        OnVegetableGrabbed.Invoke(vegetable);
    }

    private void VegetableGrabAttempt(DynamicObject vegetable)
    {
        Debug.Log("Attempted to grab a vegetable, but the grab is not allowed.");
        OnVegetableGrabAttempt.Invoke(vegetable);
    }
}
