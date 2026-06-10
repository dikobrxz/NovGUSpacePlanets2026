using UnityEngine;

/// <summary>
/// Handles objects that fall below the playable area.
/// Returns the player to the start point and respawns important interactable objects.
/// </summary>
public class FallZone : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform xrOrigin;
    [SerializeField] private Transform playerRespawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RespawnPlayer();
            return;
        }

        RespawnableObject respawnableObject = other.GetComponentInParent<RespawnableObject>();

        if (respawnableObject != null)
            respawnableObject.Respawn();
    }

    private void RespawnPlayer()
    {
        if (xrOrigin == null || playerRespawnPoint == null)
            return;

        xrOrigin.position = playerRespawnPoint.position;
        xrOrigin.rotation = playerRespawnPoint.rotation;
    }
}