using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class Cookbook : MonoBehaviour
{
    [SerializeField] private Transform binding;
    [SerializeField] private Transform front;
    [SerializeField] private float openCloseTime;
    [SerializeField] private List<TrackedDeviceGraphicRaycaster> insideCanvasRaycasters;
    [SerializeField] private List<TrackedDeviceGraphicRaycaster> outsideCanvasRaycasters;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private TextMeshProUGUI instructionConfirmationText;
    [SerializeField] private Image image;
    [SerializeField] private GameObject imageLeftButton;
    [SerializeField] private GameObject imageRightButton;
    [SerializeField] private Sprite missingImageSprite;

    public SmartAction<Instruction> OnConfirmInstruction = new SmartAction<Instruction>();

    private Transform openLocation;
    private Transform closedLocation;

    private bool _isOpen = true;
    private bool _isAnimating = false;

    private Instruction _instruction;
    private int _currentImageIndex = 0;

    public void SetLocations(Transform openLocation, Transform closedLocation)
    {
        this.openLocation = openLocation;
        this.closedLocation = closedLocation;

        transform.position = _isOpen ? openLocation.position : closedLocation.position;
        transform.rotation = _isOpen ? openLocation.rotation : closedLocation.rotation;
    }

    public void SetInstruction(Instruction instruction)
    {
        _instruction = instruction;

        instructionText.text = instruction.Text;

        if (_instruction.Images.Count > 0)
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
        OnConfirmInstruction?.Invoke(_instruction);
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
            foreach (var raycaster in insideCanvasRaycasters)
            {
                raycaster.enabled = false;
            }

            foreach (var raycaster in outsideCanvasRaycasters)
            {
                raycaster.enabled = true;
            }

            StartCoroutine(Animate());
        }
    }

    public void Open()
    {
        if (!_isOpen && !_isAnimating)
        {
            foreach (var raycaster in insideCanvasRaycasters)
            {
                raycaster.enabled = true;
            }

            foreach (var raycaster in outsideCanvasRaycasters)
            {
                raycaster.enabled = false;
            }

            SetImage(0);
            StartCoroutine(Animate());
        }
    }

    public void PlayAudio()
    {
        Debug.Log("Play audio");
    }

    public void OpenSettings()
    {
        Debug.Log("Open settings");
    }

    public void ExitGame()
    {
        Debug.Log("Exit game");
    }

    public void ChangeInstructionConfirmationText(string text)
    {
        instructionConfirmationText.text = text;
    }

    private void SetImage(int index)
    {
        if (index >= 0 && index < _instruction.Images.Count)
        {
            image.sprite = _instruction.Images[index];
            _currentImageIndex = index;

            imageLeftButton.SetActive(index > 0);
            imageRightButton.SetActive(index < _instruction.Images.Count - 1);
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
