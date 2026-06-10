using UnityEngine;
using Unity.XR.CoreUtils;

/// <summary>
/// Places the XR player either in the ship room or on the island,
/// depending on how the main scene was opened.
/// </summary>
[DefaultExecutionOrder(-1000)]
public class MainSceneEntryController : MonoBehaviour
{
    [Header("XR")]
    [SerializeField] private XROrigin xrOrigin;

    [Header("Spawn Points")]
    [SerializeField] private Transform shipRoomSpawn;
    [SerializeField] private Transform islandStartSpawn;

    [Header("Tour")]
    [SerializeField] private EarthTourManager tourManager;

    private bool startOnIsland;

    private void Awake()
    {
        startOnIsland = TourSceneState.StartMainSceneOnIsland;
        TourSceneState.StartMainSceneOnIsland = false;

        ApplyEntryPoint();
    }

    private void Start()
    {
        // Apply one more time after XR systems initialize.
        ApplyEntryPoint();
    }

    private void ApplyEntryPoint()
    {
        if (startOnIsland)
        {
            if (tourManager != null)
                tourManager.SetStage(EarthTourStage.ElementColumns);

            MovePlayerTo(islandStartSpawn);
        }
        else
        {
            if (tourManager != null)
                tourManager.SetStage(EarthTourStage.ShipIntro);

            MovePlayerTo(shipRoomSpawn);
        }
    }

    private void MovePlayerTo(Transform spawnPoint)
    {
        if (xrOrigin == null || spawnPoint == null)
            return;

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
    }
}
