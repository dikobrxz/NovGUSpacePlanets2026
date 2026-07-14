using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BucketController : MonoBehaviour
{
    [SerializeField] private SceneManager _sceneManager;

    private GameObject _fragment;

    public bool IsGrab => _isGrab;

    private bool _isGrab = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Tag2")
        { 
            var grab = other.GetComponent<XRGrabInteractable>();
            if(grab != null) grab.enabled = false;
            var rb = other.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;

            _sceneManager.SetProbeHint();
            _isGrab = true;

            if (_fragment == null) _fragment = other.gameObject;
        }
    }

    public GameObject GetFragment()
    { 
        if(_fragment != null) return _fragment;
        else return null;
    }
}
