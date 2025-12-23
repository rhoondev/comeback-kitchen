using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera _mainCamera;

    private void Start()
    {
        // Cache the main camera
        _mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        // Ensure we have a camera reference
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
            if (_mainCamera == null) return;
        }

        // Make the object's forward vector point at the camera (into the eye)
        // and align its up vector with the camera's up vector.
        transform.LookAt(_mainCamera.transform.position, _mainCamera.transform.up);
    }
}
