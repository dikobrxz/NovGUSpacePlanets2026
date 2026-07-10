using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public enum AppState { NotStarted, Running, Paused, Finished } //перечисление состояния приложения
    private AppState appState = AppState.NotStarted;

    [SerializeField] private SceneManager SceneManager; //ссылка на scene manager

    private void Awake()
    {
        if (SceneManager == null)    //автопоиск если инспектор подтупил
        {
            SceneManager = GetComponent<SceneManager>();
        }
    }

    private void Start()
    {
        SetAppState(AppState.Running);
        Debug.Log("GameManager Entry Point");

        if (SceneManager != null)    //запуск при старте
        {
            SceneManager.StartScenario();
        }
        else
        {
            Debug.LogError("GameManager нет ссылки на SceneManager");
        }
    }

    private void SetAppState(AppState newState)     //управление состоянием
    {
        appState = newState;
        Debug.Log($"GameManager: {appState}");
    }


    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            var sceneManager = FindAnyObjectByType<SceneManager>();
            if (sceneManager == null) return;

            // Старт сценария
            if (!sceneManager.IsScenarioStarted())
            {
                sceneManager.StartScenario();
                return;
            }

            // Блокируем пробел, если квиз ещё идёт
            var quiz = FindAnyObjectByType<QuizManager>();
            if (quiz != null && quiz.IsRunning())
            {
                return; // Игнорируем нажатие
            }

            // Блокируем пробел, если финальный этап уже активен
            if (sceneManager.GetCurrentState() == SceneManager.ScenarioState.QuestComplete)
            {
                return; // Игнорируем нажатие
            }

            sceneManager.AdvanceToNextStage();
        }
    }
    
    public void SetFinished()
    {
        SetAppState(AppState.Finished);
    }
}
