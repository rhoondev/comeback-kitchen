using System.Collections.Generic;
using UnityEngine;

public class Strainer : MonoBehaviour
{
    private List<Washable> _contents = new List<Washable>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Washable washable))
        {
            HandleWashable(washable);
        }
    }

    private void HandleWashable(Washable washable)
    {
        if (washable.IsWashed)
        {
            if (!_contents.Contains(washable))
            {
                // Release the object from the palyer's hand
                Rigidbody rb = washable.GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.useGravity = true;

                _contents.Add(washable);

                Debug.Log("Washed object added to strainer");
            }
        }
        else
        {
            Destroy(washable.gameObject);

            // Show error message
            Debug.Log("Unwashed object deleted");
        }
    }
}
