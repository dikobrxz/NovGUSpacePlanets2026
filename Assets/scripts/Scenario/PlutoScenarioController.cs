using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class PlutoScenarioController : MonoBehaviour
{
    [Header("Ссылки на менеджеры")]
    [SerializeField] private SceneManager scenarioManager;
    [SerializeField] private GameManager gameManager;

    [Header("Аудио")]
    [SerializeField] private AudioSource voiceoverSource;
    [SerializeField] private AudioClip[] voiceoverClips; // 0-Start, 1-Landing, 2-Observation, 3-Quest, 4-Complete

    [Header("Визуал")]
    [SerializeField] private GameObject plutoModel;      
    [SerializeField] private GameObject playerCamera;    
    [SerializeField] private GameObject skyboxSpace;     
    [SerializeField] private GameObject landingZone;     
    [SerializeField] private GameObject iceMountains;    

    [Header("Интерактивные объекты квеста")]
    [SerializeField] private PickaxeController pickaxe;  
    [SerializeField] private IceSample iceSample;        
    [SerializeField] private Container container;        
    [SerializeField] private ProbeLauncher probeButton;  
    [SerializeField] private GameObject probePrefab;     

    [Header("Настройки")]
    [SerializeField] private float rotationSpeed = 2f;   
    [SerializeField] private float transitionDuration = 3f; 

    private SceneManager.ScenarioState lastState;
    private bool isTransitioning = false;

    private void Awake()
    {
        if (scenarioManager == null)
            scenarioManager = FindObjectOfType<SceneManager>();
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (isTransitioning) return;

        var currentState = scenarioManager?.GetCurrentState() ?? SceneManager.ScenarioState.Start;

        if (currentState != lastState)
        {
            HandleStageChange(currentState);
            lastState = currentState;
        }

        if (currentState == SceneManager.ScenarioState.Start && plutoModel != null)
        {
            plutoModel.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleStageChange(SceneManager.ScenarioState state)
    {
        StartCoroutine(ExecuteStage(state));
    }

    private IEnumerator ExecuteStage(SceneManager.ScenarioState state)
    {
        isTransitioning = true;

        switch (state)
        {
            case SceneManager.ScenarioState.Start:
                yield return Stage_Start();
                break;
            case SceneManager.ScenarioState.Orbit:
                yield return Stage_Orbit();
                break;
            case SceneManager.ScenarioState.Landing:
                yield return Stage_Landing();
                break;
            case SceneManager.ScenarioState.Exploration:
                yield return Stage_Exploration();
                break;
            case SceneManager.ScenarioState.QuestComplete:
                yield return Stage_Complete();
                break;
        }

        isTransitioning = false;
    }

    //Этап 1: Стартовый экран с вращающимся Плутоном
    private IEnumerator Stage_Start()
    {
        Debug.Log("🪐 Этап 1: Знакомство с Плутоном");
        
        SetVisibility(plutoModel, true);
        SetVisibility(landingZone, false);
        SetVisibility(iceMountains, false);
        
        if (skyboxSpace != null)
            RenderSettings.skybox = skyboxSpace.GetComponent<Material>();

        yield return FadeIn(plutoModel, transitionDuration);

        if (voiceoverSource != null && voiceoverClips.Length > 0)
        {
            voiceoverSource.clip = voiceoverClips[0];
            voiceoverSource.Play();
            yield return new WaitForSeconds(voiceoverClips[0].length);
        }

    }

    //Этап 2: Орбита / Подлёт
    private IEnumerator Stage_Orbit()
    {
        Debug.Log("🛰️ Этап 2: Орбита и подлёт");
        
        if (playerCamera != null)
        {
            Vector3 orbitPosition = new Vector3(0, 50, -100);
            yield return MoveCameraSmoothly(playerCamera.transform, orbitPosition, transitionDuration);
        }

        if (voiceoverSource != null && voiceoverClips.Length > 1)
        {
            voiceoverSource.clip = voiceoverClips[2]; // индекс 2 - Observation
            voiceoverSource.Play();
            yield return new WaitForSeconds(voiceoverClips[2].length);
        }
    }

    //Этап 3: Посадка на поверхность
    private IEnumerator Stage_Landing()
    {
        Debug.Log("🪂 Этап 3: Посадка на Плутон");
        
        SetVisibility(landingZone, true);
        SetVisibility(iceMountains, true);
        SetVisibility(plutoModel, false); 

        if (playerCamera != null)
        {
            Vector3 landingPosition = landingZone != null ? landingZone.transform.position : Vector3.zero;
            landingPosition += Vector3.up * 2f; 
            yield return MoveCameraSmoothly(playerCamera.transform, landingPosition, transitionDuration);
        }

        if (voiceoverSource != null && voiceoverClips.Length > 1)
        {
            voiceoverSource.clip = voiceoverClips[1];
            voiceoverSource.Play();
            yield return new WaitForSeconds(voiceoverClips[1].length);
        }

        ActivateQuestObjects(false); // пока скрываем, включим на этапе 4
    }

    //Этап 4: Исследовательский квест
    private IEnumerator Stage_Exploration()
    {
        Debug.Log("⛏️ Этап 4: Квест с образцом льда");
        
        //"Давай отколем кусочек льда..."
        if (voiceoverSource != null && voiceoverClips.Length > 3)
        {
            voiceoverSource.clip = voiceoverClips[3];
            voiceoverSource.Play();
            yield return new WaitForSeconds(voiceoverClips[3].length);
        }

        ActivateQuestObjects(true);

        SetupQuestEvents();

        yield return new WaitUntil(() => probeButton?.WasPressed == true);

        if (probePrefab != null)
        {
            GameObject probe = Instantiate(probePrefab, probeButton.transform.position, Quaternion.identity);
            yield return StartCoroutine(ProbeAnimation(probe));
        }
    }

    //Этап 5: Завершение
    private IEnumerator Stage_Complete()
    {
        Debug.Log("✅ Этап 5: Завершение квеста");
        
        //"Посмотрим, что мы узнаем..."
        if (voiceoverSource != null && voiceoverClips.Length > 4)
        {
            voiceoverSource.clip = voiceoverClips[4];
            voiceoverSource.Play();
            yield return new WaitForSeconds(voiceoverClips[4].length);
        }

        if (playerCamera != null && landingZone != null)
        {
            Vector3 startPos = new Vector3(0, 10, -50); 
            yield return MoveCameraSmoothly(playerCamera.transform, startPos, transitionDuration);
        }

        UIManager.Instance?.ShowScreen("QuestComplete");

        gameManager?.Invoke("SetFinished", 2f); 
    }

    //Вспомогательные методы
    private void SetVisibility(GameObject obj, bool visible)
    {
        if (obj != null)
            obj.SetActive(visible);
    }

    private IEnumerator FadeIn(GameObject obj, float duration)
    {
        var renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            Color color = renderer.material.color;
            color.a = 0f;
            renderer.material.color = color;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                color.a = Mathf.Lerp(0f, 1f, elapsed / duration);
                renderer.material.color = color;
                yield return null;
            }
        }
    }

    private IEnumerator MoveCameraSmoothly(Transform cam, Vector3 target, float duration)
    {
        Vector3 start = cam.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cam.position = Vector3.Lerp(start, target, elapsed / duration);
            yield return null;
        }
        cam.position = target;
    }

    private void ActivateQuestObjects(bool active)
    {
        if (pickaxe != null) pickaxe.gameObject.SetActive(active);
        if (iceSample != null) iceSample.gameObject.SetActive(active);
        if (container != null) container.gameObject.SetActive(active);
        if (probeButton != null) probeButton.gameObject.SetActive(active);
    }

    private void SetupQuestEvents()
    {
        if (iceSample != null)
        {
            iceSample.OnCollected.AddListener(() => 
            {
                Debug.Log("Образец льда поднят!");
            });
        }

        if (container != null)
        {
            container.OnSamplePlaced.AddListener(() => 
            {
                Debug.Log("📦 Образец помещён в контейнер!");
                probeButton?.EnableButton(true); 
            });
        }
    }

    private IEnumerator ProbeAnimation(GameObject probe)
    {
        Vector3 samplePos = iceSample?.transform.position ?? Vector3.zero;
        Vector3 flyAway = samplePos + Vector3.up * 100f;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 2f;
            probe.transform.position = Vector3.Lerp(probe.transform.position, samplePos, t);
            yield return null;
        }

        if (iceSample != null) iceSample.gameObject.SetActive(false);

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 1.5f;
            probe.transform.position = Vector3.Lerp(samplePos, flyAway, t);
            yield return null;
        }

        Destroy(probe);
    }
}