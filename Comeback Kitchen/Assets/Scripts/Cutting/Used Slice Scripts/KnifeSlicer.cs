using UnityEngine;

//ChatGPT
public class KnifeSlicer : MonoBehaviour
{
    public float maxAllowedAngle = 25f;
    public LayerMask sliceableLayer;
    public float minSliceProgress = 0.75f;

    private Vector3 entryPoint;
    private Vector3 exitPoint;
    private bool isSlicing = false;
    private SliceableObject currentTarget;


    //Knife is rotated to be in correct orientation, so transform.foward is up (not transform.up)

    void OnTriggerEnter(Collider other)
    {
        //Check for correct layer
        if (((1 << other.gameObject.layer) & sliceableLayer) == 0) return;          //ChatGPT

        //Check for correct Knife angle
        if (Vector3.Angle(transform.forward, Vector3.down) > maxAllowedAngle) return;       //Only allow cuts that are within the allowed angle range

        SliceableObject sliceable = other.GetComponent<SliceableObject>();
        if (sliceable == null) return;

        isSlicing = true;
        currentTarget = sliceable;
        entryPoint = transform.position;
    }

    void OnTriggerStay(Collider other)
    {
        //Make sure the knife is slicing, the object layer and cut angle are correct, current target exist, and the object being sliced is not different than the object that the user started to cut
        if (!isSlicing || currentTarget == null || other.GetComponent<SliceableObject>() != currentTarget) return;

        Vector3 sliceDirection = transform.forward; // knife is rotated so transform.forward is along blade's cut direction
        float objectSize = Vector3.Scale(currentTarget.GetComponent<Renderer>().bounds.size, sliceDirection.normalized).magnitude;
        float movedDistance = Vector3.Project(transform.position - entryPoint, sliceDirection).magnitude;

        // Make cut after knife passes through desire distance of object
        if (movedDistance >= objectSize * minSliceProgress)
        {
            currentTarget.TrySlice(entryPoint, sliceDirection);
            isSlicing = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        //If the object is leaving the object, stop the cut
        if (other.GetComponent<SliceableObject>() == currentTarget)
        {
            isSlicing = false;
            currentTarget = null;
        }
    }
}
