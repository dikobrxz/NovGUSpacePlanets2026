using UnityEngine;

public class GameManager : MonoBehaviour
{
    private SceneManager sceneManager;
    private float timer = 0f;
    private float stageDuration = 10f;
    private bool isScenarioFinished = false;

    void Start()
    {
        sceneManager = GetComponent<SceneManager>();

        if (sceneManager != null)
        {
            sceneManager.StartScenario();
        }
    }

    void Update()
    {
        if (isScenarioFinished) return;

        timer += Time.deltaTime;

        if (timer >= stageDuration)
        {
            timer = 0f;

            if (sceneManager != null)
            {
                if (sceneManager.GetCurrentState() == SceneState.Quest)
                {
                    isScenarioFinished = true;
                    Debug.Log("Сценарий завершён! Дальнейших переходов не будет.");
                    return;
                }
                sceneManager.NextStage();
            }
        }
    }
}