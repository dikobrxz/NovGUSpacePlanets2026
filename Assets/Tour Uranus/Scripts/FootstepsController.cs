using UnityEngine;

namespace Tour_ENDI_TourStub3
{

    public class FootstepsController : MonoBehaviour
    {
        [SerializeField] private float _stepDistance;
        [SerializeField] private AudioSource _audio;
        [SerializeField] private AudioClip[] _clips;

        private Vector3 _lastPosition;

        private void Update()
        {
            Debug.Log(Vector3.Distance(transform.position, _lastPosition));
            if (_stepDistance < Vector3.Distance(transform.position, _lastPosition))
            {
                _audio.PlayOneShot(_clips[Random.Range(0, _clips.Length)]);
                _lastPosition = transform.position;
            }
        }
    }

}
