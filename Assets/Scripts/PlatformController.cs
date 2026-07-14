using UnityEngine;

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
