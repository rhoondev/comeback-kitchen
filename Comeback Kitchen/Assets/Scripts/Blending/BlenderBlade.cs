using EzySlice;
using UnityEngine;

public class BlenderBlade : MonoBehaviour
{
    public static bool blenderOn;
    [SerializeField] private Material cutTomatoMaterial;
    [SerializeField] private Liquid blenderLiquidScript;
    private int sliceLayerNum = 10;
    [SerializeField] private float minSize = 1f; // Minimum scale before destroying


    //Only when tomato chunks enter the blade do they get cut
    private void OnTriggerEnter(Collider other)
    {
        SliceCooldown cooldown = other.GetComponent<SliceCooldown>();
        if (cooldown == null || !cooldown.CanBeSliced) return; // If still in cooldown period, do not cut



        // Debug.Log($"Layer Passed - {other.gameObject.layer == sliceLayerNum} + BlenderOn - {blenderOn}");
        // Check if the object can be sliced
        if (other.gameObject.layer == sliceLayerNum && blenderOn)
        {
            GameObject target = other.gameObject;


            //slice object along blade's direction
            SlicedHull hull = target.Slice(transform.position , transform.forward, cutTomatoMaterial);


            if (hull != null)
            {
                // Create the two halves
                GameObject upperHull = hull.CreateUpperHull(target, cutTomatoMaterial);
                GameObject lowerHull = hull.CreateLowerHull(target, cutTomatoMaterial);

                // Copy physics and scale down a bit
                SetupSlicedObject(upperHull);
                SetupSlicedObject(lowerHull);

                // Destroy the original
                Destroy(target);

                // Increase how full the blender is
                blenderLiquidScript.Fill(1);
            }
        }
    }



    //Logic that is applied to every sliced object that is sliced by the blender
    void SetupSlicedObject(GameObject obj)
    {

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        // obj.AddComponent<MeshCollider>().convex = true;
        // obj.AddComponent<SliceCooldown>(); // cooldown is already on there

        rb.AddExplosionForce(3f, transform.position, 0.1f);             //May be inefficient


        // Shrink the object slightly
        obj.transform.localScale *= 0.7f;

        // // Check if it's too small to keep
        // if (obj.transform.localScale.magnitude < minSize)
        // {
        //     Destroy(obj);
        //     // Increase how full the blender is
        //     blenderLiquidScript.Fill(2);
        // }

        Destroy(obj);
    }
}
