using UnityEngine;

public enum SceneState
{
    Start,      // начало
    Orbit,      // нахождение на орбите
    Landing,    // высадка
    Exploration,// исследование поверхности
    Quest       // выполнение квеста
}

public class SceneManager : MonoBehaviour
{
    private SceneState currentState = SceneState.Start;

    public void StartScenario()
    {
        currentState = SceneState.Start;
        ShowCurrentState();
        Debug.Log("Сценарий запущен!");
    }

    public void NextStage()
    {
        switch (currentState)
        {
            case SceneState.Start:
                currentState = SceneState.Orbit;
                break;
            case SceneState.Orbit:
                currentState = SceneState.Landing;
                break;
            case SceneState.Landing:
                currentState = SceneState.Exploration;
                break;
            case SceneState.Exploration:
                currentState = SceneState.Quest;
                break;
            case SceneState.Quest:
                Debug.Log("Это финальный этап! Сценарий завершён.");
                return;
        }

        ShowCurrentState();
    }

    public void ShowCurrentState()
    {
        string stateInfo = "";

        switch (currentState)
        {
            case SceneState.Start:
                stateInfo = "Начало игры - игрок готовится к миссии";
                break;
            case SceneState.Orbit:
                stateInfo = "Орбита - игрок находится на орбите планеты";
                break;
            case SceneState.Landing:
                stateInfo = "Высадка - игрок приземляется на поверхность";
                break;
            case SceneState.Exploration:
                stateInfo = "Исследование - игрок изучает местность";
                break;
            case SceneState.Quest:
                stateInfo = "Квест - игрок выполняет задание";
                break;
        }

        Debug.Log($"Текущий этап: {currentState} - {stateInfo}");
    }

    public SceneState GetCurrentState()
    {
        return currentState;
    }
}