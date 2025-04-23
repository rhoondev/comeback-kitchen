using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    [field: SerializeField] public Transform ObjectHolder { get; private set; } // The parent object that holds the rice objects;
    [SerializeField] private GameObject prefab; // Prefab to be instantiated when releasing an object

    public List<GameObject> Objects { get; set; }
    public int MaxObjectCount { get => Objects.Count; }
    public int ObjectCount { get; private set; } = 0; // Number of objects currently in the container
    public bool IsEmpty { get => Objects.Count == 0; }

    private void Start()
    {
        Objects = new List<GameObject>();

        foreach (Transform child in ObjectHolder)
        {
            Objects.Add(child.gameObject);

            if (child.gameObject.activeSelf)
            {
                ObjectCount++;
            }
        }
    }

    public GameObject RestoreObject()
    {
        if (ObjectCount == MaxObjectCount)
        {
            return null;
        }

        GameObject targetObject = Objects[ObjectCount];
        targetObject.SetActive(true);
        ObjectCount++;

        return targetObject;
    }

    public GameObject ReleaseObject()
    {
        if (ObjectCount == 0)
        {
            return null;
        }

        GameObject original = Objects[ObjectCount - 1];

        GameObject copy = Instantiate(prefab, original.transform.position, original.transform.rotation);
        copy.transform.SetParent(null);
        copy.GetComponent<Collider>().enabled = true;
        copy.GetComponent<Rigidbody>().isKinematic = false;

        original.SetActive(false);
        ObjectCount--;

        return copy;
    }
}
