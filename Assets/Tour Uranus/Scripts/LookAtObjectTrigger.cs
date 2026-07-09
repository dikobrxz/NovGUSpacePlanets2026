using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace Tour_ENDI_TourStub3
{

    public class LookAtObjectTrigger : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform playerCamera;
        [SerializeField] private Transform targetObject;
        [SerializeField] private CanvasWorldHint hintCanvas;

        [Header("Settings")]
        [SerializeField] private float showAngle = 50f;
        [SerializeField] private float hideAngle = 60f;
        [SerializeField] private float checkInterval = 0.2f;

        private Coroutine checkCoroutine;
        private bool isHintVisible = false;
        private string currentHintText = "";

        void Start()
        {
            if (playerCamera == null)
            {
                playerCamera = Camera.main?.transform;
                if (playerCamera == null)
                {
                    playerCamera = FindFirstObjectByType<XROrigin>()?.transform;
                }
            }

            if (hintCanvas == null)
            {
                hintCanvas = FindFirstObjectByType<CanvasWorldHint>();
            }
        }

        public void SetTarget(Transform newTarget, string newText)
        {
            targetObject = newTarget;
            currentHintText = newText;
            isHintVisible = false;
            hintCanvas.HideHint();

            if (checkCoroutine != null) StopCoroutine(checkCoroutine);
            checkCoroutine = StartCoroutine(CheckLookDirection());
        }

        IEnumerator CheckLookDirection()
        {
            while (true)
            {
                if (targetObject == null || playerCamera == null || string.IsNullOrEmpty(currentHintText))
                {
                    if (isHintVisible)
                    {
                        isHintVisible = false;
                        hintCanvas.HideHint();
                    }
                    yield return new WaitForSeconds(checkInterval);
                    continue;
                }

                Vector3 directionToTarget = targetObject.position - playerCamera.position;
                directionToTarget.y = 0f;
                directionToTarget.Normalize();

                Vector3 cameraForward = playerCamera.forward;
                cameraForward.y = 0f;
                cameraForward.Normalize();

                float angle = Vector3.Angle(cameraForward, directionToTarget);

                if (angle >= showAngle)
                {
                    if (!isHintVisible)
                    {
                        isHintVisible = true;
                        hintCanvas.ShowHint(currentHintText);
                    }
                }

                if (angle <= hideAngle && isHintVisible)
                {
                    isHintVisible = false;
                    hintCanvas.HideHint();
                }

                yield return new WaitForSeconds(checkInterval);
            }
        }

        public void ResetHint()
        {
            isHintVisible = false;
            hintCanvas.HideHint();
            if (checkCoroutine != null) StopCoroutine(checkCoroutine);
            targetObject = null;
            currentHintText = "";
        }
    }

}