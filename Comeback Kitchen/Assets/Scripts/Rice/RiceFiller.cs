using UnityEngine;

public class RiceFiller : MonoBehaviour
{
    [SerializeField] private Container container;

    // private Pan _pan = null;

    // private void Awake()
    // {
    //     if (TryGetComponent(out Pan pan))
    //     {
    //         _pan = pan;
    //     }
    // }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Rice _))
        {
            GameObject rice = container.RestoreObject();

            // if (rice != null && _pan != null)
            // {
            //     _pan.Contents.Add(rice.GetComponent<Rigidbody>());
            // }

            Destroy(other.gameObject);
        }
    }
}