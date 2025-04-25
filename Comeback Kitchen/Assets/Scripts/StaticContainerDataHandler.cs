using System.Collections.Generic;
using UnityEngine;

public class StaticContainerDataHandler : MonoBehaviour
{
    [SerializeField] public StaticContainer container;
    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private StaticContainerDataAsset containerDataAsset;

    public StaticContainerDataAsset ContainerDataAsset
    {
        get => containerDataAsset;
        set => containerDataAsset = value;
    }

    public List<GameObject> TrackedObjects { get; private set; } = new List<GameObject>();

    public void LoadFromAsset()
    {
        if (containerDataAsset == null || containerDataAsset.objectData.Count == 0)
        {
            Debug.LogWarning("No object data to load.");
            return;
        }

        ClearObjects();

        foreach (var data in containerDataAsset.objectData)
        {
            GameObject instance = Instantiate(objectPrefab);
            instance.transform.SetParent(container.ObjectHolder);
            instance.transform.localPosition = data.position;
            instance.transform.localRotation = data.rotation;
        }

        Debug.Log($"Objects in {container.gameObject.name} created from asset.");
    }

    public void ClearObjects()
    {
        // Runtime destruction (no undo)
        for (int i = container.ObjectHolder.childCount - 1; i >= 0; i--)
        {
            Transform child = container.ObjectHolder.GetChild(i);
            DestroyImmediate(child.gameObject);
        }

        Debug.Log("Cleared all container objects.");
    }
}

