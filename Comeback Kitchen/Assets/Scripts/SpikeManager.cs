using UnityEngine;





public class SpikeManager : MonoBehaviour
{

    //Sliceable layer is layer 10
    private const int sliceableNum = 10;
    private const int sliceableParentNum = 31;
    [SerializeField] private Transform spotOnSpikes;



    // when an object enters the spikes, check if it has the tag of cuttable and if so, then snap it to the spikes
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == sliceableNum)
        {
            Debug.Log("Collider Entered by Sliceable Object so snapping to position");
            other.transform.position = spotOnSpikes.position;
            other.GetComponent<Rigidbody>().isKinematic = true;
            Debug.Log("Snapped to spot");
            // must also remove all the forces acting on the object from before
        }
    }


    // IEnumerator slideCooldown(Collider other)
    // {
    //     // Debug.Log("Coroutine Started");
    //     yield return new WaitForSeconds(0.3f);
    //     // Debug.Log("Coroutine Ended");
    //     other.gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
    //     other.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    //     // if it is sliceable, it will have a rigidbody and will have gravity, which we must remove
    //     other.gameObject.GetComponent<Rigidbody>().useGravity = false;
    // }



    // TODO -- test with VR to make sure OnTriggerExit actually works
    void OnTriggerExit(Collider other)
    {
        // StopAllCoroutines();
        if(other.gameObject.layer == sliceableParentNum)
        {

            Debug.Log("Sliceable Parent Object Left the Collider");

            foreach (Transform child in other.transform)
            {
                child.SetParent(null); // Unparent

                Rigidbody rb = child.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                }
            }
            Debug.Log("All children removed from parent");

            //Destroy parent object that was holding the pieces together
            Destroy(other);
            Debug.Log("Parent Destroyed");


            // if it is sliceable, it will have a rigidbody and will have gravity (disabled if it is in the collider area)
            // Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
            // if(!rb.isKinematic)
            //     rb.useGravity = true;
        }
    }
}
