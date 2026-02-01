using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private GrabInterceptor tomatoPrefab;
    [SerializeField] private GrabInterceptor bellPepperPrefab;
    [SerializeField] private GrabInterceptor onionPrefab;
    [SerializeField] private GrabInterceptor distraction1Prefab;
    [SerializeField] private GrabInterceptor distraction2Prefab;
    [SerializeField] private GrabInterceptor distraction3Prefab;
    [SerializeField] private Transform[] spawnPoints;

    public SmartAction<DynamicObject> OnVegetableGrabbed = new SmartAction<DynamicObject>();
    public SmartAction<DynamicObject> OnVegetableGrabAttempt = new SmartAction<DynamicObject>();
    public SmartAction<DynamicObject> OnDistractionGrabAttempt = new SmartAction<DynamicObject>();

    private Dictionary<VegetableType, GrabInterceptor> _vegetableDictionary;
    private List<GrabInterceptor> _allInstances;
    private GrabInterceptor _activeVegetable;

    private void Start()
    {
        if (spawnPoints.Length < 6)
        {
            // Debug.LogError("Not enough spawn points for vegetables!");
            return;
        }

        // Shuffle the spawn points to randomize locations
        var shuffledSpawnPoints = spawnPoints.OrderBy(x => Random.value).ToList();
        int spawnIndex = 0;

        _vegetableDictionary = new Dictionary<VegetableType, GrabInterceptor>();
        _allInstances = new List<GrabInterceptor>();

        // 1. Spawn Vegetables explicitly
        SpawnVegetable(tomatoPrefab, VegetableType.Tomato, shuffledSpawnPoints[spawnIndex++]);
        SpawnVegetable(bellPepperPrefab, VegetableType.BellPepper, shuffledSpawnPoints[spawnIndex++]);
        SpawnVegetable(onionPrefab, VegetableType.Onion, shuffledSpawnPoints[spawnIndex++]);

        // 2. Spawn Distractions
        var distractions = new List<GrabInterceptor> { distraction1Prefab, distraction2Prefab, distraction3Prefab };
        foreach (var distractionPrefab in distractions)
        {
            SpawnDistraction(distractionPrefab, shuffledSpawnPoints[spawnIndex++]);
        }
    }

    private void SpawnVegetable(GrabInterceptor prefab, VegetableType type, Transform spawnPoint)
    {
        if (prefab == null) return;

        var instance = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        instance.name = $"{type}_Spawned"; // Rename for easier debugging
        _allInstances.Add(instance);

        instance.OnGrabbed.Add(VegetableGrabbed);
        instance.OnGrabAttempt.Add(VegetableGrabAttempt);

        _vegetableDictionary[type] = instance;
    }

    private void SpawnDistraction(GrabInterceptor prefab, Transform spawnPoint)
    {
        if (prefab == null) return;

        var instance = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        instance.name = $"{prefab.name}_Distraction_Spawned"; // Rename for easier debugging
        _allInstances.Add(instance);

        instance.OnGrabAttempt.Add(DistractionGrabAttempt);
    }

    public void SetActiveVegetableType(VegetableType? vegetableType)
    {
        if (_activeVegetable != null)
        {
            _activeVegetable.IsGrabbable = false; // Disable the current vegetable
        }

        if (vegetableType != null)
        {
            _activeVegetable = _vegetableDictionary[(VegetableType)vegetableType]; // Set the new vegetable
            _activeVegetable.IsGrabbable = true; // Enable the new vegetable

            // Debug.Log($"Activated: {vegetableType}.");
        }
        else
        {
            _activeVegetable = null; // No vegetable is active
                                     // Debug.Log("Deactivated all vegetables.");
        }
    }

    public void Unlock()
    {
        if (_allInstances == null) return;

        foreach (var instance in _allInstances)
        {
            instance.GetComponent<InteractionLocker>().UnlockInteraction();
        }
    }

    public void Lock()
    {
        if (_allInstances == null) return;

        foreach (var instance in _allInstances)
        {
            instance.GetComponent<InteractionLocker>().LockInteraction();
        }
    }

    private void VegetableGrabbed(DynamicObject vegetable)
    {
        // Debug.Log("Vegetable grabbed successfully.");
        OnVegetableGrabbed.Invoke(vegetable);
    }

    private void VegetableGrabAttempt(DynamicObject vegetable)
    {
        // Debug.Log($"Attempted to grab a vegetable ({vegetable.name}), but the grab is not allowed.");
        OnVegetableGrabAttempt.Invoke(vegetable);
    }

    private void DistractionGrabAttempt(DynamicObject distraction)
    {
        // Debug.Log($"Attempted to grab a distraction ({distraction.name}).");
        OnDistractionGrabAttempt.Invoke(distraction);
    }
}
