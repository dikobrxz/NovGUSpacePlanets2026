using UnityEngine;
using Unity.XR.CoreUtils;

/// <summary>
/// Ограничивает перемещение XR-игрока внутри прямоугольной исследовательской зоны.
/// Проверяет позицию камеры игрока, а не только XR Origin, потому что в VR камера
/// может иметь смещение относительно корня XR Origin.
/// </summary>
public class XRBoundaryLimiter : MonoBehaviour
{
    [Header("XR")]
    [SerializeField] private XROrigin xrOrigin;

    [Header("Zone")]
    [SerializeField] private Transform zoneCenter;
    [SerializeField] private Vector2 zoneSize = new Vector2(12f, 12f);
    [SerializeField] private float padding = 0.3f;

    [Header("Debug")]
    [SerializeField] private bool drawGizmos = true;

    private void Reset()
    {
        xrOrigin = FindFirstObjectByType<XROrigin>();
        zoneCenter = transform;
    }

    private void LateUpdate()
    {
        if (xrOrigin == null || xrOrigin.Camera == null)
            return;

        Vector3 center = zoneCenter != null ? zoneCenter.position : transform.position;

        float halfX = zoneSize.x * 0.5f - padding;
        float halfZ = zoneSize.y * 0.5f - padding;

        Vector3 cameraPosition = xrOrigin.Camera.transform.position;

        float minX = center.x - halfX;
        float maxX = center.x + halfX;
        float minZ = center.z - halfZ;
        float maxZ = center.z + halfZ;

        Vector3 clampedCameraPosition = cameraPosition;
        clampedCameraPosition.x = Mathf.Clamp(cameraPosition.x, minX, maxX);
        clampedCameraPosition.z = Mathf.Clamp(cameraPosition.z, minZ, maxZ);

        Vector3 correction = clampedCameraPosition - cameraPosition;
        correction.y = 0f;

        if (correction.sqrMagnitude > 0.0001f)
        {
            xrOrigin.transform.position += correction;
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos)
            return;

        Vector3 center = zoneCenter != null ? zoneCenter.position : transform.position;

        Gizmos.color = Color.red;

        Vector3 size = new Vector3(zoneSize.x, 0.05f, zoneSize.y);
        Gizmos.DrawWireCube(center, size);
    }
}