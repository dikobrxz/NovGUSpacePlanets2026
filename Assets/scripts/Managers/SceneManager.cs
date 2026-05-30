using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour
{
    public enum ScenarioState
    {
        Start,
        Orbit,
        Landing,
        Exploration,
        QuestComplete
    }

    private ScenarioState currentState = ScenarioState.Start;
    private ScenarioState[] stageOrder = new[]
    {
        ScenarioState.Start,
        ScenarioState.Landing,
        ScenarioState.Orbit,
        ScenarioState.Exploration,
        ScenarioState.QuestComplete
    };

    [Header("Start Stage References")]
    [SerializeField] private Transform plutoModel;
    [SerializeField] private AudioSource startAudioSource;
    [SerializeField] private float plutoRotationSpeed = 2f;

    [Header("Landing Stage")]
    [SerializeField] private GameObject terrainObject;
    [SerializeField] private AudioSource landingAudioSource;
    [SerializeField] private float landingAudioDelay = 2f;

    [Header("Spaceship")]
    [SerializeField] private GameObject spaceship;

    [Header("Orbit Stage (Харон)")]
    [SerializeField] private GameObject charonModel;           // Модель Харона
    [SerializeField] private AudioSource orbitAudioSource;     // Аудио для этапа наблюдения
    [SerializeField] private float charonSpeed = 0.05f;         // Скорость движения (градусы/кадр)
    [SerializeField] private Vector3 charonStartPosition = new Vector3(208.4f, 10f, 161.5f);
    [SerializeField] private Vector3 charonEndPosition = new Vector3(-34.3f, 10f, 98.6f);
    [SerializeField] private float charonArcHeight = 40f;     //  Высота пика дуги над средней точкой



    private bool isCharonMoving = false;
    private float charonProgress = 0f;
    private Vector3 arcControlPoint; // Рассчитывается автоматически


    private bool isStartStageActive = false;
    private bool isTransitioning = false;

    private void Start()
    {
        LogCurrentState();
    }

    private void Update()
    {
        if (isStartStageActive && plutoModel != null)
        {
            RotatePluto();
        }

                // Движение Харона
        if (isCharonMoving && charonModel != null)
        {
            MoveCharon();
        }
    }

    public void StartScenario()
    {
        currentState = stageOrder[0];
        LogCurrentState();
        
        if (currentState == ScenarioState.Start)
        {
            ActivateStartStage();   
        }
    }

    private void ActivateStartStage()
    {
        isStartStageActive = true;
        
        if (startAudioSource != null && startAudioSource.clip != null)
        {
            startAudioSource.Play();
            Debug.Log("▶️ Play() вызван");
        }
        
        if (plutoModel != null)
            plutoModel.gameObject.SetActive(true);
            
        if (terrainObject != null)
            terrainObject.SetActive(false); // Скрываем Terrain на старте
    }

    private void RotatePluto()
    {
        plutoModel.Rotate(Vector3.up, plutoRotationSpeed * Time.deltaTime);
    }

    private void DeactivateStartStage()
    {
        isStartStageActive = false;
        
        if (startAudioSource != null && startAudioSource.isPlaying)
        {
            startAudioSource.Stop();
        }
    }

    public void AdvanceToNextStage()
    {
        Debug.Log("⬇️ AdvanceToNextStage вызван! Текущий этап: " + currentState);

        if (isTransitioning)
        {
            Debug.LogWarning("⏳ Переход уже в процессе, игнорирую нажатие!");
            return;
        }

        // ПРОВЕРКА: мы точно на старте?
        if (currentState == ScenarioState.Start)
        {
            Debug.Log("🚀 Запускаю корутину перехода на Landing...");
            StartCoroutine(TransitionToLanding());
            return; // Важно выйти здесь, чтобы не сработал нижний код
        }


        // Останавливаем Харона при уходе с этапа Orbit
        if (currentState == ScenarioState.Orbit)
        {
            DeactivateOrbitStage();
        }


        // Если не Start, то обычная смена этапа
        int currentIndex = System.Array.IndexOf(stageOrder, currentState);
        if (currentIndex < stageOrder.Length - 1)
        {
            currentState = stageOrder[currentIndex + 1];
            LogCurrentState();

            if (currentState == ScenarioState.Orbit)
            {
            ActivateOrbitStage();
            }
        }
        else
        {
            Debug.Log("🏁 Сценарий завершён!");
        }
    }

    private IEnumerator TransitionToLanding()
    {
        isTransitioning = true;
        Debug.Log("🔄 Переход к высадке на Плутон...");

        // 1. Деактивируем стартовую сцену
        DeactivateStartStage();

        // 2. Скрываем Плутон
        if (plutoModel != null)
        {
            plutoModel.gameObject.SetActive(false);
            Debug.Log("🪐 PlutoModel скрыт");
        }

        // 2.5. Скрываем корабль
        if (spaceship != null)
        {
            spaceship.SetActive(false);
            Debug.Log("🚀 Spaceship скрыт");
        }

        // 3. Показываем Terrain
        if (terrainObject != null)
        {
            terrainObject.SetActive(true);
            Debug.Log("🌍 Terrain активирован");
        }

        // 4. Переходим к следующему этапу
        int currentIndex = System.Array.IndexOf(stageOrder, currentState);
        if (currentIndex < stageOrder.Length - 1)
        {
            currentState = stageOrder[currentIndex + 1];
            LogCurrentState();
        }

        // 5. Ждём и запускаем аудио
        yield return new WaitForSeconds(landingAudioDelay);

        if (landingAudioSource != null && landingAudioSource.clip != null)
        {
            landingAudioSource.Play();
            Debug.Log("🔊 Воспроизводится: 1_LandingFact");
        }
        else
        {
            Debug.LogWarning("⚠️ Не назначен landingAudioSource или AudioClip!");
        }

        isTransitioning = false;
    }


    private void ActivateOrbitStage()
    {
        Debug.Log("🌑 Активация этапа: Наблюдение за Плутоном");
        
        if (charonModel != null)
        {
            charonModel.SetActive(true);
            charonProgress = 0f;

            Vector3 midPoint = (charonStartPosition + charonEndPosition) * 0.5f;
            arcControlPoint = new Vector3(midPoint.x, midPoint.y + charonArcHeight, midPoint.z);

            charonModel.transform.position = charonStartPosition;
            isCharonMoving = true;
            Debug.Log($"🌑 Харон стартовал с позиции {charonStartPosition}");
        }
        
        if (orbitAudioSource != null && orbitAudioSource.clip != null)
        {
            orbitAudioSource.Play();
            Debug.Log("🔊 Воспроизводится: 2_ObservationFacts");
        }
    }

    private void MoveCharon()
    {
        charonProgress += charonSpeed * Time.deltaTime;
        
        if (charonProgress >= 1f)
        {
            charonProgress = 1f;
            isCharonMoving = false;
            Debug.Log("🌑 Харон достиг конечной точки");
        }
        
        // ⬇️ КВАДРАТИЧНАЯ КРИВАЯ БЕЗЬЕ (3 точки: старт, пик, финиш)
        float t = charonProgress;
        float u = 1f - t;
        
        Vector3 pos = (u * u * charonStartPosition) + 
                    (2 * u * t * arcControlPoint) + 
                    (t * t * charonEndPosition);
                    
        charonModel.transform.position = pos;
        
        // Харон медленно вращается вокруг своей оси для реалистичности
        charonModel.transform.Rotate(Vector3.up, 5f * Time.deltaTime);
        
    }

    private void DeactivateOrbitStage()
    {
        isCharonMoving = false;
        
        if (charonModel != null)
            charonModel.SetActive(false);
        
        if (orbitAudioSource != null && orbitAudioSource.isPlaying)
            orbitAudioSource.Stop();
        
        Debug.Log("🌑 Этап Orbit деактивирован");
    }



    public void LogCurrentState()
    {
        Debug.Log($"SceneManager текущий этап {currentState}");
    }

    public ScenarioState GetCurrentState() => currentState;
}