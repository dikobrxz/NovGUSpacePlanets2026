using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the planet intro inside the main scene.
/// Plays intro narration, then moves the player to the island and starts the element stage.
/// </summary>
public class PlanetIntroController : MonoBehaviour
{
    [Header("Tour")]
    [SerializeField] private EarthTourManager tourManager;
    [SerializeField] private MainSceneEntryController entryController;

    [Header("Visual")]
    [SerializeField] private GameObject blackTransitionPanel;
    [SerializeField] private float blackScreenAfterStart = 0.2f;
    [SerializeField] private float blackScreenBeforeIsland = 0.5f;

    [Header("Voice Clips")]
    [SerializeField] private AudioClip mainIntroClip;
    [SerializeField] private AudioClip planetCutsceneClip;

    [Header("Timing")]
    [SerializeField] private float delayAfterVoice = 1f;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private Coroutine routine;

    private void OnEnable()
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(PlanetIntroRoutine());
    }

    private void OnDisable()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }
    }

    private IEnumerator PlanetIntroRoutine()
    {
        if (blackTransitionPanel != null)
            blackTransitionPanel.SetActive(true);

        yield return new WaitForSeconds(blackScreenAfterStart);

        if (blackTransitionPanel != null)
            blackTransitionPanel.SetActive(false);

        if (TourVoiceManager.Instance != null)
        {
            yield return TourVoiceManager.Instance.PlaySequenceAndWait(
                new AudioClip[] { mainIntroClip, planetCutsceneClip },
                "planet intro",
                true
            );
        }
        else
        {
            float fallback = 0f;

            if (mainIntroClip != null)
                fallback += mainIntroClip.length;

            if (planetCutsceneClip != null)
                fallback += planetCutsceneClip.length;

            yield return new WaitForSeconds(fallback);
        }

        yield return new WaitForSeconds(delayAfterVoice);

        if (blackTransitionPanel != null)
            blackTransitionPanel.SetActive(true);

        yield return new WaitForSeconds(blackScreenBeforeIsland);

        if (entryController == null)
            entryController = MainSceneEntryController.Instance;

        if (entryController != null)
            entryController.MovePlayerToIsland();
        else if (showDebugLogs)
            Debug.LogWarning("PlanetIntroController: EntryController is not assigned.");

        if (tourManager != null)
            tourManager.SetStage(EarthTourStage.ElementColumns);
        else if (showDebugLogs)
            Debug.LogWarning("PlanetIntroController: TourManager is not assigned.");

        if (blackTransitionPanel != null)
            blackTransitionPanel.SetActive(false);
    }
}
