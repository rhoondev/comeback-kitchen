using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class RemoveConstraintsOnGrab : MonoBehaviour
{
    private XRGrabInteractable _grabInteractable;
    private Rigidbody _rb;

    private void OnEnable()
    {
        _grabInteractable = GetComponent<XRGrabInteractable>();
        _rb = GetComponent<Rigidbody>();

        // Subscribe to the grab event
        _grabInteractable.selectEntered.AddListener(OnGrab);
    }

    // private void OnDestroy()
    // {
    //     // Always good practice to unsubscribe
    //     _grabInteractable.selectEntered.RemoveListener(OnGrab);
    // }

    private void OnGrab(SelectEnterEventArgs args)
    {
        _rb.constraints = RigidbodyConstraints.None;
    }
}
