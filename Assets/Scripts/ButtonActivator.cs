using UnityEngine;

public class ButtonActivator : MonoBehaviour
{
    public CompassDevice compassDevice;

    void OnMouseDown()
    {
        compassDevice.ActivateCompass();
    }
}