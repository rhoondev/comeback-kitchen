using UnityEngine;
using System.Collections;


public class SpikeManager : MonoBehaviour
{
    // private void OnCollisionEnter(Collision collision)
    // {
    //     Debug.Log("On Collision Called");
    //     if(collision.gameObject.tag == "Sliceable")
    //     {
    //         Debug.Log("Sliceable Object Entered");
    //         if(collision.gameObject.name == "chicken_breast")
    //         {
    //             Debug.Log("Chicken");
    //             // TODO -- CHange later (avert your eyes if you are a programmer)
    //             // What you see below is potentially one of the worst things I have ever programmed, but I need to do this
    //             // chicken breast has 4 children and the last one has ther mesh and rigidbody and stuff
    //             GameObject gameobjectWithMesh = collision.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;

    //             Debug.Log("Collider Entered by Sliceable Chicken Breast");
    //             gameobjectWithMesh.GetComponent<Rigidbody>().useGravity = false;
    //         }
    //         else
    //         {
    //             Debug.Log("Not Chicken");
    //             Debug.Log("Collider Entered by Sliceable Object");
    //             // if it is sliceable, it will have a rigidbody and will have gravity
    //             collision.gameObject.GetComponent<Rigidbody>().useGravity = false;
    //         }
    //     }
    // }

    //Sliceable layer is layer 9


    // when an object enters the spikes, check if it has the tag of cuttable and if so, then snap it to the spikes
    void OnTriggerEnter(Collider other)
    {
        // Debug.Log("On Trigger Called");
        if(other.gameObject.layer == 9)
        {
        // DO NOT HAVE TO GET CHILD OBJECTS; CODE AUTOMATICALLY GETS OBJECT WITH COLLIDER (SAVES A TON OF TIME FOR ME)
        //     Debug.Log("Sliceable Object Entered");
        //     Debug.Log("Name: " + other.gameObject.name);
        //     if(other.gameObject.name == "chicken_breast")
        //     {
        //         Debug.Log("Chicken");
        //         // TODO -- CHange later (avert your eyes if you are a programmer)
        //         // What you see below is potentially one of the worst things I have ever programmed, but I need to do this
        //         // chicken breast has 4 children and the last one has ther mesh and rigidbody and stuff
        //         GameObject gameobjectWithMesh = other.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;

        //         Debug.Log("Collider Entered by Sliceable Chicken Breast");
        //         gameobjectWithMesh.GetComponent<Rigidbody>().useGravity = false;
        //     }
            Debug.Log("Collider Entered by Sliceable Object");
            // must also remove all the forces acting on the object from before
            StartCoroutine(slideCooldown(other));
        }
    }


    IEnumerator slideCooldown(Collider other)
    {
        Debug.Log("Coroutine Started");
        yield return new WaitForSeconds(0.7f);
        Debug.Log("Coroutine Ended");
        other.gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        other.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        // if it is sliceable, it will have a rigidbody and will have gravity, which we must remove
        other.gameObject.GetComponent<Rigidbody>().useGravity = false;
    }

    // void OnTriggerStay(Collider other)
    // {
    //     if(other.gameObject.GetComponent<Rigidbody>().linearVelocity != Vector3.zero)
    //     {
    //         other.gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
    //     }
    // }

    void OnTriggerExit(Collider other)
    {
        StopAllCoroutines();
        if(other.gameObject.layer == 9)
        {
            // Debug.Log("Sliceable Object Entered");
            // if(other.gameObject.name == "chicken_breast")
            // {
            //     Debug.Log("Chicken");
            //     // Same as above
            //     GameObject gameobjectWithMesh = other.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;

                
            //     Debug.Log("Sliceable Object Left the Collider");
            //     other.gameObject.GetComponent<Rigidbody>().useGravity = true;
            // }

            Debug.Log("Sliceable Object Left the Collider");
            // if it is sliceable, it will have a rigidbody and will have gravity (disabled if it is in the collider area)
            other.gameObject.GetComponent<Rigidbody>().useGravity = true;
        }
    }
}
