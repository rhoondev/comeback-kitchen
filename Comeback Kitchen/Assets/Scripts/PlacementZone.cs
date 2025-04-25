using System.Collections.Generic;
using UnityEngine;

public enum PlacementZoneEnterAction
{
    Drop,
    Snap
}

public class PlacementZone : MonoBehaviour
{
    [SerializeField] private PlacementZoneEnterAction enterAction;
    [SerializeField] private GameObject targetObject;

    public SmartAction OnObjectEnter = new SmartAction();

    public void SetTargetObject(GameObject targetObject)
    {
        this.targetObject = targetObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == targetObject)
        {
            if (enterAction == PlacementZoneEnterAction.Drop)
            {
                Drop(other.gameObject);
            }
            else if (enterAction == PlacementZoneEnterAction.Snap)
            {
                Snap(other.gameObject);
            }

            Debug.Log($"{other.gameObject.name} has entered {gameObject.name}.");
            OnObjectEnter.Invoke();
        }
    }

    private void Drop(GameObject obj)
    {
        // TODO: Release the object from the player's hand in VR

        if (obj.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }

    private void Snap(GameObject obj)
    {
        // TODO: Release the object from the players hand in VR

        obj.transform.position = transform.position;
    }
}
