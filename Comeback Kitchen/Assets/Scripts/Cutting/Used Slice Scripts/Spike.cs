using UnityEngine;

//ChatGPT
public class Spike : MonoBehaviour
{
    [SerializeField] GameObject grabbableParentPrefab;


    public SmartAction OnObjectEnter = new SmartAction();
    public SmartAction OnObjectExit = new SmartAction();

    private string parentTag = "sliceParent";


    private void OnTriggerEnter(Collider other)
    {
        SliceableObject sliceable = other.GetComponent<SliceableObject>();
        if (sliceable != null)
        {
            sliceable.Pin();

            //Make sure that the object in the collider either has a parent-grab-thingy as its parent or is the parent-grab-thingy
            // if it does not, then add the parent-grab-thingy and give it the correct tag
            if(!other.transform.CompareTag(parentTag) && !other.transform.parent.CompareTag(parentTag))
            {
                GameObject grabbableParent = Instantiate(grabbableParentPrefab, other.transform.parent);
                grabbableParent.tag = parentTag;
                grabbableParent.transform.position = other.transform.position;
                other.transform.parent = grabbableParent.transform;
                other.transform.localPosition = Vector3.zero;
            }

            
            OnObjectEnter.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // SliceableObject sliceable = other.GetComponent<SliceableObject>();
        // if (sliceable != null)
        // {
        //     sliceable.Unpin();
        //     OnObjectExit.Invoke();
        // }
        if(other.transform.CompareTag(parentTag))
        {
            //Remove all children and destroy grab-parent-thingy
            other.transform.DetachChildren();
            
            OnObjectExit.Invoke();              // bad (could be called multiple times so must fix potentially later)
        }
        else if(other.gameObject.layer == 10)       //10 is the Sliceable layer
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.None;
            rb.AddForce(Vector3.up, ForceMode.Impulse);
        }

    }
}
