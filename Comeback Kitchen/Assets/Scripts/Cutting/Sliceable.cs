using UnityEngine;
using EzySlice;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class Sliceable : MonoBehaviour
{
    [SerializeField] private float separationDistance = 0.01f;
    [SerializeField] private Material crossSectionMaterial;

    public int DivisionCount { get; private set; }

    public SmartAction<int> OnCreated = new SmartAction<int>();

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (crossSectionMaterial == null)
        {
            // Default to the object's material if no cross-section material is assigned
            crossSectionMaterial = gameObject.GetComponent<Renderer>().material;
        }
    }

    public List<GameObject> TrySlice(Vector3 slicePos, Vector3 sliceNormal)
    {
        SlicedHull hull = gameObject.Slice(slicePos, sliceNormal, crossSectionMaterial);

        if (hull == null)
        {
            return null;
        }

        GameObject upper = hull.CreateUpperHull(gameObject, crossSectionMaterial);
        upper.transform.parent = transform.parent;
        Sliceable upperSliceable = upper.GetComponent<Sliceable>();
        upperSliceable.DivisionCount = DivisionCount + 1;
        upperSliceable.OnCreated.Invoke(upperSliceable.DivisionCount);

        GameObject lower = hull.CreateLowerHull(gameObject, crossSectionMaterial);
        lower.transform.parent = transform.parent;
        Sliceable lowerSliceable = lower.GetComponent<Sliceable>();
        lowerSliceable.DivisionCount = DivisionCount + 1;
        lowerSliceable.OnCreated.Invoke(lowerSliceable.DivisionCount);

        // Apply a small separation to the sliced pieces
        upper.transform.position += sliceNormal.normalized * separationDistance / 2f;
        lower.transform.position -= sliceNormal.normalized * separationDistance / 2f;

        // Knife will handle cleanup logic, in case slices are invalid and original needs to be restored
        // Destroy(gameObject);

        return new List<GameObject> { upper, lower };
    }

    public void Unpin()
    {
        rb.constraints = RigidbodyConstraints.None;
    }

    public void Pin()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }
}
