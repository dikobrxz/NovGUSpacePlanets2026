using UnityEngine;

public class ProbeController : MonoBehaviour
{
    [SerializeField] private BucketController _bucket;
    [SerializeField] private AudioSource _audio;
    [SerializeField] private AudioClip _pickClip;
    [SerializeField] private SceneManager _sceneManager;

    public void SetFragment()
    { 
        var fragment = _bucket.GetFragment();

        fragment.transform.SetParent(transform);
        _audio.PlayOneShot(_pickClip);
    }

    public void CompleteScene()
    {
        _sceneManager.CompleteProbeQuest();
    }
}
