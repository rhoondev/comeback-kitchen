using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

public class ZAxisClampGrabTransformer : XRBaseGrabTransformer
{
    [SerializeField] float minZ = 0f;
    [SerializeField] float maxZ = 30f;

    protected override RegistrationMode registrationMode => RegistrationMode.SingleAndMultiple;

    Vector3 m_InitialEulerRotation;

    public override void OnLink(XRGrabInteractable grabInteractable)
    {
        base.OnLink(grabInteractable);
        m_InitialEulerRotation = grabInteractable.transform.rotation.eulerAngles;
    }

    public override void Process(XRGrabInteractable grabInteractable, XRInteractionUpdateOrder.UpdatePhase updatePhase, ref Pose targetPose, ref Vector3 localScale)
    {
        Vector3 newRotationEuler = targetPose.rotation.eulerAngles;

        // Convert to [-180, 180] to make clamping across 0 easier
        float unclampedZ = NormalizeAngle(newRotationEuler.z);
        float clampedZ = Mathf.Clamp(unclampedZ, minZ, maxZ);

        newRotationEuler = m_InitialEulerRotation;
        newRotationEuler.z = clampedZ;

        targetPose.rotation = Quaternion.Euler(newRotationEuler);
    }

    float NormalizeAngle(float angle)
    {
        angle %= 360;
        if (angle > 180) angle -= 360;
        return angle;
    }
}
