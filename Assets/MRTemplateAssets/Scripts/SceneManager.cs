using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public enum ScenarioState   //состояния сцены
    {
        Start,
        Orbit,
        Landing,
        Exploration,                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   
        QuestComplete
    }

    private ScenarioState currentState = ScenarioState. Start;
    private ScenarioState[] stageOrder = new[]
    {
        ScenarioState.Start,
        ScenarioState.Orbit,
        ScenarioState.Landing,
        ScenarioState.Exploration,
        ScenarioState.QuestComplete
    };

    private void Start()
    {
        LogCurrentState();  //логируем текущее состояние
    }

    public void StartScenario()     //запуск сценария
    {
        currentState = stageOrder[0];
        LogCurrentState();
    }

    public void AdvanceToNextStage()        //след этап
    {
        int currentIndex = System.Array.IndexOf(stageOrder, currentState);
        if (currentIndex < stageOrder.Length - 1)
        {
            currentState = stageOrder[currentIndex + 1];
            LogCurrentState();      //вывод при изменении состояния
        }
        else
        {
            Debug.Log("SceneManager сценарий завершён. Все этапы пройдены!!!");
        }
    }

    public void LogCurrentState()
    {
        Debug.Log($"SceneManager текущий этап {currentState}");
    }

    public ScenarioState GetCurrentState() => currentState;     //геттер для доступа из других менеджеров
}
