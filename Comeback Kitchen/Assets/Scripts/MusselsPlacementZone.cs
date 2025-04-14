using UnityEngine;

public class MusselsPlacementZone : MonoBehaviour
{
    public SmartAction OnMusselsPlacedOnCounter = new SmartAction();

    private bool _musselsPickedUp = false;

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Mussels Strainer")
        {
            _musselsPickedUp = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_musselsPickedUp && other.gameObject.name == "Mussels Strainer" && other.transform.GetChild(0).TryGetComponent(out Washable washable))
        {
            if (washable.IsWashed)
            {
                // TODO: Release the strainer from the player's hand and snap it to the snap point

                // Placeholder code
                Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.useGravity = true;

                OnMusselsPlacedOnCounter.Invoke();
                Debug.Log("Mussels placed on counter!");
            }
            else
            {
                Debug.Log($"You must wash the mussels before putting them on the counter!");
            }
        }
    }
}
