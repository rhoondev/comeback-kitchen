using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;

public class VRPlayerMover : MonoBehaviour
{
    [SerializeField] private XROrigin xrOrigin;
    [SerializeField] private float initalCameraHeight;

    private void Start()
    {
        StartCoroutine(SetInitialCameraHeightRoutine());
    }

    // Move the player to a target position (along the XZ plane) and rotate the rig to face a target forward direction (Y rotation only)
    public void SetPlayerPosition(Vector3 targetPosition, Vector3 targetForward)
    {
        StartCoroutine(SetPlayerPositionRoutine(targetPosition, targetForward));
    }

    private IEnumerator SetPlayerPositionRoutine(Vector3 targetPosition, Vector3 targetForward)
    {
        // Wait until the position of the headset is detected
        yield return new WaitUntil(() => xrOrigin.Camera.transform.localPosition != Vector3.zero);

        Transform cameraTransform = xrOrigin.Camera.transform;

        // Flatten and normalize current and desired forward vectors
        Vector3 currentForward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
        Vector3 desiredForward = new Vector3(targetForward.x, 0, targetForward.z).normalized;

        // Calculate rotation angle
        float angleDifference = Vector3.SignedAngle(currentForward, desiredForward, Vector3.up);

        // Rotate rig around current head position (Y only)
        xrOrigin.transform.RotateAround(cameraTransform.position, Vector3.up, angleDifference);

        // Recalculate offset after rotation
        Vector3 headOffset = cameraTransform.position - xrOrigin.transform.position;

        // Only apply XZ movement, keep current Y position of rig
        Vector3 newOriginPosition = new Vector3(
            targetPosition.x - headOffset.x,
            xrOrigin.transform.position.y,
            targetPosition.z - headOffset.z
        );

        xrOrigin.transform.position = newOriginPosition;
    }

    // Configures the initial height of the camera in the game world
    private IEnumerator SetInitialCameraHeightRoutine()
    {
        // Wait until the position of the headset is detected
        yield return new WaitUntil(() => xrOrigin.Camera.transform.localPosition != Vector3.zero);

        // Compute how much to move the rig up/down so the head is at desired Y
        float offsetY = initalCameraHeight - xrOrigin.Camera.transform.position.y;

        // Apply vertical adjustment only
        xrOrigin.transform.position += new Vector3(0, offsetY, 0);
    }

}
