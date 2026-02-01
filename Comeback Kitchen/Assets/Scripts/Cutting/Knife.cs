using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Knife : MonoBehaviour
{
    [SerializeField] private BoxCollider trigger;
    [SerializeField] private LayerMask sliceableLayerMask;
    [SerializeField] private float firstPhaseMaxAngle = 15f;
    [SerializeField] private float secondPhaseMaxAngle = 25f;
    [SerializeField] private float sliceLookaheadDistance = 0.01f;
    [SerializeField] private float maxVolumeDifferencePercentage = 20f;

    public SmartAction OnCut = new SmartAction();
    public bool FirstPhaseCut { get; set; } = true;

    private Rigidbody rb;
    private List<Sliceable> currentlySlicing = new List<Sliceable>();
    private Vector3 slicePlaneOrigin;
    private Vector3 slicePlaneNormal;

    // private Vector3 initialCutDirection;
    // private bool alreadyMadeACut = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Sliceable>(out var sliceable))
        {
            if (currentlySlicing.Count == 0)
            {
                slicePlaneOrigin = transform.position;
                slicePlaneNormal = transform.right;
            }

            currentlySlicing.Add(sliceable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Sliceable>(out var sliceable))
        {
            currentlySlicing.Remove(sliceable);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<Sliceable>(out var sliceable))
        {
            // Check slightly ahead of the blade - if we don't overlap the sliceable, we're almost through
            Vector3 lookaheadCenter = trigger.bounds.center - transform.up * (trigger.size.y * trigger.transform.lossyScale.y + sliceLookaheadDistance);
            Collider[] overlaps = Physics.OverlapBox(lookaheadCenter, trigger.bounds.extents, transform.rotation, sliceableLayerMask);

            // Check if the sliceable is in the overlap results
            bool stillOverlapping = false;
            foreach (var overlap in overlaps)
            {
                if (overlap.gameObject == sliceable.gameObject)
                {
                    stillOverlapping = true;
                    break;
                }
            }

            // If lookahead doesn't overlap the sliceable, the blade is almost through
            if (!stillOverlapping)
            {
                List<GameObject> slices = sliceable.TrySlice(slicePlaneOrigin, slicePlaneNormal);

                // Remove from active slices to prevent repeated cut attempts
                currentlySlicing.Remove(sliceable);

                if (slices != null && slices.Count > 0)
                {
                    bool validSlices = MeshVolumeCalculator.AreSliceSizesValid(slices, maxVolumeDifferencePercentage);

                    if (validSlices)
                    {
                        Destroy(sliceable.gameObject);
                    }
                    else
                    {
                        // Debug.Log("Invalid Slices");
                        Destroy(slices[0]);
                        Destroy(slices[1]);
                        // TODO -- put X here above cooking board or something or alternativelty have another process to invoke when condition failed
                    }
                }
            }
        }
    }

    // private void FixedUpdate()
    // {
    //     if (activeSlices.Count > 0)
    //     {
    //         // Project current position onto the slicing plane
    //         Vector3 currentPos = rb.position;
    //         Vector3 offsetFromOrigin = currentPos - constraintOrigin;

    //         // Calculate distance from the plane (component along the normal)
    //         float distanceFromPlane = Vector3.Dot(offsetFromOrigin, activeSliceNormal);

    //         // Constrain to plane by removing the component perpendicular to the plane
    //         Vector3 constrainedPos = currentPos - activeSliceNormal * distanceFromPlane;

    //         // Apply the constrained position
    //         rb.MovePosition(constrainedPos);

    //         // Allow rotation around the slice normal, but constrain other axes
    //         // Get the current rotation's component around the slice normal
    //         Quaternion currentRotation = rb.rotation;

    //         // Decompose current rotation: extract rotation around the slice normal
    //         Vector3 currentForward = currentRotation * Vector3.forward;
    //         Vector3 currentUp = currentRotation * Vector3.up;

    //         // Project the forward vector onto the plane perpendicular to slice normal
    //         Vector3 projectedForward = Vector3.ProjectOnPlane(currentForward, activeSliceNormal).normalized;

    //         // If projection is too small (knife nearly parallel to constraint), use up vector instead
    //         if (projectedForward.sqrMagnitude < 0.001f)
    //         {
    //             projectedForward = Vector3.ProjectOnPlane(currentUp, activeSliceNormal).normalized;
    //         }

    //         // Build constrained rotation: keep the slice normal as the right axis,
    //         // with the projected forward determining the allowed rotation around it
    //         Vector3 constrainedUp = Vector3.Cross(projectedForward, activeSliceNormal).normalized;
    //         Quaternion constrainedRotation = Quaternion.LookRotation(projectedForward, constrainedUp);

    //         // Apply the constrained rotation
    //         rb.MoveRotation(constrainedRotation);
    //     }
    // }

    // private void OnTriggerStay(Collider other)
    // {
    //     if (other.TryGetComponent<Sliceable>(out var sliceableObject))
    //     {
    //         // Make sure the object being sliced is not different than the object that the user started to cut
    //         if (!activeEntryPositionsList.ContainsKey(sliceableObject))
    //         {
    //             return;
    //         }

    //         // Debug.Log($"Staying - {other.name}");

    //         Vector3 entryPosition = activeEntryPositionsList[sliceableObject];
    //         Vector3 sliceDirection = -transform.up; // knife's blade cuts downward, the slice direction is the negative up vector
    //         Vector3 sliceNormal = transform.right; // the normal of the slice is the knife's right vector

    //         float objectSize = Vector3.Scale(sliceableObject.GetComponent<Renderer>().bounds.size, sliceDirection.normalized).magnitude;

    //         // Calculates how far an object has moved in the direction of "sliceDirection" relative to "entryPoint"
    //         // "transform.position - entryPoint" is a displacement vector
    //         float movedDistance = Vector3.Project(transform.position - entryPosition, sliceDirection).magnitude;

    //         // make sure that if the object has already been cut twice and it is still only round 1, do not let it get cut again
    //         if (FirstPhaseCut && sliceableObject.DivisionCount > 1) return;

    //         // make sure that if it is round 2 of cuts, that no object is cut twice in this round (only want 8 pieces of similar size in total)
    //         if (!FirstPhaseCut && sliceableObject.DivisionCount > 2) return;

    //         // Make cut after knife passes through desire distance of object
    //         if (movedDistance >= objectSize * minSliceProgress)
    //         {
    //             //during cut round 1, we want the first cut to be one way and then the 2 subsuquent cuts 
    //             // (cut each half into 2) should be in a perpendicular direction.
    //             if (FirstPhaseCut)
    //             {
    //                 if (!alreadyMadeACut)
    //                 {
    //                     initialCutDirection = new Vector3(transform.forward.x, 0f, transform.forward.z);
    //                 }
    //                 else
    //                 {
    //                     Vector3 currentCutDirection = new Vector3(transform.forward.x, 0f, transform.forward.z);

    //                     initialCutDirection.Normalize();
    //                     currentCutDirection.Normalize();

    //                     float degreesFromFirstCut = Vector3.Angle(currentCutDirection, initialCutDirection);

    //                     //have 15 degrees of freedom from exactly 90 degrees (need to be perpendicular)
    //                     if (Mathf.Abs(90 - degreesFromFirstCut) > firstPhaseMaxAngle)
    //                     {
    //                        // Debug.Log("Angle Issue");
    //                         return;
    //                     }
    //                 }
    //             }
    //             else
    //             {
    //                 //if cut round2, we want the cuts to be along the side of the objects rather than along the long way 
    //                 // (since the objects are big, they will roll onto their sides so slicing the short way will be easy). 
    //                 // The short way/side cut is easy bc we can look at the knife's angle and check to see if it is close 
    //                 // to being perpendicular with transform.up (object will be rotated on its side so object.up should 
    //                 // be perpendicular to the cut)

    //                 // Want to look at the normal of the cut (other.transform.up) and the normal of the desired cut (transform.right)
    //                 Vector3 objectFacingDirection = other.transform.up;
    //                 Vector3 knifeBladeDirection = transform.right;
    //                 Vector3 otherKnifeBladeDirection = -transform.right; //need opposite direction vector since right side of blade could be pointing in opposite direction but still be valid

    //                 knifeBladeDirection.Normalize();
    //                 objectFacingDirection.Normalize();
    //                 otherKnifeBladeDirection.Normalize();

    //                 float degreesFromPerpendicular = Vector3.Angle(objectFacingDirection, knifeBladeDirection);
    //                 float otherDegreesFromPerpendicular = Vector3.Angle(objectFacingDirection, otherKnifeBladeDirection);

    //                 //have 15 degrees of freedom from exactly 90 degrees (need to be perpendicular) in either direction
    //                 // If 1 is true, then cut the item (both will never be true at the same time). If neither are true, nothing should be cut
    //                 if (degreesFromPerpendicular > secondPhaseMaxAngle && otherDegreesFromPerpendicular > secondPhaseMaxAngle)
    //                 {
    //                     return;
    //                 }

    //                 // if(Mathf.Abs(90 - degreesFromPerpendicular) > 15) {Debug.Log($"Angle Issue for pieces {degreesFromPerpendicular}"); return;}

    //                // Debug.Log($"Angle 1 - {degreesFromPerpendicular}");
    //                // Debug.Log($"Angle 2 - {otherDegreesFromPerpendicular}");
    //             }

    //             List<GameObject> slices = sliceableObject.TrySlice(entryPosition, sliceNormal);

    //             if (slices != null && slices.Count > 0)
    //             {
    //                 bool validSlices = MeshVolumeCalculator.AreSliceSizesValid(slices, maxVolumeDifferencePercentage);
    //                 if (validSlices)
    //                 {
    //                     Sliceable cut1 = slices[0].GetComponent<Sliceable>();
    //                     Sliceable cut2 = slices[1].GetComponent<Sliceable>();
    //                     OnCut.Invoke();

    //                     alreadyMadeACut = true;

    //                     Destroy(sliceableObject.gameObject);
    //                 }
    //                 else
    //                 {
    //                    // Debug.Log("Invalid Slices");
    //                     for (int counter = 0; counter < slices.Count; counter++)
    //                     {
    //                         Destroy(slices[counter]);
    //                     }
    //                     // TODO -- put X here above cooking board or something or alternativelty have another process to invoke when condition failed
    //                 }
    //                 //slices is reset here so no future contamination
    //             }
    //         }
    //     }
    // }
}
