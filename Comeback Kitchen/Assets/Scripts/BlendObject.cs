using UnityEngine;
using System.Collections;
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


// TODO -- make liquid level of blender be proportional to the size of the slice and the necessary amount of fruit that needs to be destroyed





public class BlendObject : MonoBehaviour
{


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

    bool angledUp = false;

    [SerializeField] private Liquid blenderLiquidScript;

    private bool coroutineRunning = false;
    Coroutine runningBlenderBlockRoutine;


    private bool firstCutHappened = false;

    static public bool bladesSpinning = false;


    
    private float radius = 0.03f;
    private Vector3 center;




    void Awake()
    {
        center = this.gameObject.transform.position;
    }



    void FixedUpdate()
    {
        
        // StopCoroutine(runningBlenderBlockRoutine);


        // Debug.Log("Routine Name - " + runningBlenderBlockRoutine);
        // For future, make list of all points that knife goes through in an object. If the knife stops touching object, 
        //  take last pair of points in that list and create a plane with the first pair of points in that object
        // Also make new variable (prevHit) which is true if hasHit was true and say if(prevHit == true and hasHit != true) 
        //  ->  make a plane (Transform) and use .position and .up on the transform to get the correct info (vector and position) for the slice function

        bool hasHit = Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hit, sliceableLayer);

        //Only do stuff if the blades are spinnning
        if(bladesSpinning)
        {
            Debug.Log("Blade Spinning");
            
            if(hasHit)        //if it hit an object, proceed
            {

                GameObject target = hit.transform.gameObject;

                // if this object is sliceable/not on cooldown, then proceed
                if(!target.GetComponent<SliceTimer>().getRecentlySliced())
                {

                    // if the first cut has not already happened, then let the rest of the script know it has happened
                    // This is important since the first-cut-var being true allows the coroutine of deleting-everything-after-no-cuts to happen
                    if(!firstCutHappened)
                        firstCutHappened = true;

                    Debug.Log("Coroutine before kill: " + runningBlenderBlockRoutine);
                    // StopCoroutine("destroyBlockingBlenderObjects()");
                    coroutineRunning = false;
                    StopCoroutine(runningBlenderBlockRoutine);
                    Debug.Log("Coroutine after kill: " + runningBlenderBlockRoutine);

                    // Debug.Log("Hit a sliceable object!!!");
                    Slice(target);
                }
            }
            else if(!coroutineRunning && firstCutHappened)
            {
                coroutineRunning = true;
                runningBlenderBlockRoutine = StartCoroutine(destroyBlockingBlenderObjects());
            }
        }

        //if the blades are not spininng, then do nothing
            //and/or if the blender has not hit an object this frame and a coroutine is running, then do nothing and just let the other coroutine run
            //and/or if the blender has never hit an object, then do not call a coroutine to run, aka do nothing also
        
        // ---- Blades Spinning ----

        //blades spinning, hit an object (that is sliceable/not on cooldown), and never hit an object before -> slice, set firstCutHappened to true, stop coroutine, & set coroutineRunning to false
        //blades spinning, hit an object (that is sliceable/not on cooldown), and has hit objects before -> slice, stop coroutine, & set coroutineRunning to false

        //blades spinning, did not hit an object, never hit an object, & no coroutine -> do nothing
        //blades spinning, did not hit an object, never hit an object, & coroutine running -> do nothing

        //blades spinning, did not hit an object, has previously hit an object & no coroutine -> start coroutine & set coroutineRunning to true
        //blades spinning, did not hit an object, has previously hit an object & coroutine running -> do nothing

        // ---- Blades Not Spinning ----

        //blades not spinning & hit an object -> do nothing
        //blades not spinning & did not hit an object -> do nothing


        if(!angledUp)
        {
            //Periodically angle the blade up if the blade is down
            sliceAngle.position += new Vector3(0, 0.01f, 0);
            angledUp = true;
        }
        else
        {
            //Periodically angle the blade down if the blade is up
            sliceAngle.position -= new Vector3(0, 0.01f, 0);
            angledUp = false;
        }

    }


    // This function cuts the object in 2 halfs (separated by the normal plane of a transform)
    public void Slice(GameObject target)
    {

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
        // tutorial here - https://youtu.be/GQzW6ZJFQ94?si=XouKykpD4ThyuScg

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
            bool destroyBottom = false;
            bool destroyTop = false;






            // Cross section material is material that is put on cut face once cut happens
            crossSectionMaterial = target.GetComponent<Renderer>().material;
            // Debug.Log("Name: " + target.gameObject.name);

            GameObject upperHull = hull.CreateUpperHull(target, crossSectionMaterial);
            // Vector3 upperSize = upperHull.GetComponent<Renderer>().bounds.size;         // Code R2
            // make upper slice behave with physics
            SetupSlicedComponent(upperHull, parentPos, out destroyTop);


            GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMaterial);
            // Vector3 lowerSize = lowerHull.GetComponent<Renderer>().bounds.size;         // Code R2
            // make lower slice behave with physics
            SetupSlicedComponent(lowerHull, parentPos, out destroyBottom);



            Destroy(target);
            blenderLiquidScript.Fill(1);

            if(blenderLiquidScript.FillCount == blenderLiquidScript.MaxFillCount)
                removeAllFromBlender();
            //for efficiency, I put the clearing-the-entire-blender method call in the slice function (this was so 
            // that the check for if the blender was full was not called in every frame)


            if(destroyTop)
            {
                Destroy(upperHull);
                blenderLiquidScript.Fill(5);
                // Debug.Log("Destroying Upper");
            }


            if(destroyBottom)
            {
                Destroy(lowerHull);
                blenderLiquidScript.Fill(5);
                // Debug.Log("Destroying Lower");
            }
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


    // This function adds a rigidbody, collider, and gives object a force away from slice, after the object is cut
    public void SetupSlicedComponent(GameObject slicedObject, Vector3 position, out bool destroyHull)
    {
        // let the object know that it has recently been cut and can not be cut for a bit (0.3f seconds as of now)
        SliceTimer cut = slicedObject.AddComponent<SliceTimer>();
        StartCoroutine(cut.startSliceTimer());




        // make sliced object have physics and stuff
        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
        // rb.useGravity = false;

        //make sliced object be able to interact with environment
        // MeshCollider collider = slicedObject.AddComponent<MeshCollider>();   //optimization 2 (removing it)
        slicedObject.AddComponent<BoxCollider>(); // optimization 2
        // rb.useGravity = true;


        // Code R2
        // make cut object cutable again as long as it is big enough
        Vector3 size = slicedObject.GetComponent<Renderer>().bounds.size;
        if(size.x >= sizeLimit && size.y >= sizeLimit && size.z >= sizeLimit)
        {
            slicedObject.layer = sliceLayerNum;
            destroyHull = false;
        }
        else
        {
            //TODO -- maybe put the destroy function here to optimize (bunch of stuff happens below that may not be necessary)
            destroyHull = true;
        }
        // mesh colliders need to be convex when used with a rigidbody
        // collider.convex = true;          // optimization 2 (removing it)


        // Next 2 lines of code are super important for chicken breasts and any other mesh that has children
        // change the created object's position if necessary (if a position is entered/0,0,0 will be sent if not necessary)
        if(position != Vector3.zero)
            slicedObject.transform.position = position;

        
        // slicedObject.transform.rotation;
        



        // add an initial force to each object
        if(!rb.isKinematic)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }


        // just apply a force to the object that is straight up
        rb.AddForce(new Vector3(0, 1, 1) * cutForce, ForceMode.Impulse);
    }



    public IEnumerator destroyBlockingBlenderObjects()
    {
        yield return new WaitForSeconds(3f);
        Debug.Log("Coroutine Gonna Destroy Everything");
        Collider[] colliders = Physics.OverlapSphere(center, radius, sliceableLayer);
        foreach (Collider collider in colliders)
        {
            Destroy(collider.gameObject);
            blenderLiquidScript.Fill(5);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(center, radius); //DrawWireSphere(center, radius);
    }



    // This method just removes basically everything in the blender (only called when the blender is full)
    void removeAllFromBlender()
    {
        
        Collider[] colliders = Physics.OverlapSphere(center, radius + 0.05f, sliceableLayer);
        foreach (Collider collider in colliders)
        {
            Destroy(collider.gameObject);
            blenderLiquidScript.Fill(5);
        }
        
        // TODO -- if you want the blender to auto-stop, then put it here (you probably do not tho since you want the user to do it)
    }


}
