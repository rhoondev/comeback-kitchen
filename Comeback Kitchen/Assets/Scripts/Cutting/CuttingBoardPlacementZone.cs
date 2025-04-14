using System.Collections.Generic;
using UnityEngine;

public class CuttingBoardPlacementZone : MonoBehaviour
{
    public SmartAction OnVegetableAdded = new SmartAction();

    private List<GameObject> _vegetablesOnBoard = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Vegetable"))
        {
            // TODO: Release the object from the player's hand

            // Placeholder code
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = true;

            _vegetablesOnBoard.Add(other.gameObject);
            OnVegetableAdded.Invoke();
            Debug.Log($"Vegetable added to cutting board: {other.gameObject.name}");
        }
    }
}
