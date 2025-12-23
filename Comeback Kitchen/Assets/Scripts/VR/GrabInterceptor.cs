using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRGrabInteractable))]
public class GrabInterceptor : MonoBehaviour
{
    [field: SerializeField] public bool IsGrabbable { get; set; }

    protected XRGrabInteractable interactable;

    public SmartAction<DynamicObject> OnGrabbed = new SmartAction<DynamicObject>();
    public SmartAction<DynamicObject> OnGrabAttempt = new SmartAction<DynamicObject>();

    protected virtual void OnEnable()
    {
        interactable = GetComponent<XRGrabInteractable>();

        // Intercept the grab attempt
        interactable.selectEntered.AddListener(OnSelectEntering);
    }

    protected virtual void OnDisable()
    {
        // Stop listening when destroyed
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnSelectEntering);
        }
    }

    private void OnSelectEntering(SelectEnterEventArgs args)
    {
        if (args.interactorObject is XRBaseInteractor interactor)
        {
            // Release the grab
            interactable.interactionManager.SelectExit(interactor, (IXRSelectInteractable)interactable);

            // Check if the grab is allowed
            if (!IsGrabbable)
            {
                // Invoke the grab attempt event with a reference to the object
                OnGrabAttempt.Invoke(GetComponent<DynamicObject>());
                return;
            }

            PerformGrabAction(interactor);
        }
    }

    protected virtual void PerformGrabAction(XRBaseInteractor interactor) { }
}
