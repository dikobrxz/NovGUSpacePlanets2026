using UnityEngine;

public class GameManager : MonoBehaviour
{
    private ScenarioManager scenarioManager;
    private float stageTimer = 0f;
    private float questTimer = 0f;
    private bool isScenarioFinished = false;
    private bool isQuestActive = false;
    private bool isDrawing = false;

    [SerializeField] private float questDrawingTime = 90f;

    void Start()
    {
        scenarioManager = GetComponent<ScenarioManager>();
        if (scenarioManager == null) return;

        scenarioManager.StartScenario();
        Debug.Log($"[GameManager] Сценарий запущен");
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
        stageTimer += Time.deltaTime;

        float duration = scenarioManager.GetStageDuration(currentState);

        if (stageTimer >= duration)
        {
            stageTimer = 0f;

            if (currentState == SceneState.Return)
            {
                isScenarioFinished = true;
                Debug.Log("[GameManager] Сценарий завершён!");
                return;
            }

            scenarioManager.NextStage();
            Debug.Log($"[GameManager] Переход на этап: {scenarioManager.GetCurrentState()}");
        }
    }

    private void HandleQuestStage()
    {
        if (!isQuestActive)
        {
            stageTimer += Time.deltaTime;

            if (stageTimer >= 4f)
            {
                stageTimer = 0f;
                isQuestActive = true;
                isDrawing = true;
                Debug.Log("[GameManager] Начало рисования! 90 секунд");
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
        isDrawing = false;
        isQuestActive = false;
        questTimer = 0f;
        stageTimer = 0f;

        scenarioManager.NextStage();
        Debug.Log("[GameManager] Квест завершён. Переход на возвращение.");
    }

    public void OnSendDrawingButtonPressed()
    {
        if (scenarioManager.GetCurrentState() == SceneState.Quest && isDrawing)
        {
            Debug.Log("[GameManager] Рисунок отправлен досрочно!");
            CompleteQuest();
        }
    }
}