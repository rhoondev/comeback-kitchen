using System.Collections;
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
            StartCoroutine(PickUpObjectRoutine(tomato.transform));
        }
        else if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            GameObject bellPepper = vegetableBasket.GrabVegetable(vegetableBasket.BellPepper);
            StartCoroutine(PickUpObjectRoutine(bellPepper.transform));
        }
        else if (Keyboard.current.oKey.wasPressedThisFrame)
        {
            GameObject onion = vegetableBasket.GrabVegetable(vegetableBasket.Onion);
            StartCoroutine(PickUpObjectRoutine(onion.transform));
        }
    }

    private IEnumerator PickUpObjectRoutine(Transform obj)
    {
        Vector3 startPos = obj.position;
        Vector3 endPos = startPos + Vector3.up * 0.075f;
        float timeElapsed = 0f;

        while (timeElapsed < 1f)
        {
            obj.position = Vector3.Lerp(startPos, endPos, timeElapsed);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        obj.position = endPos;
    }
}