using UnityEngine;

public class SpikeManager : MonoBehaviour
{
    // when an object enters the spikes, check if it has the tag of cuttable and if so, then snap it to the spikes
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Sliceable")
        {
            if(other.gameObject.name == "chicken_breast")
            {
                // TODO -- CHange later (avert your eyes if you are a programmer)
                // What you see below is potentially one of the worst things I have ever programmed, but I need to do this
                // chicken breast has 4 children and the last one has ther mesh and rigidbody and stuff
                GameObject gameobjectWithMesh = other.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;

                Debug.Log("Collider Entered by Sliceable Chicken Breast");
                gameobjectWithMesh.GetComponent<Rigidbody>().useGravity = false;
            }
            else
            {
                Debug.Log("Collider Entered by Sliceable Object");
                // if it is sliceable, it will have a rigidbody and will have gravity
                other.gameObject.GetComponent<Rigidbody>().useGravity = false;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Sliceable")
        {
            if(other.gameObject.name == "chicken_breast")
            {
                // Same as above
                GameObject gameobjectWithMesh = other.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;

                
                Debug.Log("Sliceable Object Left the Collider");
                other.gameObject.GetComponent<Rigidbody>().useGravity = true;
            }
            else
            {
                Debug.Log("Sliceable Object Left the Collider");
                // if it is sliceable, it will have a rigidbody and will have gravity (disabled if it is in the collider area)
                other.gameObject.GetComponent<Rigidbody>().useGravity = true;
            }
        }
    }
}
