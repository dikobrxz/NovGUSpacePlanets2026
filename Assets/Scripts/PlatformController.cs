using UnityEngine;

namespace Tour_ENDI_TourStub5
{

    public class PlatformController : MonoBehaviour
    {
        [SerializeField] private SceneManager _sceneManager;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Tag3")
            {
                _sceneManager.TeleportPlayerToShip();
            }
        }
    }

}
