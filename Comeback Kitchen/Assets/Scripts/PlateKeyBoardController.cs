using UnityEngine;
using UnityEngine.InputSystem;

public class PlateKeyBoardController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    private void FixedUpdate()
    {
        if (Keyboard.current.rKey.isPressed)
        {
            rb.MoveRotation(Quaternion.Euler(60f * Time.fixedDeltaTime, 0f, 0f) * rb.rotation);
        }
        else if (Keyboard.current.lKey.isPressed)
        {
            rb.MoveRotation(Quaternion.Euler(-60f * Time.fixedDeltaTime, 0f, 0f) * rb.rotation);
        }
    }
}