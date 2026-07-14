using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;


namespace Tour_ENDI_TourStub5
{

    public class ResetPositionComponent : MonoBehaviour
    {
        [SerializeField] private GameObject _go;
        [SerializeField] private Collider _goCollider;
        [SerializeField] private Collider _resetCollider;

        [Space, Header("Events")]
        [SerializeField] private ResetEvent _onReset;

        private Vector3 _startPosition;
        private Quaternion _startRotation;
        private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _grab;

        private Coroutine _checkCoroutine;
        private bool _isActive = true;
        private bool _isKinematic;

        private void Start()
        {
            _isKinematic = _go.GetComponent<Rigidbody>().isKinematic;
            _startPosition = _go.transform.position;
            _startRotation = _go.transform.rotation;
            _grab = _go.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

            _checkCoroutine = StartCoroutine(CheckOutOfBounds());
        }

        public void DisableCheck()
        { 
            _isActive = false;
        }

        private IEnumerator CheckOutOfBounds()
        {
            while (_isActive)
            {
                yield return new WaitForSeconds(1f);

                if (!_resetCollider.bounds.Intersects(_goCollider.bounds))
                {
                    _onReset?.Invoke(_go.transform);
                    ResetPosition();
                }
            }
        }

        private void ResetPosition()
        {
            if (_grab && _grab.isSelected)
                _grab.interactionManager.SelectExit(_grab.firstInteractorSelecting, _grab);

            var rb = _go.GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            _go.transform.position = _startPosition;
            _go.transform.rotation = _startRotation;

            StartCoroutine(ReenableAfter());
        }

        private IEnumerator ReenableAfter()
        {
            yield return null;
            if(_grab != null) _grab.enabled = true;

            var rb = _go.GetComponent<Rigidbody>();
            rb.isKinematic = _isKinematic;
        }

        private void OnDisable()
        {
            if (_checkCoroutine != null) StopCoroutine(_checkCoroutine);
            StopAllCoroutines();
        }
    }

    [Serializable]
    public class ResetEvent : UnityEvent<Transform>
    { }

}
