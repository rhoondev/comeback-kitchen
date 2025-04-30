using System.Collections.Generic;
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
    public float MaxVolumeDifferencePercentage {get; set;} = 5f;
    public Material desiredCutMaterial;

    List<GameObject> slices;

    public SmartAction OnCutPassed = new SmartAction();


    //Knife is rotated to be in correct orientation, so transform.foward is up (not transform.up)

    void OnTriggerEnter(Collider other)
    {

        //Check for correct layer
        if (((1 << other.gameObject.layer) & sliceableLayer) == 0) return;          //ChatGPT

        // TODO -- change
        //Check for correct Knife angle 
        // if (Vector3.Angle(-transform.forward, Vector3.down) > maxAllowedAngle) return;       //Only allow cuts that are within the allowed angle range

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
        

        Vector3 sliceDirection = -transform.up; // knife's blade cuts downward, so take the opposite vector of up (down)

        float objectSize = Vector3.Scale(currentTarget.GetComponent<Renderer>().bounds.size, sliceDirection.normalized).magnitude;
        // Calculates how far an object has moved in the direction of "sliceDirection" relative to "entryPoint"
        // "transform.position - entryPoint" is a displacement vector
        float movedDistance = Vector3.Project(transform.position - entryPoint, sliceDirection).magnitude;




        // Make cut after knife passes through desire distance of object
        if (movedDistance >= objectSize * minSliceProgress)
        {
            //target's cut material is always null unless manually changed, so worst case is that we set a null var to null
            // desiredCutMaterial only has to be set if a different material is desired/needed
            currentTarget.cutMaterial = desiredCutMaterial;

            List<GameObject> slices = currentTarget.TrySlice(entryPoint, transform.right);

            

            if(slices != null && slices.Count > 0)
            {
                bool validSlices = MeshVolumeCalculator.AreSliceSizesValid(slices, MaxVolumeDifferencePercentage);
                if(validSlices)
                {
                    Destroy(currentTarget.gameObject);
                    OnCutPassed.Invoke();
                }
                else
                {
                    for(int counter = 0; counter < slices.Count; counter++)
                    {
                        Destroy(slices[counter]);
                    }
                    // TODO -- put X here above cooking board or something or alternativelty have another process to invoke when condition failed
                }
                slices = null;  //reset slices to avoid confusion
            }
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
