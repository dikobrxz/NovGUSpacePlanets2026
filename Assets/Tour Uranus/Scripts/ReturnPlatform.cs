using Unity.XR.CoreUtils;
using UnityEngine;

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