using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Collections;

public class InfiniteGrabSpawner : GrabInterceptor
{
    protected override void PerformGrabAction(XRBaseInteractor interactor)
    {
        // Instantiate a copy of this object
        GameObject clone = Instantiate(gameObject, transform.position, transform.rotation);

        // Make sure the clone does not create more clones
        Destroy(clone.GetComponent<InfiniteGrabSpawner>());

        // Make sure the clone will remove any constraints when it is grabbed
        clone.GetComponent<DynamicGrabInteractable>().EnterDynamicModeOnGrabbed = true;

        // Make sure the clone has a non-trigger collider
        clone.GetComponent<Collider>().isTrigger = false;

        // Wait until the end of frame to let the grab interaction proceed
        StartCoroutine(TransferGrabNextFrame(interactor, clone));

        // Invoke the grab event with a reference to the cloned object
        OnGrabbed.Invoke(clone.GetComponent<DynamicObject>());
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
