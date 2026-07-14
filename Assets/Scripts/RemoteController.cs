using UnityEngine;

public class RemoteController : MonoBehaviour
{
    [SerializeField] private GameObject _probe;
    [SerializeField] private BucketController _bucket;
    [SerializeField] private AudioSource _audio;
    [SerializeField] private AudioClip _accessSound;
    [SerializeField] private AudioClip _denySound;
    [SerializeField] private SceneManager _sceneManager;

    private bool _isActive = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Tag3" && _isActive)
        {
            if (_bucket.IsGrab)
            {
                _audio.PlayOneShot(_accessSound);
                _isActive = false;
                _probe.SetActive(true);
                _sceneManager.HideAllHints();
            }
            else
            {
                _audio.PlayOneShot(_denySound);
            }
        }
    }
}
