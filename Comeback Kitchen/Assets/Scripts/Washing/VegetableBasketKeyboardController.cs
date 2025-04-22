using UnityEngine;
using UnityEngine.InputSystem;

public class VegetableBasketKeyboardController : MonoBehaviour
{
    [SerializeField] private VegetableBasket vegetableBasket;

    private void Update()
    {
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            GameObject tomato = vegetableBasket.GrabVegetable(vegetableBasket.Tomato);
            tomato.transform.Translate(Vector3.up * 0.1f, Space.World);
        }
        else if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            GameObject bellPepper = vegetableBasket.GrabVegetable(vegetableBasket.BellPepper);
            bellPepper.transform.Translate(Vector3.up * 0.1f, Space.World);
        }
        else if (Keyboard.current.oKey.wasPressedThisFrame)
        {
            GameObject onion = vegetableBasket.GrabVegetable(vegetableBasket.Onion);
            onion.transform.Translate(Vector3.up * 0.1f, Space.World);
        }
    }
}