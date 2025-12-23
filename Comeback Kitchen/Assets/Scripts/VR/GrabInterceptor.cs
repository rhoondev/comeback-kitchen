using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRGrabInteractable))]
public class GrabInterceptor : MonoBehaviour
{
    [field: SerializeField] public bool IsGrabbable { get; set; }
    [SerializeField] private bool alwaysExitOnIntercept = false;

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
            if (alwaysExitOnIntercept || !IsGrabbable)
            {
                // Release the grab
                interactable.interactionManager.SelectExit(interactor, (IXRSelectInteractable)interactable);
            }

            // Check if the grab is allowed
            if (IsGrabbable)
            {
                PerformGrabAction(interactor);
            }
            else
            {
                // Invoke the grab attempt event with a reference to the object
                OnGrabAttempt.Invoke(GetComponent<DynamicObject>());
            }
        }
    }

    // Default grab action simply invokes the grab event
    protected virtual void PerformGrabAction(XRBaseInteractor interactor)
    {
        OnGrabbed.Invoke(GetComponent<DynamicObject>());
    }
}
