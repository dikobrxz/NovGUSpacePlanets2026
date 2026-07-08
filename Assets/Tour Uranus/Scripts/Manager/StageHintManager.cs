using Unity.XR.CoreUtils;
using UnityEngine;

namespace Tour_ENDI_PlanetsUranus
{

    public class StageHintManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private LookAtObjectTrigger lookTrigger;
        [SerializeField] private Transform questTarget;
        [SerializeField] private Transform returnTarget;

        [Header("Texts")]
        [SerializeField] private string questHintPrefix;
        [SerializeField] private string returnHintPrefix;
        [SerializeField] private string rightText;
        [SerializeField] private string leftText;

        [Header("Stage")]
        [SerializeField] private SceneState questStage = SceneState.Quest;
        [SerializeField] private SceneState returnStage = SceneState.Return;

        private ScenarioManager scenarioManager;
        private Transform playerCamera;

        void Start()
        {
            scenarioManager = FindFirstObjectByType<ScenarioManager>();
            playerCamera = Camera.main?.transform;
            if (playerCamera == null)
            {
                playerCamera = FindFirstObjectByType<XROrigin>()?.transform;
            }

            if (lookTrigger == null)
            {
                lookTrigger = FindFirstObjectByType<LookAtObjectTrigger>();
            }
        }

        void Update()
        {
            if (scenarioManager == null || playerCamera == null || lookTrigger == null) return;

            SceneState currentState = scenarioManager.GetCurrentState();
            Transform target = null;
            string prefix = "";

            if (currentState == questStage && questTarget != null)
            {
                target = questTarget;
                prefix = questHintPrefix;
            }
            else if (currentState == returnStage && returnTarget != null)
            {
                target = returnTarget;
                prefix = returnHintPrefix;
            }
            else
            {
                lookTrigger.ResetHint();
                return;
            }

            Vector3 directionToTarget = target.position - playerCamera.position;
            directionToTarget.y = 0f;
            directionToTarget.Normalize();

            Vector3 cameraRight = playerCamera.right;
            cameraRight.y = 0f;
            cameraRight.Normalize();

            float side = Vector3.Dot(cameraRight, directionToTarget);
            string sideText = side >= 0 ? rightText : leftText;

            string fullHintText = $"{prefix} {sideText}";

            lookTrigger.SetTarget(target, fullHintText);
        }
    }

}