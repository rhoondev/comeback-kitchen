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

    public SmartAction<GameObject> OnObjectEnter = new SmartAction<GameObject>();

    public void SetTargetObject(GameObject targetObject)
    {
        this.targetObject = targetObject;
    }

    public void EnterObject(GameObject obj)
    {
        if (obj == targetObject)
        {
            if (enterAction == PlacementZoneEnterAction.Drop)
            {
                Drop(obj);
            }
            else if (enterAction == PlacementZoneEnterAction.Snap)
            {
                Snap(obj);
            }

            Debug.Log($"{obj.name} has entered {gameObject.name}.");

            OnObjectEnter.Invoke(obj);
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
