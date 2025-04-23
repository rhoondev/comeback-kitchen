using UnityEngine;
using UnityEngine.InputSystem;

public class Blender : MonoBehaviour
{
    [SerializeField] private Rigidbody blade;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;

    private float _currentSpeed = 0f;
    private bool _isOn = false;

    private void Update()
    {
        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            _isOn = !_isOn;
            BlendObject.bladesSpinning = !BlendObject.bladesSpinning;       //Ryan here, I added this for my BlendObject script (only works if there is 1 blender being used // static var)
        }
    }

    private void FixedUpdate()
    {
        if (_isOn)
        {
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, maxSpeed, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, 0f, acceleration * Time.fixedDeltaTime);
        }

        blade.MoveRotation(Quaternion.Euler(0f, _currentSpeed * Time.fixedDeltaTime, 0f) * blade.rotation);
    }
}
