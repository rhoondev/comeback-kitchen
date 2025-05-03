// using UnityEngine;
// using UnityEngine.XR.Interaction.Toolkit.Interactables;
// using UnityEngine.XR.Interaction.Toolkit.Interactors;

// public class PlacementZone : MonoBehaviour
// {
//     [SerializeField] private XRSocketInteractor socketInteractor;
//     [SerializeField] private bool snapToSocket;
//     [SerializeField] private GameObject targetObject;

//     public SmartAction<GameObject> OnObjectEnter = new SmartAction<GameObject>();

//     public void SetTargetObject(GameObject targetObject)
//     {
//         this.targetObject = targetObject;
//     }

//     public void EnterObject(GameObject obj)
//     {
//         if (obj == targetObject)
//         {
//             if (obj.TryGetComponent<XRGrabInteractable>(out var interactable))
//             {
//                 ReleaseInteractable(interactable);
//             }

//             socketInteractor.enabled = true;

//             Debug.Log($"{obj.name} has entered {gameObject.name}.");

//             OnObjectEnter.Invoke(obj);
//         }
//     }
// }
