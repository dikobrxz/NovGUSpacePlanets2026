using UnityEngine;

namespace Tour_ENDI_TourStub6
{

    public class ButtonActivator : MonoBehaviour
    {
        public CompassDevice compassDevice;

        void OnMouseDown()
        {
            compassDevice.ActivateCompass();
        }
    }

}