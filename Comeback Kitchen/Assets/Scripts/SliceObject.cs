using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;


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



public class SliceObject : MonoBehaviour
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
            Slice(target);
        }
        // else if(prevHit == true)      // do not have to put hasHit != true bc it will always be false when it gets here
        // {
        //     // makePlaneVector();
        // }
    }


    // This function cuts the object in 2 halfs (separated by the normal plane of a transform)
    public void Slice(GameObject target)
    {
        // Code R1
        //disable gravity and forces for the object doing the slicing so that weird physics doesn't happen
        this.gameObject.GetComponent<Rigidbody>().useGravity = false;
        this.gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        this.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

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
                                            // Vector3 s = startSlicePoint.position;
                                            // Vector3 e = endSlicePoint.position;

        // ---- To make a plane in unity, you need the normal of the plane, followed by a point on that plane ----
        SlicedHull hull = target.Slice(endSlicePoint.position, planeNormal);

                                            // Debug.DrawRay(e, attackAngle, Color.red); // Draws Line from endSlicePoint and in direction of the attackAngle
                                            // Debug.DrawLine(s, e, Color.magenta); // Draws Line from start of bladde cutting zone to end of blade cutting zone
                                    
                                            // Vector from start of knife to end of knife (difference vector)
                                            // Vector3 es = e-s;
                                            // Debug.Log(es);
                                            // Debug.Log("Plane Normal: " + planeNormal);
                                            // Debug.DrawRay( es , new Vector3(0, 15, 0), Color.green);
                                            // Debug.DrawRay( e, new Vector3(0, 15, 0), Color.green);
                                            // Debug.DrawRay(endSlicePoint.position, planeNormal, Color.cyan);

                                            // Debug.DrawLine(planeNormal, endSlicePoint.position, Color.blue);
                                            // Debug.Log("Hull: " + hull);
                                            // Debug.DrawLine(endSlicePoint.position, startSlicePoint.position, Color.blue);
                                            // Debug.DrawLine();

        if(hull != null)
        {
            // Debug.Log("Slice!!!");


            // Vector3 sizeNum = target.GetComponent<Renderer>().bounds.size;
            // Debug.Log("Bounding Box Size - " + sizeNum.x + ", " + sizeNum.y + ", " + sizeNum.z);
            // Output -- Bounding Box Size - 0.09410773, 0.05836695, 0.08301699


            GameObject parentObj = target;
            Vector3 parentPos = target.transform.position;

        

                                    // This section is to make sure that the new objects are put in the correct postion, rather than their 
                                    // original position (if mesh was a child of parent, position is in relation to parent, so when the 
                                    // mesh is split, the object gets put at child coords but has no parent with same coords, therefore, 
                                    // being put in the totally wrong spot)
                                    // Debug.Log("Sliced Name: " + target.transform.name);

                                    // Debug.Log("Object: " + parentObj);
                                    // Vector3 parentPos = Vector3.zero;
                                    // while(parentObj.transform.parent != null)
                                    // {
                                    //     Debug.Log("Parent: " + parentObj);
                                    //     parentObj = parentObj.transform.parent.gameObject;
                                    //     parentPos = parentObj.transform.position;
                                    // }







            // Cross section material is material that is put on cut face once cut happens
            crossSectionMaterial = target.GetComponent<Renderer>().material;
            // Debug.Log("Name: " + target.gameObject.name);

            GameObject upperHull = hull.CreateUpperHull(target, crossSectionMaterial);
            Vector3 upperSize = upperHull.GetComponent<Renderer>().bounds.size;         // Code R2
            // make upper slice behave with physics
            SetupSlicedComponent(upperHull, parentPos);


            GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMaterial);
            Vector3 lowerSize = lowerHull.GetComponent<Renderer>().bounds.size;         // Code R2
            // make lower slice behave with physics
            SetupSlicedComponent(lowerHull, parentPos);

            // Code R2
            if( (upperSize.x >= sizeLimit && upperSize.y >= sizeLimit && upperSize.z >= sizeLimit) && (lowerSize.x >= sizeLimit && lowerSize.y >= sizeLimit && lowerSize.z >= sizeLimit) )
                Destroy(parentObj);
            else
            {
                Destroy(upperHull);
                Destroy(lowerHull);
                // TODO -- Send a message to the user so that they know that the cut created piece(s) that were too small
                Debug.Log("Pieces were too small, so no cut was created");
            }            
        }

        // Code R1
        //make the object get it's gravity back and set the forces to zero so the object does not get the explosive force generated by the cut
        this.gameObject.GetComponent<Rigidbody>().useGravity = true;
        this.gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        this.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }


    // This function adds a rigidbody, collider, and gives object a force away from slice, after the object is cut
    public void SetupSlicedComponent(GameObject slicedObject, Vector3 position)
    {
        // make sliced object have physics and stuff
        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
        // rb.useGravity = false;

        //make sliced object be able to interact with environment
        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();
        // rb.useGravity = true;

        // make cut object cutable again
        slicedObject.layer = sliceLayerNum;

        // mesh colliders need to be convex when used with a rigidbody
        collider.convex = true;


        // change the created object's position if necessary (if a position is entered/0,0,0 will be sent if not necessary)
        if(position != Vector3.zero)
            slicedObject.transform.position = position;
        



        // add an initial force to each object
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        
        rb.AddExplosionForce(cutForce, slicedObject.transform.position, 1);
    }





                                    // public Vector3 makePlaneVector(Vector3 point1, Vector3 point2, Vector3 point3)
                                    // {
                                    //     //

                                    //     return new Vector3();
                                    // }

}
