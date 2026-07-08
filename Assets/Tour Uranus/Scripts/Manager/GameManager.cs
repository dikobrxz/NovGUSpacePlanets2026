using UnityEngine;

namespace Tour_ENDI_PlanetsUranus
{

    public class GameManager : MonoBehaviour
    {
        private ScenarioManager scenarioManager;
        private float stageTimer = 0f;
        private float questTimer = 0f;
        private bool isScenarioFinished = false;
        private bool isQuestActive = false;
        private bool isDrawing = false;
        private bool isQuestCompleted = false;

        [SerializeField] private float questDrawingTime = 90f;

        void Start()
        {
            scenarioManager = GetComponent<ScenarioManager>();
            if (scenarioManager == null) return;

            scenarioManager.StartScenario();
        }

        void Update()
        {
            if (isScenarioFinished) return;

            SceneState currentState = scenarioManager.GetCurrentState();

            if (currentState == SceneState.Quest)
            {
                HandleQuestStage();
            }
            else
            {
                HandleNormalStage(currentState);
            }
        }

        private void HandleNormalStage(SceneState currentState)
        {
            float duration = scenarioManager.GetStageDuration(currentState);

            if (duration < 0)
            {
                return;
            }

            stageTimer += Time.deltaTime;

            if (stageTimer >= duration)
            {
                stageTimer = 0f;
                scenarioManager.NextStage();
            }
        }

        private void HandleQuestStage()
        {
            if (isQuestCompleted) return;

            if (!isQuestActive)
            {
                stageTimer += Time.deltaTime;

                if (stageTimer >= 4f)
                {
                    stageTimer = 0f;
                    isQuestActive = true;
                    isDrawing = true;
                }
            }
            else if (isDrawing)
            {
                questTimer += Time.deltaTime;

                if (questTimer >= questDrawingTime)
                {
                    CompleteQuest();
                }
            }
        }

        private void CompleteQuest()
        {
            if (isQuestCompleted) return;

            isDrawing = false;
            isQuestActive = false;
            isQuestCompleted = true;
            questTimer = 0f;
            stageTimer = 0f;

            scenarioManager.NextStage();
        }

        public void OnSendDrawingButtonPressed()
        {
            if (scenarioManager.GetCurrentState() == SceneState.Quest && isDrawing && !isQuestCompleted)
            {
                CompleteQuest();
            }
        }

        public void OnPlayerReturnedToShip()
        {
            if (scenarioManager.GetCurrentState() == SceneState.Return)
            {
                scenarioManager.CompleteReturn();
            }
        }
    }

}