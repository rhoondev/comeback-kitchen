using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRBaseInteractable))]
public class InteractionLocker : MonoBehaviour
{
    [SerializeField] private bool lockOnAwake;

    private XRBaseInteractable _interactable;
    private InteractionLayerMask _normalInteractionLayers;

    private void Awake()
    {
        _interactable = GetComponent<XRBaseInteractable>();
        _normalInteractionLayers = _interactable.interactionLayers;

        if (lockOnAwake)
        {
            LockInteraction();
        }
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