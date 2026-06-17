using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Physical VR button in the ship room.
/// In the single-scene version it does not load another scene.
/// It switches the tour to PlanetIntro stage.
/// </summary>
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(XRSimpleInteractable))]
public class LoadPlanetIntroButton : MonoBehaviour
{
    [Header("Tour")]
    [SerializeField] private EarthTourManager tourManager;

    [Header("Transition")]
    [SerializeField] private GameObject blackTransitionPanel;
    [SerializeField] private float delayBeforePlanetIntro = 0.7f;

    [Header("Visibility")]
    [SerializeField] private GameObject visibleRoot;

    private XRSimpleInteractable interactable;
    private Collider buttonCollider;
    private bool wasPressed;
    private bool isLocked;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        buttonCollider = GetComponent<Collider>();

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
        if (isLocked || wasPressed)
            return;

        wasPressed = true;

        if (GameAudioManager.Instance != null)
            GameAudioManager.Instance.PlayButtonClick();

        if (TourVoiceManager.Instance != null)
            TourVoiceManager.Instance.StopAllVoice();

        StartCoroutine(StartPlanetIntroRoutine());
    }

    private IEnumerator StartPlanetIntroRoutine()
    {
        SetLocked(true);

        if (blackTransitionPanel != null)
            blackTransitionPanel.SetActive(true);

        yield return new WaitForSeconds(delayBeforePlanetIntro);

        if (tourManager != null)
            tourManager.SetStage(EarthTourStage.PlanetIntro);
        else
            Debug.LogWarning("LoadPlanetIntroButton: TourManager is not assigned.");

        if (blackTransitionPanel != null)
            blackTransitionPanel.SetActive(false);
    }

    public void SetLocked(bool locked)
    {
        isLocked = locked;

        if (interactable != null)
            interactable.enabled = !locked;

        if (buttonCollider != null)
            buttonCollider.enabled = !locked;
    }

    public void SetButtonVisible(bool visible)
    {
        if (visibleRoot != null)
        {
            visibleRoot.SetActive(visible);
            return;
        }

        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);

        foreach (Renderer renderer in renderers)
            renderer.enabled = visible;
    }

    public void ResetButton()
    {
        wasPressed = false;
        SetLocked(false);
        SetButtonVisible(true);
    }
}