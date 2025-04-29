using UnityEngine;

//ChatGPT
public class Spike : MonoBehaviour
{
    public SmartAction OnObjectEnter = new SmartAction();
    private void OnTriggerEnter(Collider other)
    {
        SliceableObject sliceable = other.GetComponent<SliceableObject>();
        if (sliceable != null)
        {
            sliceable.Pin();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        SliceableObject sliceable = other.GetComponent<SliceableObject>();
        if (sliceable != null)
            sliceable.Unpin();
    }
}
