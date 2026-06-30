using UnityEngine;
using Unity.XR.CoreUtils;

namespace Tour_Endi_Earth
{
    /// <summary>
    /// Starts the element matching quest when the player enters
    /// the table trigger zone.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class ElementQuestTriggerZone : MonoBehaviour
    {
        [Header("Tour")]
        [SerializeField] private EarthTourManager tourManager;

        [Header("Player")]
        [SerializeField] private XROrigin xrOrigin;

        [Header("Settings")]
        [SerializeField] private float activationDelay = 0.5f;

        [Header("Debug")]
        [SerializeField] private bool showDebugLogs = true;

        private BoxCollider boxCollider;
        private bool activated;
        private float timer;

        private void Awake()
        {
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.isTrigger = true;
        }

        private void Update()
        {
            if (activated)
                return;

            if (tourManager == null || xrOrigin == null || boxCollider == null)
                return;

            if (tourManager.CurrentStage != EarthTourStage.ElementColumns)
            {
                timer = 0f;
                return;
            }

            Vector3 playerHeadPosition = xrOrigin.Camera.transform.position;

            bool playerInsideZone = boxCollider.bounds.Contains(playerHeadPosition);

            if (!playerInsideZone)
            {
                timer = 0f;
                return;
            }

            timer += Time.deltaTime;

            if (timer >= activationDelay)
                StartMatchingQuest();
        }

        private void StartMatchingQuest()
        {
            if (activated)
                return;

            activated = true;

            if (showDebugLogs)
                Debug.Log("Element quest started: player entered table trigger zone.");

            tourManager.SetStage(EarthTourStage.MatchingQuest);
        }
    }
}