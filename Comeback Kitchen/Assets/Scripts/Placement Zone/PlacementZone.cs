using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

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
            if (obj.TryGetComponent<XRGrabInteractable>(out var interactable))
            {
                ReleaseInteractable(interactable);
            }

            Debug.Log($"{obj.name} has entered {gameObject.name}.");

            OnObjectEnter.Invoke(obj);
        }
    }

    private void Snap(GameObject obj)
    {
        obj.transform.position = transform.position;
        obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    }

    private void ReleaseInteractable(XRBaseInteractable interactable)
    {
        var interactor = interactable.firstInteractorSelecting;

        if (interactor != null)
        {
            // Drop the object
            interactable.interactionManager.SelectExit(interactor, interactable);
        }
    }
}
