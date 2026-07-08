using UnityEngine;

namespace Tour_ENDI_PlanetsUranus
{

    public enum SceneState
    {
        Start,
        Introduction,
        Landing,
        Exploration,
        Historical,
        Quest,
        Return,
        End
    }

    public class ScenarioManager : MonoBehaviour
    {
        private SceneState currentState = SceneState.Start;

        public float GetStageDuration(SceneState state)
        {
            switch (state)
            {
                case SceneState.Start: return -1f;
                case SceneState.Introduction: return 6f;
                case SceneState.Landing: return 15f;
                case SceneState.Exploration: return 14f;
                case SceneState.Historical: return 20f;
                case SceneState.Quest: return 3f;
                case SceneState.Return: return -1f;
                case SceneState.End: return -1f;
                default: return 10f;
            }
        }

        public void StartScenario()
        {
            currentState = SceneState.Start;
            Debug.Log($"[ScenarioManager] Сценарий запущен! Этап: {currentState}");
        }

        public void NextStage()
        {
            switch (currentState)
            {
                case SceneState.Start:
                    currentState = SceneState.Introduction;
                    break;
                case SceneState.Introduction:
                    currentState = SceneState.Landing;
                    break;
                case SceneState.Landing:
                    currentState = SceneState.Exploration;
                    break;
                case SceneState.Exploration:
                    currentState = SceneState.Historical;
                    break;
                case SceneState.Historical:
                    currentState = SceneState.Quest;
                    break;
                case SceneState.Quest:
                    currentState = SceneState.Return;
                    break;
                case SceneState.Return:
                    currentState = SceneState.End;
                    break;
                case SceneState.End:
                    Debug.Log("[ScenarioManager] Сценарий полностью завершён!");
                    return;
            }
            Debug.Log($"[ScenarioManager] Переход на этап: {currentState}");
        }

        public void CompleteReturn()
        {
            if (currentState == SceneState.Return)
            {
                currentState = SceneState.End;
            }
        }

        public void RestartScenario()
        {
            currentState = SceneState.Start;
            Debug.Log($"[ScenarioManager] Сценарий перезапущен! Этап: {currentState}");
        }

        public SceneState GetCurrentState() => currentState;
    }

}