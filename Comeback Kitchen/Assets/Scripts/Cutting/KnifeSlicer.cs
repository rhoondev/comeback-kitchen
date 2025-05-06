using System.Collections.Generic;
using UnityEngine;

public class KnifeSlicer : MonoBehaviour
{
    [SerializeField] private CuttingSystem cuttingSystem;
    [SerializeField] private float firstPhaseMaxAngle = 15f;
    [SerializeField] private float secondPhaseMaxAngle = 25f;
    [SerializeField] private float minSliceProgress = 0.75f;
    [SerializeField] private float MaxVolumeDifferencePercentage;

    public bool FirstPhaseCut { get; set; } = true;

    private Dictionary<SliceableObject, Vector3> _activeEntryPositionsList = new Dictionary<SliceableObject, Vector3>();
    private Vector3 initialCutDirection;
    private bool alreadyMadeACut = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<SliceableObject>(out var sliceableObject))
        {
            // Debug.Log($"Entered - {other.name}");

            _activeEntryPositionsList.Add(sliceableObject, transform.position);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<SliceableObject>(out var sliceableObject))
        {
            // Make sure the object being sliced is not different than the object that the user started to cut
            if (!_activeEntryPositionsList.ContainsKey(sliceableObject))
            {
                return;
            }

            // Debug.Log($"Staying - {other.name}");

            Vector3 entryPosition = _activeEntryPositionsList[sliceableObject];
            Vector3 sliceDirection = -transform.up; // knife's blade cuts downward, so take the opposite vector of up (down)

            float objectSize = Vector3.Scale(sliceableObject.GetComponent<Renderer>().bounds.size, sliceDirection.normalized).magnitude;
            // Calculates how far an object has moved in the direction of "sliceDirection" relative to "entryPoint"
            // "transform.position - entryPoint" is a displacement vector
            float movedDistance = Vector3.Project(transform.position - entryPosition, sliceDirection).magnitude;

            // make sure that if the object has already been cut twice and it is still only round 1, do not let it get cut again
            if (FirstPhaseCut && sliceableObject.DivisionCount > 1) return;

            // make sure that if it is round 2 of cuts, that no object is cut twice in this round (only want 8 pieces of similar size in total)
            if (!FirstPhaseCut && sliceableObject.DivisionCount > 2) return;

            // Make cut after knife passes through desire distance of object
            if (movedDistance >= objectSize * minSliceProgress)
            {
                //during cut round 1, we want the first cut to be one way and then the 2 subsuquent cuts 
                // (cut each half into 2) should be in a perpendicular direction.
                if (FirstPhaseCut)
                {
                    if (!alreadyMadeACut)
                    {
                        initialCutDirection = new Vector3(transform.forward.x, 0f, transform.forward.z);
                    }
                    else
                    {
                        Vector3 currentCutDirection = new Vector3(transform.forward.x, 0f, transform.forward.z);

                        initialCutDirection.Normalize();
                        currentCutDirection.Normalize();

                        float degreesFromFirstCut = Vector3.Angle(currentCutDirection, initialCutDirection);

                        //have 15 degrees of freedom from exactly 90 degrees (need to be perpendicular)
                        if (Mathf.Abs(90 - degreesFromFirstCut) > firstPhaseMaxAngle)
                        {
                            Debug.Log("Angle Issue");
                            return;
                        }
                    }
                }
                else
                {
                    //if cut round2, we want the cuts to be along the side of the objects rather than along the long way 
                    // (since the objects are big, they will roll onto their sides so slicing the short way will be easy). 
                    // The short way/side cut is easy bc we can look at the knife's angle and check to see if it is close 
                    // to being perpendicular with transform.up (object will be rotated on its side so object.up should 
                    // be perpendicular to the cut)

                    // Want to look at the normal of the cut (other.transform.up) and the normal of the desired cut (transform.right)
                    Vector3 objectFacingDirection = other.transform.up;
                    Vector3 knifeBladeDirection = transform.right;
                    Vector3 otherKnifeBladeDirection = -transform.right; //need opposite direction vector since right side of blade could be pointing in opposite direction but still be valid

                    knifeBladeDirection.Normalize();
                    objectFacingDirection.Normalize();
                    otherKnifeBladeDirection.Normalize();

                    float degreesFromPerpendicular = Vector3.Angle(objectFacingDirection, knifeBladeDirection);
                    float otherDegreesFromPerpendicular = Vector3.Angle(objectFacingDirection, otherKnifeBladeDirection);

                    //have 15 degrees of freedom from exactly 90 degrees (need to be perpendicular) in either direction
                    // If 1 is true, then cut the item (both will never be true at the same time). If neither are true, nothing should be cut
                    if (degreesFromPerpendicular > secondPhaseMaxAngle && otherDegreesFromPerpendicular > secondPhaseMaxAngle)
                    {
                        return;
                    }

                    // if(Mathf.Abs(90 - degreesFromPerpendicular) > 15) {Debug.Log($"Angle Issue for pieces {degreesFromPerpendicular}"); return;}

                    Debug.Log($"Angle 1 - {degreesFromPerpendicular}");
                    Debug.Log($"Angle 2 - {otherDegreesFromPerpendicular}");
                }

                List<GameObject> slices = sliceableObject.TrySlice(entryPosition, transform.right);

                if (slices != null && slices.Count > 0)
                {
                    bool validSlices = MeshVolumeCalculator.AreSliceSizesValid(slices, MaxVolumeDifferencePercentage);
                    if (validSlices)
                    {
                        SliceableObject cut1 = slices[0].GetComponent<SliceableObject>();
                        SliceableObject cut2 = slices[1].GetComponent<SliceableObject>();
                        cuttingSystem.ReplaceObject(sliceableObject, cut1, cut2);

                        alreadyMadeACut = true;

                        Destroy(sliceableObject.gameObject);
                    }
                    else
                    {
                        Debug.Log("Invalid Slices");
                        for (int counter = 0; counter < slices.Count; counter++)
                        {
                            Destroy(slices[counter]);
                        }
                        // TODO -- put X here above cooking board or something or alternativelty have another process to invoke when condition failed
                    }
                    //slices is reset here so no future contamination
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the object is leaving the object, stop the cut
        if (other.TryGetComponent<SliceableObject>(out var sliceableObject))
        {
            _activeEntryPositionsList.Remove(sliceableObject);
        }
    }
}
