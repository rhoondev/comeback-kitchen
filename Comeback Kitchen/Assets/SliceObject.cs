using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class SliceObject : MonoBehaviour
{

    public Transform planeDebug;
    public GameObject target;
    public Material crossSectionMaterial;
    public float cutForce = 2000;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Slice(target);
        }
    }

    public void Slice(GameObject target)
    {
        SlicedHull hull = target.Slice(planeDebug.position, planeDebug.up);

        if(hull != null)
        {
            GameObject upperHull = hull.CreateUpperHull(target, crossSectionMaterial);
            // make upper slice behave with physics
            SetupSlicedComponent(upperHull);


            GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMaterial);
            // make lower slice behave with physics
            SetupSlicedComponent(lowerHull);

            Destroy(target);
        }
    }


    public void SetupSlicedComponent(GameObject slicedObject)
    {
        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();
        // mesh colliders need to be convex when used with a rigidbody
        collider.convex = true;

        // add an initial force to each object
        rb.AddExplosionForce(cutForce, slicedObject.transform.position, 1);
    }
}
