using System.Collections.Generic;
using UnityEngine;

//ChatGPT
public class KnifeSlicer : MonoBehaviour
{
    Dictionary<SliceableObject, Vector3> _activeEntryPositionsList = new Dictionary<SliceableObject, Vector3>();


    public bool makePerpendicularCuts = true;
    private bool alreadyMadeACut = false;

    public bool firstPhaseCut = true;

    public float maxAllowedAngle = 25f;
    public LayerMask sliceableLayer;
    public float minSliceProgress = 0.75f;

    private Vector3 initialCutDirection;
    [SerializeField] float MaxVolumeDifferencePercentage;
    // public float MaxVolumeDifferencePercentage {get; set;} = 5f;
    public Material desiredCutMaterial;



    [SerializeField] CuttingSystem cutSys;




    //Knife is rotated to be in correct orientation, so transform.foward is up (not transform.up)

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Entered - {other.name}");

        //Check for correct layer
        if (((1 << other.gameObject.layer) & sliceableLayer) == 0) return;          //ChatGPT

        // TODO -- change
        //Check for correct Knife angle 
        // if (Vector3.Angle(-transform.forward, Vector3.down) > maxAllowedAngle) return;       //Only allow cuts that are within the allowed angle range

        SliceableObject sliceable = other.GetComponent<SliceableObject>();
        if (sliceable == null) return;

        _activeEntryPositionsList.Add(sliceable, transform.position);

    }

    void OnTriggerStay(Collider other)
    {
        SliceableObject keyValue = other.GetComponent<SliceableObject>();
        //Make sure the knife is slicing, the object layer and cut angle are correct, current target exist, 
        // and the object being sliced is not different than the object that the user started to cut

        if(_activeEntryPositionsList == null || keyValue == null) return;

        if (_activeEntryPositionsList.Count == 0 || !_activeEntryPositionsList.ContainsKey(keyValue)) return;
        
        Vector3 entryPosition = _activeEntryPositionsList[keyValue];
        Vector3 sliceDirection = -transform.up; // knife's blade cuts downward, so take the opposite vector of up (down)

        float objectSize = Vector3.Scale(keyValue.GetComponent<Renderer>().bounds.size, sliceDirection.normalized).magnitude;
        // Calculates how far an object has moved in the direction of "sliceDirection" relative to "entryPoint"
        // "transform.position - entryPoint" is a displacement vector
        float movedDistance = Vector3.Project(transform.position - entryPosition, sliceDirection).magnitude;


        // make sure that if the object has already been cut twice and it is still only round 1, do not let it get cut again
        if(firstPhaseCut && keyValue.DivisionCount > 1) return;

        // make sure that if it is round 2 of cuts, that no object is cut twice in this round (only want 8 pieces of similar size in total)
        if(!firstPhaseCut && keyValue.DivisionCount > 2) return;

        


        // Make cut after knife passes through desire distance of object
        if (movedDistance >= objectSize * minSliceProgress)
        {
            if(makePerpendicularCuts)
            {
                if(!alreadyMadeACut)
                {
                    initialCutDirection = transform.forward;
                    initialCutDirection.y = 0;
                }
                else
                {
                    Vector3 currentCutDirection = transform.forward;
                    currentCutDirection.y = 0;


                    initialCutDirection.Normalize();
                    currentCutDirection.Normalize();


                    float degreesFromFirstCut = Vector3.Angle(currentCutDirection, initialCutDirection);


                    //have 15 degrees of freedom from exactly 90 degrees (need to be perpendicular)
                    if(Mathf.Abs(90 - degreesFromFirstCut) > 15) {Debug.Log("Angle Issue"); return;}
                }
            }
            //target's cut material is always null unless manually changed, so worst case is that we set a null var to null
            // desiredCutMaterial only has to be set if a different material is desired/needed
            keyValue.cutMaterial = desiredCutMaterial;
            List<GameObject> slices = keyValue.TrySlice(entryPosition, transform.right);

            

            if(slices != null && slices.Count > 0)
            {
                bool validSlices = MeshVolumeCalculator.AreSliceSizesValid(slices, MaxVolumeDifferencePercentage);
                if(validSlices)
                {
                    SliceableObject cut1 = slices[0].GetComponent<SliceableObject>();
                    SliceableObject cut2 = slices[1].GetComponent<SliceableObject>();
                    cutSys.ReplaceObject(keyValue, cut1, cut2);
                    
                    alreadyMadeACut = true;



                    Destroy(keyValue.gameObject);
                }
                else
                {
                    Debug.Log("Invalid Slices");
                    for(int counter = 0; counter < slices.Count; counter++)
                    {
                        Destroy(slices[counter]);
                    }
                    // TODO -- put X here above cooking board or something or alternativelty have another process to invoke when condition failed
                }
                //slices is reset here so no future contamination
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        //If the object is leaving the object, stop the cut
        SliceableObject objSliceScript = other.GetComponent<SliceableObject>();
        if(objSliceScript == null) return;
        
        _activeEntryPositionsList.Remove(objSliceScript);
    }


}
