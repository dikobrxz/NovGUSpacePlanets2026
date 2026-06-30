using System.Collections;
using UnityEngine;

namespace Tour_Endi_Earth
{
    /// <summary>
    /// Returns an elemental object back to its table respawn point
    /// if it falls, leaves the allowed area, or becomes hard to reach.
    /// Does not respawn the object while it is safely lying near its table point.
    /// </summary>
    public class EarthElementRespawn : MonoBehaviour
    {
        [Header("Respawn")]
        [SerializeField] private Transform respawnPoint;

        [Header("Safe Table Zone")]
        [SerializeField] private float safeRadiusAroundRespawn = 0.75f;

        [Header("Allowed Area")]
        [SerializeField] private Collider allowedArea;
        [SerializeField] private float maxDistanceFromRespawn = 8f;
        [SerializeField] private float minWorldY = -5f;

        [Header("Floor / Decoration Return")]
        [SerializeField] private bool returnIfBelowTableLevel = true;
        [SerializeField] private float minHeightOffsetFromRespawn = -0.6f;
        [SerializeField] private float returnDelay = 2f;

        [Header("Physics After Respawn")]
        [SerializeField] private bool enableGravityAfterRespawn = true;
        [SerializeField] private float linearDampingAfterRespawn = 1f;
        [SerializeField] private float angularDampingAfterRespawn = 1f;

        [Header("Debug")]
        [SerializeField] private bool showDebugLogs = true;

        private Rigidbody rb;
        private ElementItem elementItem;

        private float unsafeTimer;
        private bool isRespawning;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            elementItem = GetComponent<ElementItem>();
        }

        private void Update()
        {
            if (isRespawning)
                return;

            if (respawnPoint == null)
                return;

            if (elementItem != null && elementItem.IsLockedOnPedestal)
                return;

            if (elementItem != null && elementItem.IsGrabbed)
            {
                unsafeTimer = 0f;
                return;
            }

            // Главное правило: если предмет лежит рядом со своей точкой на столе,
            // не считаем его потерянным и не респавним каждые пару секунд.
            if (IsNearRespawnPoint())
            {
                unsafeTimer = 0f;
                return;
            }

            if (ShouldRespawnImmediately())
            {
                Respawn();
                return;
            }

            if (IsInUnsafeRestState())
            {
                unsafeTimer += Time.deltaTime;

                if (unsafeTimer >= returnDelay)
                    Respawn();

                return;
            }

            unsafeTimer = 0f;
        }

        private bool IsNearRespawnPoint()
        {
            if (respawnPoint == null)
                return false;

            float distance = Vector3.Distance(transform.position, respawnPoint.position);
            return distance <= safeRadiusAroundRespawn;
        }

        private bool ShouldRespawnImmediately()
        {
            if (transform.position.y < minWorldY)
                return true;

            float distance = Vector3.Distance(transform.position, respawnPoint.position);

            if (distance > maxDistanceFromRespawn)
                return true;

            return false;
        }

        private bool IsInUnsafeRestState()
        {
            if (allowedArea != null && !allowedArea.bounds.Contains(transform.position))
                return true;

            if (returnIfBelowTableLevel)
            {
                float minSafeY = respawnPoint.position.y + minHeightOffsetFromRespawn;

                if (transform.position.y < minSafeY)
                    return true;
            }

            return false;
        }

        public void Respawn()
        {
            if (respawnPoint == null)
                return;

            StartCoroutine(RespawnRoutine());
        }

        private IEnumerator RespawnRoutine()
        {
            isRespawning = true;
            unsafeTimer = 0f;

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            transform.SetParent(null);
            transform.SetPositionAndRotation(respawnPoint.position, respawnPoint.rotation);

            yield return null;

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = false;

                rb.useGravity = enableGravityAfterRespawn;
                rb.linearDamping = linearDampingAfterRespawn;
                rb.angularDamping = angularDampingAfterRespawn;
            }

            isRespawning = false;

            if (showDebugLogs)
                Debug.Log($"{name} returned to table respawn point.");
        }
    }
}