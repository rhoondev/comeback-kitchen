using UnityEngine;
using UnityEngine.InputSystem;

public class CookbookKeyboardController : MonoBehaviour
{
    [SerializeField] private Cookbook cookbook;

    private void Update()
    {
        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            cookbook.Open();
        }
        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            cookbook.PreviousImage();
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            cookbook.NextImage();
        }
        else if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            cookbook.ConfirmInstruction();
        }
    }
}
