using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.XR.CoreUtils;

/// <summary>
/// Places the XR player in the ship room at the beginning of the only scene.
/// Other scripts can later move the player to the island without loading another scene.
/// Also handles the final return to the ship after the quiz.
/// </summary>
[DefaultExecutionOrder(-1000)]
public class MainSceneEntryController : MonoBehaviour
{
    public static MainSceneEntryController Instance { get; private set; }

    [Header("XR")]
    [SerializeField] private XROrigin xrOrigin;

    [Header("Spawn Points")]
    [SerializeField] private Transform shipRoomSpawn;
    [SerializeField] private Transform islandStartSpawn;

    [Header("Tour")]
    [SerializeField] private EarthTourManager tourManager;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private Coroutine finalReturnRoutine;

    private void Awake()
    {
        Instance = this;
        StartInShip();
    }

    private void Start()
    {
        // Apply one more time after XR systems initialize.
        if (tourManager != null && tourManager.CurrentStage == EarthTourStage.ShipIntro)
            MovePlayerTo(shipRoomSpawn);
    }

    public void StartInShip()
    {
        if (tourManager != null)
            tourManager.SetStage(EarthTourStage.ShipIntro);

        MovePlayerTo(shipRoomSpawn);
    }

    public void MovePlayerToShip()
    {
        MovePlayerTo(shipRoomSpawn);
    }

    public void MovePlayerToIsland()
    {
        MovePlayerTo(islandStartSpawn);
    }

    public void MovePlayerTo(Transform spawnPoint)
    {
        if (xrOrigin == null || spawnPoint == null)
        {
            if (showDebugLogs)
                Debug.LogWarning("MainSceneEntryController: XR Origin or Spawn Point is not assigned.");

            return;
        }

        CharacterController characterController = xrOrigin.GetComponent<CharacterController>();
        bool characterControllerWasEnabled = false;

        if (characterController != null)
        {
            characterControllerWasEnabled = characterController.enabled;
            characterController.enabled = false;
        }

        xrOrigin.MatchOriginUpCameraForward(spawnPoint.up, spawnPoint.forward);
        xrOrigin.MoveCameraToWorldLocation(spawnPoint.position);

        if (characterController != null)
            characterController.enabled = characterControllerWasEnabled;

        if (showDebugLogs)
            Debug.Log("MainSceneEntryController: player moved to " + spawnPoint.name);
    }

    public void StartFinalReturnToShip(
        EarthTourManager targetTourManager,
        LoadPlanetIntroButton startButton,
        bool hideStartButton,
        bool reloadSceneAfterFinalVoice,
        float delayAfterReturnToShip,
        float delayBeforeSceneReload)
    {
        if (finalReturnRoutine != null)
            StopCoroutine(finalReturnRoutine);

        finalReturnRoutine = StartCoroutine(FinalReturnToShipRoutine(
            targetTourManager,
            startButton,
            hideStartButton,
            reloadSceneAfterFinalVoice,
            delayAfterReturnToShip,
            delayBeforeSceneReload));
    }

    private IEnumerator FinalReturnToShipRoutine(
        EarthTourManager targetTourManager,
        LoadPlanetIntroButton startButton,
        bool hideStartButton,
        bool reloadSceneAfterFinalVoice,
        float delayAfterReturnToShip,
        float delayBeforeSceneReload)
    {
        if (targetTourManager == null)
            targetTourManager = tourManager;

        if (targetTourManager != null)
        {
            if (TourVoiceManager.Instance != null)
                TourVoiceManager.Instance.SuppressNextStageVoice(EarthTourStage.ShipIntro);

            targetTourManager.SetStage(EarthTourStage.ShipIntro);
        }

        MovePlayerToShip();

        if (startButton != null)
        {
            startButton.SetLocked(true);

            if (hideStartButton)
                startButton.SetButtonVisible(false);
        }

        if (delayAfterReturnToShip > 0f)
            yield return new WaitForSeconds(delayAfterReturnToShip);

        if (TourVoiceManager.Instance != null)
        {
            TourVoiceManager.Instance.StopAllVoice();
            TourVoiceManager.Instance.PlayNextAdventureVoice();

            yield return TourVoiceManager.Instance.WaitUntilIdle();
        }

        if (reloadSceneAfterFinalVoice)
        {
            if (delayBeforeSceneReload > 0f)
                yield return new WaitForSeconds(delayBeforeSceneReload);

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            yield break;
        }

        if (startButton != null)
        {
            startButton.SetButtonVisible(true);
            startButton.ResetButton();
        }

        finalReturnRoutine = null;
    }
}