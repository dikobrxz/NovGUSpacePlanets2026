using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tour_ENDI_TourStub5
{

    public class AxeController : MonoBehaviour
    {
        [SerializeField] private AudioSource _audio;
        [SerializeField] private List<AudioClip> _hitClips;
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private GameObject _fragment;
        [SerializeField] private ParticleSystem _particles;
        [SerializeField] private SceneManager _sceneManager;
        private int _counter = 0;
        private bool _isActive = false;
        private bool _isHint = true;

        public void Activate(float delay = .5f)
        {
            StopAllCoroutines();
            StartCoroutine(Delay(.5f));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Tag1" && _isActive)
            {
                if (_rb.linearVelocity.magnitude > .4f)
                {
                    _counter++;
                    var clip = _hitClips[Random.Range(0, _hitClips.Count)];

                    _audio.PlayOneShot(clip);
                    _particles.Emit(50);

                    Activate();

                    if (_counter > 6)
                    {
                        _counter = 0;
                        Vector3 hitPoint = other.ClosestPoint(transform.position);
                        Instantiate(_fragment, hitPoint, Quaternion.identity);
                        if(_isHint)_sceneManager.SetGrabHint();
                        _isHint = false;
                    }
                }


            }
        }

        private IEnumerator Delay(float delay)
        {
            _isActive = false;
            yield return new WaitForSeconds(delay);
            _isActive = true;
        }
    }

}
