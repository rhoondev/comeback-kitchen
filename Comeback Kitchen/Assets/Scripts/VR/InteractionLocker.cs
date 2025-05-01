using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRBaseInteractable))]
public class InteractionLocker : MonoBehaviour
{
    private XRBaseInteractable _interactable;
    private InteractionLayerMask _normalInteractionLayers;

    private void Awake()
    {
        _interactable = GetComponent<XRBaseInteractable>();
        _normalInteractionLayers = _interactable.interactionLayers;
    }

    public void LockInteraction()
    {
        // Set the layer to a custom layer to disable interaction
        _interactable.interactionLayers = InteractionLayerMask.GetMask("Non-Interactable");
    }

    public void UnlockInteraction()
    {
        // Reset the layer to the original layer to enable interaction
        _interactable.interactionLayers = _normalInteractionLayers;
    }
}