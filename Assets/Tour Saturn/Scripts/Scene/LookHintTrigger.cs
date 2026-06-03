using UnityEngine;
using System.Collections;

public class LookHintTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform target;
    [SerializeField] private UIManager uiManager;

    [Header("Hint Settings")]
    [SerializeField] private string hintText = "Обернитесь, чтобы увидеть спутник.";
    [SerializeField] private float showIfAngleGreaterThan = 70f;
    [SerializeField] private float hideIfAngleLessThan = 45f;

    public IEnumerator CheckLookDirection()
    {
        while (true)
        {
            Vector3 directionToTarget = target.position - playerCamera.position;
            directionToTarget.y = 0f;

            Vector3 cameraForward = playerCamera.forward;
            cameraForward.y = 0f;

            float angle = Vector3.Angle(cameraForward, directionToTarget);

            if (angle >= showIfAngleGreaterThan)
            {
                uiManager.ShowHint(hintText);
            }

            if (angle <= hideIfAngleLessThan)
            {
                uiManager.HideTextPanel();
                yield break;
            }

            yield return null;
        }
    }
}
