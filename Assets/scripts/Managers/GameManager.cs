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

    private void Update()       //тестовое управление для переключения этапов
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (SceneManager != null)
            {
                SceneManager.AdvanceToNextStage();
            }
        }
    }

    public void SetFinished()
    {
        SetAppState(AppState.Finished);
    }
}
