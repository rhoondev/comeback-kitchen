using UnityEngine;
using EzySlice;


/* 
For future reference, if a mesh model is added, 
the model must be set to have a convex mesh collider, 
and also the readwrite setting must be checked so 
the blade cutting script can make 2 new models from 
the old model (this script needs to be able to read 
model's data)
*/


// Ctrl + f for these codes if errors occur
// Removal Code R1 - remove if an issue happens immediately after cutting (physics break or something / knife stops moving)
// Removal Code R1 - remove if an issue happens where pieces can not be cut since they are too small


// Done -- have objects become child objects of a single parent after they are cut

// Done -- make objects stick together after being cut

// Done -- make objects keep track of original object size (could make variable on parent object)

// TODO -- make the original size o object impact something

// TODO -- objects move apart slightly after being sliced

// TODO -- objects can not be sliced when not on cutting board's spikes

// TODO -- implement better way to slice (entry and exit points to find plane)


public class KnifeSliceScript : MonoBehaviour
{

    [SerializeField] private GameObject parentObjPrefab;

    private const int sliceLayerNum = 10;
    [SerializeField] private float sizeLimit;


    // bottom of the blade (by handle)
    [SerializeField] private Transform startSlicePoint;

    // top of the blade (pointy end)
    [SerializeField] private Transform endSlicePoint;
    [SerializeField] private Transform sliceAngle;
    [SerializeField] private LayerMask sliceableLayer;

    // public Transform planeDebug;
    // public GameObject target;
    private Material crossSectionMaterial;
    [SerializeField] private float cutForce;

    [SerializeField] private Vector3 originalObjectSize;




    

    void FixedUpdate()
    {
        // For future, make list of all points that knife goes through in an object. If the knife stops touching object, 
        //  take last pair of points in that list and create a plane with the first pair of points in that object
        // Also make new variable (prevHit) which is true if hasHit was true and say if(prevHit == true and hasHit != true) 
        //  ->  make a plane (Transform) and use .position and .up on the transform to get the correct info (vector and position) for the slice function

        bool hasHit = Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hit, sliceableLayer);
        if(hasHit)
        {
            // Debug.Log("Hit a sliceable object!!!");
            GameObject target = hit.transform.gameObject;
            // Debug.Log("Object's Name: " + target.name);
            //if te gameobject was not sliced recently, then do not slice it quite yet (wait until it tries to get cut again)
            if(!target.GetComponent<SliceTimer>().getRecentlySliced())
                Slice(target);
        }
        // else if(prevHit == true)      // do not have to put hasHit != true bc it will always be false when it gets here
        // {
        //     // makePlaneVector();
        // }
    }








    /* 
     * Gameobject will check to see if it has a parent using this method. If it has a parent, then nothing in 
     * the function will happen. If the object does not have a parent, then a parent will be created so that 
     * all future slices can be children under the parent (so slices stay together while on board and so that all slices can be grbbed together)
    */
    Transform checkForParent(GameObject childTransform)
    {
        bool hasParent = childTransform.transform.parent;
        Transform parent = null;    //this should never be null (but conpiler still throws error if it is not assigned as null by default)

        if(!hasParent || transform.parent.name != "Parent Obj")
        {
            // Debug.Log("No Parent");
            //since I can not manually add the 2 necessary scripts to the gameobject, I am instead just going 
            // to have a prefab of the parent which will be created when necessary and by default has the 2 
            // necessary grab scripts
            parent = Instantiate(parentObjPrefab).transform;
            parent.transform.position = childTransform.transform.position;



            // First cut of an object should be the only cut where there is no parent named Parent Obj, 
            // so we can safely assume that this object is the first object (and largest size object will be) 
            originalObjectSize = childTransform.GetComponent<Renderer>().bounds.size;;
        }
        else if(transform.parent.name == "Parent Obj")
        {
            parent = childTransform.transform.parent;
            // Debug.Log("Has Parent: " + parent);
        }
        return parent;
    }








    // This function cuts the object in 2 halfs (separated by the normal plane of a transform)
    public void Slice(GameObject target)
    {

        // get the knife's rigidbody (knife's physics)
        Rigidbody rb = this.gameObject.GetComponent<Rigidbody>();
        // Code R1
        //disable gravity and forces for the object doing the slicing so that weird physics doesn't happen
        if(!rb.isKinematic)
        {
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Debug.Log("Target: " + target);
        // Can get plane of cut by getting cross product of vector of the length of the knife with the vector of the direction of movement of the knife
        // helpful video here - https://youtu.be/GQzW6ZJFQ94?si=XouKykpD4ThyuScg

        // get the Vector of the knife's edge (angle from top of knife to blade, but at the end of the knife/point)
        Vector3 attackAngle = sliceAngle.position - endSlicePoint.position;

        // makes a vector which is the normal vector for a plane (perpendicular to plane)
        Vector3 planeNormal = Vector3.Cross(endSlicePoint.position - startSlicePoint.position, attackAngle);
        // normalize the vector
        planeNormal.Normalize();

        // Slice method from assets works by sending in a position and a plane to cut through that position (plane dimensions matter)
        // hull is a name for the the magic that the slicing class does

        // ---- To make a plane in unity, you need the normal of the plane, followed by a point on that plane ----
        SlicedHull hull = target.Slice(endSlicePoint.position, planeNormal);


        if(hull != null)
        {
            // Debug.Log("Slice!!!");


            // Vector3 sizeNum = target.GetComponent<Renderer>().bounds.size;
            // Debug.Log("Bounding Box Size - " + sizeNum.x + ", " + sizeNum.y + ", " + sizeNum.z);
            // Output -- Bounding Box Size - 0.09410773, 0.05836695, 0.08301699


            Vector3 parentPos = target.transform.position;
            Debug.Log("Target - " + target);
            Transform parentInfo = checkForParent(target);




            // Cross section material is material that is put on cut face once cut happens
            crossSectionMaterial = target.GetComponent<Renderer>().material;
            // Debug.Log("Name: " + target.gameObject.name);

            GameObject upperHull = hull.CreateUpperHull(target, crossSectionMaterial);
            Vector3 upperSize = upperHull.GetComponent<Renderer>().bounds.size;         // Code R2
            // make upper slice behave with physics
            SetupSlicedComponent(upperHull, parentPos, parentInfo);

            // TODO -- test if object once sliced keeps the same parent as the previous slices (I do not think so but idk)

            GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMaterial);
            Vector3 lowerSize = lowerHull.GetComponent<Renderer>().bounds.size;         // Code R2
            // make lower slice behave with physics
            SetupSlicedComponent(lowerHull, parentPos, parentInfo);

            // Code R2
            if( (upperSize.x >= sizeLimit && upperSize.y >= sizeLimit && upperSize.z >= sizeLimit) && (lowerSize.x >= sizeLimit && lowerSize.y >= sizeLimit && lowerSize.z >= sizeLimit) )
                Destroy(target);

            //Optimization 1
            // Debug.Log("Can Slice Again");
        }

        // Code R1
        //make the object get it's gravity back and set the forces to zero so the object does not get the explosive force generated by the cut
        if(!rb.isKinematic)
        {
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }






    // This function adds a rigidbody and a collider                                 , and gives object a force away from slice, after the object is cut
    public void SetupSlicedComponent(GameObject slicedObject, Vector3 position, Transform parentInfo)
    {
        // let the object know that it has recently been cut and can not be cut for a bit (0.3f seconds as of now)
        SliceTimer cut = slicedObject.AddComponent<SliceTimer>();
        StartCoroutine(cut.startSliceTimer());

        

        // make the sliced object a child of a single parent object that can be grabbed
        slicedObject.transform.SetParent(parentInfo);




        // make sliced object have physics and stuff
        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();                                                               //     <---------------------
        // rb.useGravity = false;

        //make sliced object be able to interact with environment
        // MeshCollider collider = slicedObject.AddComponent<MeshCollider>();   //optimization 2 (removing it)
        slicedObject.AddComponent<BoxCollider>(); // optimization 2
        // rb.useGravity = true;




        // make sure the object does not move after being sliced and are basically locked in plane next to the parent object
        rb.useGravity = false;
        rb.isKinematic = true;




        // Code R2
        // make cut object cutable again as long as it is big enough
        Vector3 size = slicedObject.GetComponent<Renderer>().bounds.size;
        if(size.x >= sizeLimit && size.y >= sizeLimit && size.z >= sizeLimit)
        {
            slicedObject.layer = sliceLayerNum;
        }
        // mesh colliders need to be convex when used with a rigidbody
        // collider.convex = true;          // optimization 2 (removing it)


        // Next 2 lines of code are super important for chicken breasts and any other mesh that has children
        // change the created object's position if necessary (if a position is entered/0,0,0 will be sent if not necessary)
        if(position != Vector3.zero)
            slicedObject.transform.position = position;

        



        


        // if(!rb.isKinematic)                                                                                  //     <---------------------
        // {
        //     rb.linearVelocity = Vector3.zero;
        //     rb.angularVelocity = Vector3.zero;
        //     rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        // }


            // just apply a force to the object that is straight up
            // rb.AddForce(new Vector3(0, 1, 1) * cutForce, ForceMode.Impulse);
        // rb.AddExplosionForce(cutForce, endSlicePoint.position, 1);
    }


}
