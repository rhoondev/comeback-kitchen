using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRBaseInteractable))]
public class ReloadSceneButton : MonoBehaviour
{
    [SerializeField] private float reloadDelay = 1f;

    private XRBaseInteractable interactable;
    private bool isReloading = false;

    private void Awake()
    {
        interactable = GetComponent<XRBaseInteractable>();
    }

    private void OnEnable()
    {
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnButtonPressed);
        }
    }

    private void OnDisable()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnButtonPressed);
        }
    }

    private void OnButtonPressed(SelectEnterEventArgs args)
    {
        if (isReloading) return;

        StartCoroutine(ReloadWithDelay());
    }

    private IEnumerator ReloadWithDelay()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadDelay);

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}