using UnityEngine;

public enum SceneState
{
    Start,
    Orbit,
    Landing,
    Exploration,
    Quest,
    Return
}

public class ScenarioManager : MonoBehaviour
{
    private SceneState currentState = SceneState.Start;

    public float GetStageDuration(SceneState state)
    {
        switch (state)
        {
            case SceneState.Start: return 10f;
            case SceneState.Orbit: return 20f;
            case SceneState.Landing: return 18f;
            case SceneState.Exploration: return 25f;
            case SceneState.Quest: return 4f;
            case SceneState.Return: return 8f;
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
            case SceneState.Start: currentState = SceneState.Orbit; break;
            case SceneState.Orbit: currentState = SceneState.Landing; break;
            case SceneState.Landing: currentState = SceneState.Exploration; break;
            case SceneState.Exploration: currentState = SceneState.Quest; break;
            case SceneState.Quest: currentState = SceneState.Return; break;
            case SceneState.Return:
                Debug.Log("[ScenarioManager] Сценарий завершён!");
                return;
        }
        Debug.Log($"[ScenarioManager] Переход на этап: {currentState}");
    }

    public SceneState GetCurrentState() => currentState;
}