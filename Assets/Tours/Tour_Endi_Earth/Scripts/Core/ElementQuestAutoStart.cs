using UnityEngine;
using Unity.XR.CoreUtils;

/// <summary>
/// Automatically switches from ElementColumns stage to MatchingQuest
/// when the player comes close to the element table.
/// </summary>
public class ElementQuestAutoStart : MonoBehaviour
{
    [Header("Tour")]
    [SerializeField] private EarthTourManager tourManager;

    [Header("Player")]
    [SerializeField] private XROrigin xrOrigin;

    [Header("Target")]
    [SerializeField] private Transform tablePoint;
    [SerializeField] private float activationDistance = 2.5f;

    [Header("Fallback")]
    [SerializeField] private bool useFallbackTimer = true;
    [SerializeField] private float fallbackDelay = 8f;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private bool isWaiting;
    private bool activated;
    private float timer;

    private void Update()
    {
        if (tourManager == null || xrOrigin == null || tablePoint == null)
            return;

        if (activated)
            return;

        if (tourManager.CurrentStage != EarthTourStage.ElementColumns)
        {
            isWaiting = false;
            timer = 0f;
            return;
        }

        isWaiting = true;

        Vector3 cameraPosition = xrOrigin.Camera.transform.position;
        float distance = Vector3.Distance(cameraPosition, tablePoint.position);

        if (distance <= activationDistance)
        {
            StartMatchingQuest("player approached the table");
            return;
        }

        if (useFallbackTimer)
        {
            timer += Time.deltaTime;

            if (timer >= fallbackDelay)
                StartMatchingQuest("fallback timer");
        }
    }

    private void StartMatchingQuest(string reason)
    {
        if (activated)
            return;

        activated = true;

        if (showDebugLogs)
            Debug.Log($"Element quest started automatically: {reason}");

        tourManager.SetStage(EarthTourStage.MatchingQuest);
    }
}