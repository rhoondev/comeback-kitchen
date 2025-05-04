using EzySlice;
using UnityEngine;

public class BlenderBlade : MonoBehaviour
{
    public static bool blenderOn;
    [SerializeField] private Material cutTomatoMaterial;
    [SerializeField] private Liquid blenderLiquidScript;
    private int sliceLayerNum = 10;


    //Only when tomato chunks enter the blade do they get cut
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Zone Entered");
        SliceCooldown cooldown = other.GetComponent<SliceCooldown>();
        Debug.Log($"Cooldown {cooldown.CanBeSliced}");
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
                blenderLiquidScript.Fill(10);
            }
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == sliceLayerNum && blenderOn)
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            rb.AddForce(transform.up, ForceMode.Impulse);
        }
    }



    //Logic that is applied to every sliced object that is sliced by the blender
    void SetupSlicedObject(GameObject obj)
    {

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        // obj.AddComponent<MeshCollider>().convex = true;
        // obj.AddComponent<SliceCooldown>(); // cooldown is already on there

        rb.AddForce(transform.up, ForceMode.Impulse);             //May be inefficient


        // Shrink the object slightly
        SliceCooldown sc = obj.GetComponent<SliceCooldown>();
        sc.futureSlicesCount -= 1;

        // Check if it's too small to keep
        if (sc.futureSlicesCount == 0)
        {
            Destroy(obj);
            // Increase how full the blender is
            blenderLiquidScript.Fill(20);
        }
    }
}
