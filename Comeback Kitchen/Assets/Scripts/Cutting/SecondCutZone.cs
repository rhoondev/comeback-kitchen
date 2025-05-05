using UnityEngine;

public class SecondCutZone : MonoBehaviour
{

    public SmartAction OnObjectEnter = new SmartAction();

    private string parentTag = "sliceParent";


    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.CompareTag(parentTag))
        {
            //Remove all children and destroy grab-parent-thingy
            other.transform.DetachChildren();
            
            OnObjectEnter.Invoke();              // bad (could be called multiple times so must fix potentially later)
            Destroy(other.gameObject);
        }
        else if(other.gameObject.layer == 10)       //10 is the Sliceable layer
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.None;
            rb.AddForce(Vector3.up, ForceMode.Impulse);
        }
    }

}
