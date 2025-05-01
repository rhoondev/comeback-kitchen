using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Collections;

[RequireComponent(typeof(XRGrabInteractable))]
public class InfiniteGrabSpawner : MonoBehaviour
{
    [field: SerializeField] public bool IsGrabbable { get; set; }

    private XRGrabInteractable _grabInteractable;

    public SmartAction<DynamicObject> OnGrabbed = new SmartAction<DynamicObject>();
    public SmartAction<DynamicObject> OnGrabAttempt = new SmartAction<DynamicObject>();

    private void Awake()
    {
        _grabInteractable = GetComponent<XRGrabInteractable>();

        // Intercept the grab attempt
        _grabInteractable.selectEntered.AddListener(OnSelectEntering);
    }

    private void OnDestroy()
    {
        // Stop listening when destroyed (important because clones are destroyed when grabbed)
        _grabInteractable.selectEntered.RemoveListener(OnSelectEntering);
    }

    private void OnSelectEntering(SelectEnterEventArgs args)
    {
        if (args.interactorObject is XRBaseInteractor interactor)
        {
            // Check if the grab is allowed
            if (!IsGrabbable)
            {
                // Invoke the grab attempt event with a reference to the spawner object
                OnGrabAttempt.Invoke(GetComponent<DynamicObject>());
                return;
            }

            // Instantiate a copy of this object
            GameObject clone = Instantiate(gameObject, transform.position, transform.rotation);

            // Make sure the clone does not create more clones
            Destroy(clone.GetComponent<InfiniteGrabSpawner>());

            // Make sure the clone will remove any constraints when it is grabbed
            clone.GetComponent<RemoveConstraintsOnGrab>().enabled = true;

            // Make sure the clone has a non-trigger collider
            clone.GetComponent<Collider>().isTrigger = false;

            // Wait until the end of frame to let the grab interaction proceed
            StartCoroutine(TransferGrabNextFrame(interactor, clone));

            // Invoke the grab event with a reference to the cloned object
            OnGrabbed.Invoke(clone.GetComponent<DynamicObject>());
        }
    }

    private IEnumerator TransferGrabNextFrame(XRBaseInteractor interactor, GameObject clone)
    {
        // Wait one frame to ensure everything is initialized
        yield return null;

        // Transfer the grab to the new object
        IXRSelectInteractable cloneGrabInteractable = clone.GetComponent<XRGrabInteractable>();
        interactor.interactionManager.SelectEnter(interactor, cloneGrabInteractable);
    }
}
