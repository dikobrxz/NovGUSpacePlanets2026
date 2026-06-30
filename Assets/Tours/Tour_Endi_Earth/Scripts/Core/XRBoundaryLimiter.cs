using UnityEngine;
using Unity.XR.CoreUtils;

namespace Tour_Endi_Earth
{
    /// <summary>
    /// Ограничивает перемещение XR-игрока внутри прямоугольной исследовательской зоны.
    /// Ограничение работает только на островных этапах тура.
    /// В корабле оно отключается, чтобы финальный телепорт не возвращал игрока обратно на остров.
    /// </summary>
    public class XRBoundaryLimiter : MonoBehaviour
    {
        [Header("XR")]
        [SerializeField] private XROrigin xrOrigin;

        [Header("Tour")]
        [SerializeField] private EarthTourManager tourManager;

        [Header("Zone")]
        [SerializeField] private Transform zoneCenter;
        [SerializeField] private Vector2 zoneSize = new Vector2(12f, 12f);
        [SerializeField] private float padding = 0.3f;

        [Header("Debug")]
        [SerializeField] private bool drawGizmos = true;

        private void Reset()
        {
            xrOrigin = FindFirstObjectByType<XROrigin>();
            tourManager = FindFirstObjectByType<EarthTourManager>();
            zoneCenter = transform;
        }

        private void Awake()
        {
            if (xrOrigin == null)
                xrOrigin = FindFirstObjectByType<XROrigin>();

            if (tourManager == null)
                tourManager = FindFirstObjectByType<EarthTourManager>();
        }

        private void LateUpdate()
        {
            if (!ShouldLimitPlayer())
                return;

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
                xrOrigin.transform.position += correction;
        }

        private bool ShouldLimitPlayer()
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
}