using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Physical VR button in the ship room.
/// Shows black panel and loads the planet intro scene.
/// </summary>
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(XRSimpleInteractable))]
public class LoadPlanetIntroButton : MonoBehaviour
{
    [SerializeField] private string planetIntroSceneName = "SCN_Earth_PlanetIntro";

    [Header("Transition")]
    [SerializeField] private GameObject blackTransitionPanel;
    [SerializeField] private float delayBeforeLoad = 0.7f;

    private XRSimpleInteractable interactable;
    private bool wasPressed;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();

        interactable.activated.AddListener(OnActivated);
        interactable.selectEntered.AddListener(OnSelected);
    }

    private void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.activated.RemoveListener(OnActivated);
            interactable.selectEntered.RemoveListener(OnSelected);
        }
    }

    private void OnActivated(ActivateEventArgs args)
    {
        PressButton();
    }

    private void OnSelected(SelectEnterEventArgs args)
    {
        PressButton();
    }

    private void PressButton()
    {
        if (wasPressed)
            return;

        wasPressed = true;

        if (GameAudioManager.Instance != null)
            GameAudioManager.Instance.PlayButtonClick();

        StartCoroutine(LoadSceneRoutine());
    }

    private IEnumerator LoadSceneRoutine()
    {
        if (blackTransitionPanel != null)
            blackTransitionPanel.SetActive(true);

        yield return new WaitForSeconds(delayBeforeLoad);

        SceneManager.LoadScene(planetIntroSceneName);
    }
}