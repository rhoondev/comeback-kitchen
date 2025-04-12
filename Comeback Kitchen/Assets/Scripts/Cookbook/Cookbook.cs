using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Cookbook : MonoBehaviour
{
    [SerializeField] private Transform binding;
    [SerializeField] private Transform front;
    [SerializeField] private float openCloseTime;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private TextMeshProUGUI instructionConfirmationText;
    [SerializeField] private Image image;
    [SerializeField] private GameObject imageLeftButton;
    [SerializeField] private GameObject imageRightButton;
    [SerializeField] private Sprite missingImageSprite;

    public event Action<string> OnConfirmInstruction;

    private Transform openLocation;
    private Transform closedLocation;

    private bool _isOpen = true;
    private bool _isAnimating = false;

    private List<Sprite> _instructionImages = new List<Sprite>();
    private int _currentImageIndex = 0;

    public void SetLocations(Transform openLocation, Transform closedLocation)
    {
        this.openLocation = openLocation;
        this.closedLocation = closedLocation;

        transform.position = _isOpen ? openLocation.position : closedLocation.position;
        transform.rotation = _isOpen ? openLocation.rotation : closedLocation.rotation;
    }

    public void SetInstruction(string text, List<Sprite> images)
    {
        instructionText.text = text;
        _instructionImages = images;

        if (_instructionImages.Count > 0)
        {
            SetImage(0);
        }
        else
        {
            image.sprite = missingImageSprite;
            imageLeftButton.SetActive(false);
            imageRightButton.SetActive(false);
        }
    }

    public void ConfirmInstruction()
    {
        OnConfirmInstruction?.Invoke(instructionText.text);
    }

    public void NextImage()
    {
        SetImage(_currentImageIndex + 1);
    }

    public void PreviousImage()
    {
        SetImage(_currentImageIndex - 1);
    }

    public void Close()
    {
        if (_isOpen && !_isAnimating)
        {
            StartCoroutine(Animate());
        }
    }

    public void Open()
    {
        if (!_isOpen && !_isAnimating)
        {
            SetImage(0);
            StartCoroutine(Animate());
        }
    }

    public void PlayAudio()
    {
        Debug.Log("Play audio");
    }

    public void ChangeInstructionConfirmationText(string text)
    {
        instructionConfirmationText.text = text;
    }

    private void SetImage(int index)
    {
        if (index >= 0 && index < _instructionImages.Count)
        {
            image.sprite = _instructionImages[index];
            _currentImageIndex = index;

            imageLeftButton.SetActive(index > 0);
            imageRightButton.SetActive(index < _instructionImages.Count - 1);
        }
    }

    private IEnumerator Animate()
    {
        _isAnimating = true;

        Vector3 initialPosition = _isOpen ? openLocation.position : closedLocation.position;
        Vector3 finalPosition = _isOpen ? closedLocation.position : openLocation.position;
        Quaternion initialRotation = _isOpen ? openLocation.rotation : closedLocation.rotation;
        Quaternion finalRotation = _isOpen ? closedLocation.rotation : openLocation.rotation;
        float intialChildAngle = _isOpen ? 0f : -90f;
        float finalChildAngle = _isOpen ? -90f : 0f;
        float timePassed = 0f;

        while (timePassed < openCloseTime)
        {
            float t = Mathf.SmoothStep(0f, 1f, timePassed / openCloseTime);

            transform.position = Vector3.Lerp(initialPosition, finalPosition, t);
            transform.rotation = Quaternion.Slerp(initialRotation, finalRotation, t);

            Quaternion childRotation = Quaternion.Euler(0f, Mathf.Lerp(intialChildAngle, finalChildAngle, t), 0f);
            binding.localRotation = childRotation;
            front.localRotation = childRotation;

            timePassed += Time.deltaTime;
            yield return null;
        }

        transform.position = finalPosition;

        Quaternion finalChildRotation = Quaternion.Euler(0f, finalChildAngle, 0f);
        binding.localRotation = finalChildRotation;
        front.localRotation = finalChildRotation;

        _isOpen = !_isOpen;

        _isAnimating = false;
    }
}
