using System.Collections;
using UnityEngine;

/// <summary>
/// Keeps the XR player inside the exploration zone.
/// If the player leaves the allowed trigger area, returns XR Origin to the respawn point.
/// </summary>
public class ZoneBoundary : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform xrOrigin;
    [SerializeField] private Transform respawnPoint;

    [Header("Return Settings")]
    [SerializeField] private float returnDelay = 0.25f;

    [Header("Optional")]
    [SerializeField] private AudioSource warningAudio;

    private bool isReturning;

    private void OnTriggerExit(Collider other)
    {
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

        if (xrOrigin != null && respawnPoint != null)
        {
            xrOrigin.position = respawnPoint.position;
            xrOrigin.rotation = respawnPoint.rotation;
        }

        isReturning = false;
    }
}