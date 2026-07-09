using Unity.XR.CoreUtils;
using UnityEngine;

namespace Tour_ENDI_TourStub3
{

    public class ReturnPlatform : MonoBehaviour
    {
        private StageVisuals stageVisuals;

        void Start()
        {
            stageVisuals = FindFirstObjectByType<StageVisuals>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<XROrigin>() != null)
            {
                stageVisuals?.TeleportPlayerToShip();
            }
        }
    }

}