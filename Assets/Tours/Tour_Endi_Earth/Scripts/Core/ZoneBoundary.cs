using System.Collections;
using UnityEngine;

namespace Tour_Endi_Earth
{
    /// <summary>
    /// Keeps the XR player inside the exploration zone.
    /// Works only during island gameplay stages.
    /// It is disabled during ship intro and final return to ship.
    /// </summary>
    public class ZoneBoundary : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private Transform xrOrigin;
        [SerializeField] private Transform respawnPoint;

        [Header("Tour")]
        [SerializeField] private EarthTourManager tourManager;

        [Header("Return Settings")]
        [SerializeField] private float returnDelay = 0.25f;

        [Header("Optional")]
        [SerializeField] private AudioSource warningAudio;

        private bool isReturning;

        private void Awake()
        {
            if (tourManager == null)
                tourManager = FindFirstObjectByType<EarthTourManager>();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!ShouldControlPlayer())
                return;

            if (isReturning)
                return;

            if (!other.CompareTag("Player"))
                return;

            StartCoroutine(ReturnPlayerToZone());
        }

        private IEnumerator ReturnPlayerToZone()
        {
            isReturning = true;

            if (warningAudio != null)
                warningAudio.Play();

            yield return new WaitForSeconds(returnDelay);

            if (ShouldControlPlayer() && xrOrigin != null && respawnPoint != null)
            {
                xrOrigin.position = respawnPoint.position;
                xrOrigin.rotation = respawnPoint.rotation;
            }

            isReturning = false;
        }

        private bool ShouldControlPlayer()
        {
            if (tourManager == null)
                return true;

            switch (tourManager.CurrentStage)
            {
                case EarthTourStage.ElementColumns:
                case EarthTourStage.MatchingQuest:
                case EarthTourStage.Quiz:
                    return true;

                case EarthTourStage.ShipIntro:
                case EarthTourStage.PlanetIntro:
                case EarthTourStage.SurfaceIntro:
                case EarthTourStage.End:
                default:
                    return false;
            }
        }
    }
}