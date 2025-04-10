using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cookbook : MonoBehaviour
{
    [SerializeField] private Transform binding;
    [SerializeField] private Transform front;
    [SerializeField] private Transform openLocation;
    [SerializeField] private Transform closedLocation;
    [SerializeField] private float openCloseTime;

    private bool _isOpen = true;
    private bool _isAnimating = false;

    private void Update()
    {
        if (Keyboard.current.oKey.wasPressedThisFrame && !_isAnimating)
        {
            StartCoroutine(Animate());
        }
    }

    private IEnumerator Animate()
    {
        _isAnimating = true;

        Vector3 initialPosition = _isOpen ? openLocation.position : closedLocation.position;
        Vector3 finalPosition = _isOpen ? closedLocation.position : openLocation.position;
        float intialAngle = _isOpen ? 0f : -90f;
        float finalAngle = _isOpen ? -90f : 0f;
        float timePassed = 0f;

        while (timePassed < openCloseTime)
        {
            float t = Mathf.SmoothStep(0f, 1f, timePassed / openCloseTime);

            transform.position = Vector3.Lerp(initialPosition, finalPosition, t);

            Quaternion rotation = Quaternion.Euler(0f, Mathf.Lerp(intialAngle, finalAngle, t), 0f);
            binding.localRotation = rotation;
            front.localRotation = rotation;

            timePassed += Time.deltaTime;
            yield return null;
        }

        transform.position = finalPosition;

        Quaternion finalRotation = Quaternion.Euler(0f, finalAngle, 0f);
        binding.localRotation = finalRotation;
        front.localRotation = finalRotation;

        _isOpen = !_isOpen;

        _isAnimating = false;
    }
}
