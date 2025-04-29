using UnityEngine;

//ChatGPT
public class Spike : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        SliceableObject sliceable = other.GetComponent<SliceableObject>();
        if (sliceable != null)
            sliceable.isPinned = true;
    }

    private void OnTriggerExit(Collider other)
    {
        SliceableObject sliceable = other.GetComponent<SliceableObject>();
        if (sliceable != null)
            sliceable.Unpin();
    }
}
