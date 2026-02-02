using System.Collections.Generic;
using UnityEngine;

public enum CuttingPhase
{
    Phase1,
    Phase2
}

public class CuttingState
{
    public CuttingPhase Phase { get; }
    public List<Slice> PreviousSlices { get; }

    public CuttingState(CuttingPhase phase)
    {
        Phase = phase;
        PreviousSlices = new List<Slice>();
    }
}

public struct Slice
{
    public Vector3 HorizontalForwardVector;
    public Vector3 CuttingPlaneNormal;
}

[RequireComponent(typeof(Rigidbody))]
public class Knife : MonoBehaviour
{
    [SerializeField] private BoxCollider trigger;
    [SerializeField] private LayerMask sliceableLayerMask;
    [SerializeField] private float sliceLookaheadDistance = 0.01f;
    [SerializeField] private float maxVolumeDifferencePercentage = 20f;
    [SerializeField] private bool mustFollowCuttingRules = false;
    [SerializeField] private float firstPhaseMaxYAngleError = 15f;
    [SerializeField] private float firstPhaseMaxZAngleError = 15f;
    [SerializeField] private float secondPhaseMaxAngleError = 25f;

    public SmartAction OnCut = new SmartAction();
    public CuttingState CurrentCuttingState { get; set; } = new CuttingState(CuttingPhase.Phase1);

    private Rigidbody rb;

    private Dictionary<Sliceable, Slice> activeSlices = new Dictionary<Sliceable, Slice>();
    private Vector3 slicePlaneOrigin;
    private Vector3 slicePlaneNormal;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Sliceable>(out var sliceable))
        {
            if (activeSlices.Count == 0)
            {
                slicePlaneOrigin = transform.position;
                slicePlaneNormal = transform.right;
            }

            activeSlices.Add(sliceable, new Slice { HorizontalForwardVector = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized, CuttingPlaneNormal = transform.right });
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Sliceable>(out var sliceable))
        {
            activeSlices.Remove(sliceable);
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
                if (mustFollowCuttingRules)
                {
                    // If it is still phase 1, and the object has already been cut twice, do not let it get cut again
                    if (CurrentCuttingState.Phase == CuttingPhase.Phase1 && sliceable.DivisionCount > 1)
                    {
                        return;
                    }

                    // If it is phase 2, and the object has already been cut three times, then stop (only want 2^3=8 pieces of similar size in total)
                    if (CurrentCuttingState.Phase == CuttingPhase.Phase2 && sliceable.DivisionCount > 2)
                    {
                        return;
                    }

                    if (CurrentCuttingState.Phase == CuttingPhase.Phase1)
                    {
                        Vector3 cuttingPlaneNormal = activeSlices[sliceable].CuttingPlaneNormal;
                        float degreesFromHorizontal = 90f - Vector3.Angle(Vector3.up, cuttingPlaneNormal);

                        if (Mathf.Abs(degreesFromHorizontal) > firstPhaseMaxZAngleError)
                        {
                            // Debug.Log("Z Angle Issue");
                            return;
                        }

                        if (CurrentCuttingState.PreviousSlices.Count > 0)
                        {
                            float degreesFromFirstCut = Vector3.Angle(activeSlices[sliceable].HorizontalForwardVector, CurrentCuttingState.PreviousSlices[0].HorizontalForwardVector);

                            if (Mathf.Abs(90f - degreesFromFirstCut) > firstPhaseMaxYAngleError)
                            {
                                // Debug.Log("Y Angle Issue");
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

                        float degreesFromPerpendicular = Vector3.Angle(objectFacingDirection, knifeBladeDirection);
                        float otherDegreesFromPerpendicular = Vector3.Angle(objectFacingDirection, otherKnifeBladeDirection);

                        //have 15 degrees of freedom from exactly 90 degrees (need to be perpendicular) in either direction
                        // If 1 is true, then cut the item (both will never be true at the same time). If neither are true, nothing should be cut
                        if (degreesFromPerpendicular > secondPhaseMaxAngleError && otherDegreesFromPerpendicular > secondPhaseMaxAngleError)
                        {
                            // Debug.Log("Angle Issue");
                            return;
                        }
                    }
                }

                List<GameObject> slices = sliceable.TrySlice(slicePlaneOrigin, slicePlaneNormal);

                if (slices != null && slices.Count > 0)
                {
                    // Even when not following cutting rules, we still want to validate slice sizes to avoid degenerate cuts
                    bool validSlices = MeshVolumeCalculator.AreSliceSizesValid(slices, maxVolumeDifferencePercentage);

                    if (validSlices)
                    {
                        CurrentCuttingState.PreviousSlices.Add(activeSlices[sliceable]);
                        activeSlices.Remove(sliceable);
                        Destroy(sliceable.gameObject);
                        OnCut.Invoke();
                    }
                    else
                    {
                        float slice1Volume = MeshVolumeCalculator.Volume(slices[0].GetComponent<MeshFilter>());
                        float slice2Volume = MeshVolumeCalculator.Volume(slices[1].GetComponent<MeshFilter>());
                        Debug.Log("Invalid Slice Sizes: " + slice1Volume + " , " + slice2Volume);

                        Destroy(slices[0]);
                        Destroy(slices[1]);
                    }
                }
            }
        }
    }

    // Works to keep knife in plane but causing jittery movement when player is interacting with knife, need better solution for VR
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
}
