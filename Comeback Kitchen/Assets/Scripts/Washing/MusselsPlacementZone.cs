using UnityEngine;

public class MusselsPlacementZone : MonoBehaviour
{
    public SmartAction OnMusselsPlacedOnCounter = new SmartAction();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Mussels Strainer")
        {
            // TODO: Release the strainer from the player's hand and snap it to the snap point

            // Placeholder code
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = true;

            OnMusselsPlacedOnCounter.Invoke();
            Debug.Log("Mussels placed on counter!");
            gameObject.SetActive(false); // Hide the mussels placement zone after placing the strainer
        }
    }
}
