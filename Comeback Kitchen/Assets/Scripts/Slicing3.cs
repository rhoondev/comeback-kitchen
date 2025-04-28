using UnityEngine;
using EzySlice;
using System.Collections;
using TMPro;


// Difference between slicing2 and slicing3 is that slicing2 looks after a cut to see 
// if the cut is at a valid angle while slicing3 looks before the cut to see if it 
// is at a valid angle
public class Slicing3 : MonoBehaviour
{

    // -----   Constants   -----
    [SerializeField] private const int sliceLayerNum = 10;

    [SerializeField] private const float invalidMarkerOffsetFromObject = 0.2f;


    

    // -----   Cutting Info   -----
    // [SerializeField] private float cutForce;

    [SerializeField] private LayerMask sliceableLayer;      // layer that can be cut by knife objects

    [SerializeField] private float minCutPercentageRange;       // This is the range of which percents can vary to be legit (ex. 5 would mean +5 and -5 from targetCutPercentage)

    [SerializeField] private int targetCutTimes;              // Target cut times is variable that keeps track of the pieces you want something subdivided into

    private int numberOfCuts = 0;

    private float targetCutPercentage;         // This is the target for which the chunks should be (if you want 3 chunks of equal size, the targetCutPercentage would be 33)

    [SerializeField] private GameObject parentObjPrefab;


    
    [SerializeField] private GameObject invalidCutMarker;

    [SerializeField] private float cutSpacing;          // Variable used to control the distance sliced objects are placed from one another

    


    private bool previouslyHit = false;                 // bool to keep track of if the knife previously entered 

    

    private GameObject lastHullHit;

    private Rigidbody knifeRB;

    public bool doneCutting = false;




    // -----   Knife Blade Points   -----
    [SerializeField] private Transform startSlicePoint;     // blade handle spot

    [SerializeField] private Transform endSlicePoint;       // blade tip spot

    [SerializeField] private Transform sliceAngle;          // point in line with blade (if blade's rotation is normal, this point will be directly below endSlicePoint)

    //



    // -----   Material Used   -----
    private Material crossSectionMaterial;



    
    // -----   Debug Configuration Variables   -----
    [SerializeField] private float debugCircleRadius;     // Size of circles to show spots on knife and rays for math

























    void Awake()
    {
        targetCutPercentage = 100 / targetCutTimes;
        knifeRB = GetComponent<Rigidbody>();
    }




    void FixedUpdate()
    {
        if(!doneCutting)
        {
            // For future, make list of all points that knife goes through in an object. If the knife stops touching object, 
            //  take last pair of points in that list and create a plane with the first pair of points in that object
            // Also make new variable (prevHit) which is true if hasHit was true and say if(prevHit == true and hasHit != true) 
            //  ->  make a plane (Transform) and use .position and .up on the transform to get the correct info (vector and position) for the slice function

            bool hasHit = Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hit, sliceableLayer);
            // Debug.Log("Hit - " + hasHit);

            // don't need to keep looking at the knife's rotation if it's rotation is locked (will not change), hence 
            // why the !previouslyHit is in there
            if(hasHit && !previouslyHit)         
            {

                // Make sure the knife's cut angle is valid
                Vector3 cutAngle = sliceAngle.position - endSlicePoint.position;    //vector pointing in directin of slice
                bool validCutAngle = ValidAngleOfCut(cutAngle);             
                
                
                
                if(validCutAngle)
                {
                    lastHullHit = hit.transform.gameObject;
                    
                    
                    knifeRB.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY; //ChatGPT
                }
                else
                {
                    StepFailOutcome("cut was at an invalid angle");
                }



                previouslyHit = true;
            }
            else if(!hasHit)    // only care if object is not currently being hit
            {
                // Only slice an object if the knife went through the object fully (previously entered the object and has since left the object)
                if(previouslyHit)
                {
                    previouslyHit = false;

                    // Rigidbody rb = hit.rigidbody;
                    Debug.Log("Rigidbody - " + knifeRB);

                    if(Slice(lastHullHit))
                        numberOfCuts++;

                    


                    // Unlock the Knife's Position (by setting constraint to just be the knife's rotation).
                    // Do not want to unlock the rotation so that the knife stays at the same angle for all cuts
                    if(numberOfCuts != targetCutTimes)
                        knifeRB.constraints = RigidbodyConstraints.FreezeRotation;
                    else    // unlock position and rotation as soon as every cut has been made
                    {
                        knifeRB.constraints = RigidbodyConstraints.None;
                        doneCutting = true;
                    }




                    
                }
            }
        }
        else
        {
            Debug.Log("You are done cutting, do something else!!!");
        }

    }




    // If the object is too small, or the cut is at a bad angle, then do some random stuff
    public void StepFailOutcome(string errorMessage)
    {
        Vector3 currentPosition = transform.position;

        transform.position = new Vector3(currentPosition.x, currentPosition.y + 0.1f, currentPosition.z);

        // Display X above bad cut (TODO -- Get actual rejection image/fix rotation when actual rejection image is added)
        transform.position = new Vector3(currentPosition.x, currentPosition.y + 0.1f, currentPosition.z);


        Vector3 markerLocation = new Vector3(currentPosition.x, currentPosition.y + invalidMarkerOffsetFromObject, currentPosition.z);
        Quaternion validRotation = new Quaternion(1, 0, 1, 0);
        GameObject marker = Instantiate(invalidCutMarker, markerLocation, validRotation);



        TMP_Text textbox = marker.transform.Find("Error Msg").GetComponent<TMP_Text>();
        textbox.text = errorMessage;

        StartCoroutine(removeBadCutMarker(marker));
    }





    bool ValidAngleOfCut(Vector3 bladeVector)
    {
        Vector3 endPosition = endSlicePoint.position;
        Vector3 belowEndPosition = new Vector3(endPosition.x, endPosition.y - 0.1f, endPosition.z);


        // If A & B are points and I do, A - B, I get a vector that starts at point B and points to point A
        Vector3 verticalVector = belowEndPosition - endPosition;



        verticalVector.Normalize();     // vector of vertical (normal to floor plane)
        bladeVector.Normalize();        // vector of direction or blade edge

        
        float dotResult = Vector3.Dot(verticalVector, bladeVector);       // Have Associative property or something so order of dot math does not matter

        float radianAngleBetweenVectors = Mathf.Acos(dotResult);
        
        DebugKnifeAngle(endPosition, verticalVector, bladeVector, radianAngleBetweenVectors);

        if(radianAngleBetweenVectors * Mathf.Rad2Deg > 15)
        {
            Debug.Log("Knife Angle is too great so... ignoring the cut now");
            // Debug.Log("Radians - " + radianAngleBetweenVectors + "\nDegrees - " + radianAngleBetweenVectors * Mathf.Rad2Deg);
            return false;
        }

        return true;


        //Normalizing vectors and then taking dot product has a crazy math ramification
        // Once the vectors are normalized, the dot product produces a range of 1 for the vectors 
        // being the same (0) and -1 for the vectors being opposite (180).
        // When graphed, this function also follows the cosine function, so I can easily get the 
        // angle between the 2 vectors using the dot product.
        // Since I can get the angle and I know that one of the vectors is always pointing down 
        // (verticalVector), I can constrain the angle from vertical to be whatever I
        // want it to be. 
        // Godot's very helpful explanation here - https://docs.godotengine.org/en/stable/tutorials/math/vector_math.html
        // (Explanation here from stackoverflow - https://stackoverflow.com/questions/10002918/what-is-the-need-for-normalizing-a-vector#:~:text=Reading%20Godot%20Game%20Engine%20documentation%20about%20unit%20vector%2C%20normalization%2C%20and%20dot%20product%20really%20makes%20a%20lot%20of%20sense.%20Here%20is%20the%20article%3A)


        //
    }




    /*
    Actually returns a % instead of a ratio
    Also returns the % of the total volume that hull1 contains
    */
    float findVolumeRatios(SliceVars hull1, SliceVars hull2)
    {


        float bounds1Volume = hull1.GetCurrentSize();
        float bounds2Volume = hull2.GetCurrentSize();

        float totalVolume = bounds1Volume + bounds2Volume;
        Debug.Log("Total Volume - " + totalVolume);
        Debug.Log("Upper Volume - " + bounds1Volume);

        return 100 * bounds1Volume / totalVolume;         //Multiply by 100 to get the ratio in terms of percentages
    }



    bool ValidCuts(SliceVars upperHull, SliceVars lowerHull)
    {
        float upperHullPercentage = findVolumeRatios(upperHull, lowerHull);
        float lowerHullPercentage = 100 - upperHullPercentage;
        // float cutPercentOfOriginal = FindPercentageOfOriginalSize(target);

                    // closenessToTarget is a variable that stores the remainder of dividing the percentage by the original target.
                    // If for example, someone wanted thirds, 33% & 66% for the first cut, then we would want to find out if they got
                    // these numbers by modding the the percent by the target percents (66 % 33 = 0). Of course, due to humans being
                    // inaccurate, the percentages would never be exactly 33% splits, so a range has to be implemented. This range
                    // is stored in the variable minCutPercentageRange. For example, a cut that got 39% in a scenario where the range 
                    // was + or - 7, the cut would be good. Another example would be a cut that got 26% in a scenario with the same 
                    // range. Both of these examples are 6% away from the desired ratios, but they are good enough. This means we need 
                    // an upper and lower range in case the number is slightly higher or slightly lower than the desired number.
                    // float closenessToTarget = cutPercentOfOriginal % targetCutPercentage;

        Debug.Log("Upper Hull is " + upperHullPercentage + " % of the Volume");
        Debug.Log("Lower Hull is " + lowerHullPercentage + " % of the Volume");


        // Want to make sure that the cut was not an edge case (if the cut was 6% of original size, ten we do not want to pass it since 6% is within the range)
        bool notEdgeCaseCutUp = ((int)upperHullPercentage / (int)targetCutPercentage) > 0;
        bool notEdgeCaseCutLow = ((int)lowerHullPercentage / (int)targetCutPercentage) > 0;

        // max number minus range gives us the lower bound
        float upperLimit = targetCutPercentage - minCutPercentageRange;

        // max it can be above the desired percentage is the minCutPercentageRange value, so this is the upper limit
        float lowerLimit = minCutPercentageRange;

        float upperCloseness = upperHullPercentage % targetCutPercentage;
        float lowerCloseness = lowerHullPercentage % targetCutPercentage;

        // Debug.Log("Upper Limit - " + upperLimit);
        // Debug.Log("Lower Limit - " + lowerLimit);
        // Debug.Log("Closeness Up - " + upperCloseness);
        // Debug.Log("Closeness Low - " + lowerCloseness);

        bool upperIsWithinRange = upperCloseness < lowerLimit || upperCloseness > upperLimit;
        bool lowerIsWithinRange = lowerCloseness < lowerLimit || lowerCloseness > upperLimit;

        if( upperIsWithinRange && lowerIsWithinRange && notEdgeCaseCutUp && notEdgeCaseCutLow )
        {
            // Cut is good since it was not in the invalid range (if range is + or - 6%, then invalid range would be between 6% and 28%, assuming 3 cuts (33%) )
            return true;
        }

        return false;
    }









    // TODO -- fix issue with knife still slicing after being removed upwards (should stop cut instead)
    // This function cuts the object in 2 halfs (separated by the normal plane of a transform)
    public bool Slice(GameObject target)
    {

        Vector3 endSlicePosition = endSlicePoint.position;
        Vector3 startSlicePosition = startSlicePoint.position;
        Vector3 anglePosition = sliceAngle.position;

        Vector3 bladeVector = endSlicePosition - startSlicePosition;
        Vector3 directionVector = anglePosition - endSlicePosition;

        Debug.DrawRay(startSlicePosition, bladeVector, Color.cyan);
        Debug.DrawRay(endSlicePosition, directionVector, Color.red);

        Vector3 cutNormal = Vector3.Cross(directionVector, bladeVector);
        cutNormal.Normalize();


        Debug.DrawRay(endSlicePosition, cutNormal, Color.yellow);


        // point on plane and normal to plane to get the equation for a plane that the Slice() function needs
        SlicedHull hull = target.Slice(endSlicePosition, cutNormal);



        if(hull != null)
        {



            SliceVars sliceVarsForObject = target.GetComponent<SliceVars>();

            // if it is the first slice, then make sure the first slice variable is set to false so no future slices are thought to be the first
            if(sliceVarsForObject.isFirstSlice())
                sliceVarsForObject.firstSlice();



            
            
            Vector3 parentPos = target.transform.position;



            // Cross section material is material that is put on cut face once cut happens
            crossSectionMaterial = target.GetComponent<Renderer>().material;
            

            GameObject upperHull = hull.CreateUpperHull(target, crossSectionMaterial);


            // set up the upper slice so that it acts like an actually sliceable object
            SetupSlicedComponent(upperHull, parentPos, target);





            GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMaterial);
            
            
            // set up the lower slice so that it acts like an actually sliceable object
            SetupSlicedComponent(lowerHull, parentPos, target);
            


            bool cutsAreValidSizes = ValidCuts(upperHull.GetComponent<SliceVars>(), lowerHull.GetComponent<SliceVars>());






            







            // Code R2
            if(cutsAreValidSizes)
            {
                // TODO -- figure out how to move both pieces away from the slice point (maybe move then the pieces away from the endSlicePoint by 0.01)
                Destroy(target);

                // Direction from lowerHull to upperHull (normalized)
                Debug.Log("Upper Position - " + upperHull.transform.position);
                Debug.Log("Lower Position - " + lowerHull.transform.position);
                Vector3 direction = (upperHull.transform.position - lowerHull.transform.position).normalized;
                
                
                
                // Push B slightly away along that direction
                Debug.Log("Direction - " + direction);
                Debug.Log("Change - " + direction * cutSpacing);
                upperHull.transform.position += direction * cutSpacing;



                // if valid, then add objects to grabbable object that stays together until taken off of spikes
                Transform parentInfo = checkForParent(target);      // parent is actually empty object that holds all of pieces together

            
                // make the sliced object a child of a single parent object that can be grabbed
                upperHull.transform.SetParent(parentInfo);
                lowerHull.transform.SetParent(parentInfo);


                return true;
            }
            else
            {
                StepFailOutcome("Cut size was not even enough");
                Destroy(upperHull);
                Destroy(lowerHull);

                Debug.Log("One of the cuts was too small");
                return false;
            }

        }
        else
        {
            Debug.Log("Hull is somehow null (should be impossible)");
            return false;
        }
    }




 // This function adds a rigidbody and a collider, and gives the object a force away from slice, after the object is cut
    public void SetupSlicedComponent(GameObject slicedObject, Vector3 position, GameObject target)
    {
        
        // set object's vars so that is is already cut and knows the original parent's size
        SliceVars vars = slicedObject.AddComponent<SliceVars>();
        vars.alreadyCut();          
        vars.setOriginalSize(target.GetComponent<SliceVars>().GetOriginalSize());


        




        // make sliced object have physics and stuff
        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
        

        //make sliced object be able to interact with the environment
        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();




        // make sure the object does not move after being sliced and are basically locked in plane next to the parent object
        rb.useGravity = false;
        rb.isKinematic = true;



        // make new object sliceable again
        slicedObject.layer = sliceLayerNum;



        //TODO -- remove if meshes are ever fixed
        // Next 2 lines of code are super important for chicken breasts and any other mesh that has children
        // change the created object's position if necessary (if a position is entered/0,0,0 will be sent if not necessary)
        if(position != Vector3.zero)
            slicedObject.transform.position = position;

    }





    /* 
     * Gameobject will check to see if it has a parent using this method. If it has a parent, then nothing in 
     * the function will happen. If the object does not have a parent, then a parent will be created so that 
     * all future slices can be children under the parent (so slices stay together while on board and so that all slices can be grbbed together)
    */
    Transform checkForParent(GameObject childTransform)
    {
        bool hasParent = childTransform.transform.parent;
        Debug.Log("Parent - " + childTransform.transform.parent);
        Debug.Log("Has Parent - " + hasParent);
        Transform parent = null;    //this should never be null (but compiler still throws error if it is not assigned as null by default)

        // TODO -- figure out how to add to new parent if old parent is not parent prefab
        if(!hasParent)
        {

            // Debug.Log("No Parent");
            //since I can not manually add the 2 necessary scripts to the gameobject, I am instead just going 
            // to have a prefab of the parent which will be created when necessary and by default has the 2 
            // necessary grab scripts
            parent = Instantiate(parentObjPrefab).transform;
            parent.transform.position = childTransform.transform.position;
            Debug.Log("Added new parent");
        }
        else    //if it does have a parent
        {
            Debug.Log("Already has parent");
            parent = childTransform.transform.parent;
        }
        return parent;
    }




    IEnumerator removeBadCutMarker(GameObject marker)
    {
        yield return new WaitForSeconds(1f);
        Destroy(marker);
    }








































    // -----   Debug Methods   -----
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 endPosition = endSlicePoint.position;

        Gizmos.DrawSphere(endPosition, debugCircleRadius);
        Gizmos.DrawSphere(new Vector3(endPosition.x, endPosition.y - 0.1f, endPosition.z), debugCircleRadius);
    }



    void DebugKnifeAngle(Vector3 endPosition, Vector3 downVector, Vector3 bladeVector, float radianAngleBetweenVectors)
    {
        Debug.DrawRay(endPosition, downVector, Color.green);
        Debug.DrawRay(endPosition, bladeVector, Color.blue);

        Debug.DrawLine(endPosition, startSlicePoint.position, Color.red);

        // Debug.Log("Radian Angle from Vertical - " + radianAngleBetweenVectors);
        // Debug.Log("Degree Angle from Vertical - " + (Mathf.Rad2Deg * radianAngleBetweenVectors) );
    }
}
