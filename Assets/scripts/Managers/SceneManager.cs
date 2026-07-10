using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour
{
    public enum ScenarioState { Start, Orbit, Landing, Exploration, Cryovolcano, QuestComplete }

    private ScenarioState currentState = ScenarioState.Start;
    private ScenarioState[] stageOrder = new[]
    {
        ScenarioState.Start,
        ScenarioState.Landing,   
        ScenarioState.Orbit,
        ScenarioState.Exploration,
        ScenarioState.QuestComplete
    };

    [Header("Start Stage")]
    [SerializeField] private Transform plutoModel;
    [SerializeField] private AudioSource startAudioSource;
    [SerializeField] private float plutoRotationSpeed = 2f;
    private bool isStartStageActive = false;
    [SerializeField] private GameObject spaceship;

    [Header("Landing Stage")]
    [SerializeField] private GameObject terrainObject;
    [SerializeField] private AudioSource landingAudioSource;
    [SerializeField] private float landingAudioDelay = 2f;
    [SerializeField] private GameObject iceBlock;

    [Header("Orbit Stage (Харон)")]
    [SerializeField] private GameObject charonModel;
    [SerializeField] private AudioSource orbitAudioSource;
    [SerializeField] private float charonSpeed = 0.05f;
    [SerializeField] private Vector3 charonStartPosition = new Vector3(208.4f, 15f, 161.5f);
    [SerializeField] private Vector3 charonEndPosition = new Vector3(-34.3f, 15f, 98.6f);
    [SerializeField] private float charonArcHeight = 40f;
    private bool isCharonMoving = false;
    private float charonProgress = 0f;
    private Vector3 arcControlPoint;

    [Header("Exploration Stage")]
    [SerializeField] private GameObject pickaxe;
    [SerializeField] private GameObject bucket;
    [SerializeField] private GameObject remoteControl;
    [SerializeField] private GameObject probe;
    [SerializeField] private GameObject iceChunkPrefab;
    [SerializeField] private int hitsNeeded = 3;
    [SerializeField] private AudioSource explorationAudioSource;

    [Header("Probe Flight (как у Харона)")]
    [SerializeField] private Vector3 probeSpawnPosition = new Vector3(0, 30, 0);   
    [SerializeField] private Vector3 probeFlyEndPosition = new Vector3(0, 100, 200); 
    [SerializeField] private float probeSpeed = 8f;                                 
    private bool isProbeMoving = false;       
    private Vector3 probeTargetPos;           
    private int probeStep = 0;                

    [Header("Quest Complete Stage")]
    [SerializeField] private AudioSource questCompleteAudioSource;
    [SerializeField] private Transform playerReturnPoint;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private AudioSource finalVoiceOver; 

    [Header("Effects")]
    [SerializeField] private ParticleSystem iceHitParticles;  

    [Header("Audio")]
    [SerializeField] private AudioSource pickaxeHitAudioSource;  
    [SerializeField] private AudioClip pickaxeHitSound;
    [SerializeField] private AudioSource bucketAudioSource;     
    [SerializeField] private AudioClip iceInBucketSound;    

    [Header("Cryovolcano")]
    [SerializeField] private GameObject cryovolcano;
    [SerializeField] private GameObject activatorPrefab;
    [SerializeField] private Transform activatorSpawnPoint;
    private bool isCryovolcanoActivated = false;
    private bool isCryovolcanoSampleCollected = false;

    private bool isTransitioning = false;
    private int currentHits = 0;
    private bool isExplorationActive = false;
    private bool isIceInBucket = false;
    private bool isProbeDone = false;
    private bool isScenarioStarted = false;

    public bool IsScenarioStarted() => isScenarioStarted;

    private void Update()
    {
        if (isStartStageActive && plutoModel != null) plutoModel.Rotate(Vector3.up, plutoRotationSpeed * Time.deltaTime);
        if (isCharonMoving && charonModel != null) MoveCharon();

        if (isProbeMoving && probe != null)
        {
            probe.transform.position = Vector3.MoveTowards(probe.transform.position, probeTargetPos, probeSpeed * Time.deltaTime);
            
            if (Vector3.Distance(probe.transform.position, probeTargetPos) < 0.5f)
            {
                isProbeMoving = false;
                OnProbeReachedTarget(); 
            }
        }

        if (Time.time % 3 < 0.02f)
        {
            RecoverLostItems();
        }
    }

    private void RecoverLostItems()
    {
        Vector3 safePos = new Vector3(100, 5, 140); // Безопасная позиция
        
        if (pickaxe != null && pickaxe.activeInHierarchy && pickaxe.transform.position.y < -10)
        {
            pickaxe.transform.position = safePos;
            pickaxe.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            Debug.Log("Кирка возвращена!");
        }
        
        if (bucket != null && bucket.activeInHierarchy && bucket.transform.position.y < -10)
        {
            bucket.transform.position = safePos;
            bucket.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            Debug.Log("Ведро возвращено!");
        }
    }

    public void StartScenario()
    {
        isScenarioStarted = true; 
        currentState = stageOrder[0];
        LogCurrentState();
        ActivateStartStage();
    }

    private void ActivateStartStage()
    {
        isStartStageActive = true;
        if (startAudioSource != null && startAudioSource.clip != null) startAudioSource.Play();
        if (plutoModel != null) plutoModel.gameObject.SetActive(true);
        if (terrainObject != null) terrainObject.SetActive(false);
        if (probe != null) probe.SetActive(false); 
        if (charonModel != null) charonModel.SetActive(false);
        if (pickaxe != null) pickaxe.SetActive(false); 
        if (bucket != null) bucket.SetActive(false);
        if (remoteControl != null) remoteControl.SetActive(false);
        if (iceBlock != null) iceBlock.SetActive(false);
        if (cryovolcano != null) cryovolcano.SetActive(false);
        HUDManager.ShowHint("Нажмите ПРОБЕЛ для перехода к посадке");
    }

    private void DeactivateStartStage()
    {
        isStartStageActive = false;
        if (startAudioSource != null && startAudioSource.isPlaying) startAudioSource.Stop();
    }

    private IEnumerator TransitionToLanding()
    {
        isTransitioning = true;
        
        if (startAudioSource != null && startAudioSource.isPlaying) startAudioSource.Stop();
        if (plutoModel != null) plutoModel.gameObject.SetActive(false);
        if (spaceship != null) spaceship.SetActive(false);
        if (terrainObject != null) terrainObject.SetActive(true);
        if (iceBlock != null) iceBlock.SetActive(true);

        if (cryovolcano != null)
        {
            cryovolcano.SetActive(true);
            Debug.Log("✅ Криовулкан активирован на этапе Landing!");
        }

        if (activatorPrefab != null && activatorSpawnPoint != null)
        {
            GameObject activator = Instantiate(activatorPrefab, activatorSpawnPoint.position, activatorSpawnPoint.rotation);
            Debug.Log("✅ Активатор вулкана создан!");
        }

        currentState = ScenarioState.Landing;
        LogCurrentState();

        yield return new WaitForSeconds(landingAudioDelay);

        if (landingAudioSource != null && landingAudioSource.clip != null)
        {
            landingAudioSource.Play();
            Debug.Log("Landing аудио играет");
        }

        isTransitioning = false;
        HUDManager.ShowNotification("Посадка на Плутон...");
        HUDManager.ShowHint("Нажмите ПРОБЕЛ для перехода к орбите Харона");
    }

    private void ActivateOrbitStage()
    {
        if (charonModel != null)
        {
            charonModel.SetActive(true);
            charonProgress = 0f;
            arcControlPoint = ((charonStartPosition + charonEndPosition) * 0.5f) + Vector3.up * charonArcHeight;
            charonModel.transform.position = charonStartPosition;
            isCharonMoving = true;
        }
        if (orbitAudioSource != null) orbitAudioSource.Play();
        HUDManager.ShowHint("Наблюдайте за полётом Харона. ПРОБЕЛ — следующий этап");
    }

    private void MoveCharon()
    {
        charonProgress += charonSpeed * Time.deltaTime;
        if (charonProgress >= 1f) { charonProgress = 1f; isCharonMoving = false; }
        float t = charonProgress;
        float u = 1f - t;
        charonModel.transform.position = (u*u*charonStartPosition) + (2*u*t*arcControlPoint) + (t*t*charonEndPosition);
        charonModel.transform.Rotate(Vector3.up, 5f * Time.deltaTime);
    }

    private void DeactivateOrbitStage()
    {
        isCharonMoving = false;
        if (charonModel != null) charonModel.SetActive(false);
        if (orbitAudioSource != null && orbitAudioSource.isPlaying) orbitAudioSource.Stop();
    }

    public void AdvanceToNextStage()
    {
        if (isTransitioning) return;

        if (currentState == ScenarioState.Exploration)
        {
            if (!isProbeDone)
            {
                Debug.Log($"Миссия не завершена! Сначала собиерите лёд");
                HUDManager.ShowHint("Сначала откопайте лёд и соберите его зондом!");
                return;
            }
        }

        int currentIndex = System.Array.IndexOf(stageOrder, currentState);
        if (currentIndex < stageOrder.Length - 1)
        {
            if (currentState == ScenarioState.Start) { StartCoroutine(TransitionToLanding()); return; }

            currentState = stageOrder[currentIndex + 1];
            LogCurrentState();

            switch (currentState)
            {
                case ScenarioState.Orbit: ActivateOrbitStage(); break;
                case ScenarioState.Exploration: ActivateExplorationStage(); break;
                case ScenarioState.QuestComplete: ActivateQuestCompleteStage(); break;
            }
        }
        else
        {
            Debug.Log("Сценарий полностью завершён!");
        }
    }

    public void ActivateExplorationStage()
    {
        Debug.Log("АКТИВАЦИЯ EXPLORATION");
        isExplorationActive = true;
        currentHits = 0;
        isIceInBucket = false;
        isProbeDone = false; 
        isCryovolcanoActivated = false;
        isCryovolcanoSampleCollected = false;
        probeStep = 0;

        if (explorationAudioSource != null) explorationAudioSource.Play();
        if (pickaxe != null) pickaxe.SetActive(true);
        if (bucket != null) bucket.SetActive(false);
        if (remoteControl != null) remoteControl.SetActive(false);
        if (probe != null) probe.SetActive(false); 
        HUDManager.ShowHint("Откопайте лёд киркой (3 удара)");
    }

    public void RegisterIceHit(Vector3 hitPoint)  
    {
        if (!isExplorationActive) return;
        currentHits++;
        
        if (pickaxeHitAudioSource != null && pickaxeHitSound != null)
        {
            pickaxeHitAudioSource.PlayOneShot(pickaxeHitSound);
        }

        SpawnHitParticles(hitPoint);
        
        if (currentHits >= hitsNeeded)
        {
            SpawnIceChunk(hitPoint);  
            
            if (bucket != null) 
            {
                bucket.SetActive(true);
                Debug.Log("✅ Bucket активирован");
            }
            
            HUDManager.ShowHint("Лёд отколот! Положите его в таз");
        }
        else
        {
            HUDManager.ShowHint($"Удар: {currentHits}/{hitsNeeded}");
        }
    }

        private void SpawnHitParticles(Vector3 position)
    {
        if (iceHitParticles == null) return;
        
        GameObject particlesObj = Instantiate(iceHitParticles.gameObject, position, Quaternion.identity);
        ParticleSystem ps = particlesObj.GetComponent<ParticleSystem>();
        if (ps != null) ps.Play();
        
        Destroy(particlesObj, 1f);
    }

    private void SpawnIceChunk(Vector3 spawnPos)
    {
        Vector3 finalPos = spawnPos + Vector3.up * 0.5f + Vector3.forward * 0.3f;
        GameObject chunk = Instantiate(iceChunkPrefab, finalPos, Quaternion.identity);
        
        chunk.SetActive(true);
        Rigidbody rb = chunk.GetComponent<Rigidbody>();
        if (rb != null) 
        {
            rb.AddForce(Vector3.forward * 3f + Vector3.up * 1.5f, ForceMode.Impulse);
        }
        else
        {
            Debug.LogWarning("⚠️ Rigidbody НЕ найден на iceChunkPrefab!");
        }
    }

    public void OnIcePlacedInBucket()
    {
        if (!isExplorationActive) return;
        isIceInBucket = true;

        if (bucketAudioSource != null && iceInBucketSound != null)
        {
            bucketAudioSource.PlayOneShot(iceInBucketSound);
        }

        if (remoteControl != null)
        {
            remoteControl.SetActive(true);
        }
        HUDManager.ShowHint("Лёд в тазу! Возьмите пульт и нажмите кнопку");
    }

    public void OnRemoteButtonPressed()
    {
        if (isProbeMoving) 
        { 
            Debug.LogWarning("Зонд уже летит, жди."); 
            return; 
        }
        
        if (isIceInBucket && !isProbeDone)
        {
            Debug.Log("Запуск зонда за льдом...");
            StartProbeFlight(); 
            HUDManager.ShowHint("Зонд летит за льдом!");
            return;
        }
        
        if (isCryovolcanoActivated && !isCryovolcanoSampleCollected)
        {
            var volcano = FindAnyObjectByType<Cryovolcano>();
            if (volcano != null && volcano.IsSampleAvailable())
            {
                Debug.Log("Запуск зонда за образцом криовулкана...");
                
                if (probe != null && !probe.activeInHierarchy) 
                {
                    probe.SetActive(true);
                    Debug.Log("Зонд активирован досрочно для вулкана!");
                }
                
                StartCryovolcanoProbeFlight();
                HUDManager.ShowHint("Зонд летит за образцом криовулкана!");
                return;
            }
            else
            {
                Debug.LogWarning("⚠️ Образец криовулкана недоступен!");
                HUDManager.ShowHint("Сначала активируйте криовулкан!");
                return;
            }
        }
        
        if (isProbeDone && isCryovolcanoSampleCollected)
        {
            HUDManager.ShowHint("Все образцы уже собраны! Нажмите ПРОБЕЛ для продолжения");
            return;
        }
        
        HUDManager.ShowHint("Нечего собирать! Откопайте лёд или активируйте вулкан.");
    }

    private void StartProbeFlight()
    {
        if (probe == null) { Debug.LogError("Probe не назначен!"); return; }

        probe.transform.position = probeSpawnPosition;
        probe.SetActive(true);

        probeTargetPos = bucket != null ? bucket.transform.position + Vector3.up * 2f : probeSpawnPosition;
        probeStep = 1;             
        isProbeMoving = true;     
    }

    private void StartCryovolcanoProbeFlight()
    {
        if (probe == null) { Debug.LogError("Probe не назначен!"); return; }
        
        var volcano = FindAnyObjectByType<Cryovolcano>();
        if (volcano == null) { Debug.LogError("Cryovolcano не найден!"); return; }
        
        Transform targetPoint = volcano.GetProbeTargetPoint();
        if (targetPoint == null)
        {
            Debug.LogError("probeTargetPoint не назначен в Cryovolcano!");
            return;
        }
        
        probe.transform.position = targetPoint.position + Vector3.up * 5f;
        probe.SetActive(true);
        Debug.Log($"Зонд появился у криовулкана");
        
        probeTargetPos = targetPoint.position;
        probeStep = 10;
        isProbeMoving = true;
    }


    private void OnProbeReachedTarget()
    {
        if (probeStep == 1)
        {
            Debug.Log("Зонд у таза! Забираю образец льда...");
            if (bucket != null) bucket.SetActive(false);
            probeStep = 2;
            StartCoroutine(WaitBeforeFlyAway());
        }
        else if (probeStep == 3)
        {
            Debug.Log("Зонд улетел. Миссия со льдом завершена!");
            probe.SetActive(false);
            isProbeDone = true;
            CheckAllSamplesCollected();
        }
        else if (probeStep == 10)
        {
            Debug.Log("Зонд у криовулкана! Забираю образец азотного льда...");
            
            var volcano = FindAnyObjectByType<Cryovolcano>();
            if (volcano != null) volcano.CollectSample();
            
            probeStep = 11;
            StartCoroutine(WaitBeforeFlyAway());
        }
        else if (probeStep == 11)
        {
            Debug.Log("Зонд улетел. Миссия с криовулканом завершена!");
            probe.SetActive(false);
            isCryovolcanoSampleCollected = true;
            CheckAllSamplesCollected();
        }
    }

    public void OnCryovolcanoActivated()
    {
        isCryovolcanoActivated = true;
        Debug.Log("✅ Криовулкан активирован!");
        
        if (remoteControl != null && !remoteControl.activeInHierarchy)
        {
            remoteControl.SetActive(true);
            Debug.Log("✅ Пульт активирован!");
        }
    }

    public void OnCryovolcanoSampleCollected()
    {
        isCryovolcanoSampleCollected = true;
        Debug.Log("✅ Образец криовулкана получен!");
        CheckAllSamplesCollected();
    }

    private void CheckAllSamplesCollected()
    {
        if (isProbeDone && isCryovolcanoSampleCollected)
        {
            Debug.Log("✅ ВСЕ ОБРАЗЦЫ СОБРАНЫ (включая вулкан)!");
            HUDManager.ShowHint("Все образцы собраны! Отличная работа!");
        }
        else if (isProbeDone && !isCryovolcanoSampleCollected)
        {
            // ✅ Лёд собран — можно идти дальше, вулкан не обязателен
            Debug.Log("✅ Лёд собран! Вулкан опционален.");
            HUDManager.ShowHint("Лёд собран! Можно переходить к финалу (вулкан — бонус)");
        }
        else if (!isProbeDone && isCryovolcanoSampleCollected)
        {
            // ❗ Лёд ещё не собран — это обязательно
            HUDManager.ShowHint("Образец вулкана собран! Но лёд всё ещё нужно откопать");
        }
    }


    private IEnumerator WaitBeforeFlyAway()
    {
        yield return new WaitForSeconds(1.5f);
        probeTargetPos = probeFlyEndPosition; 
        probeStep = 3;                        
        isProbeMoving = true;                 
    }

    public void ActivateQuestCompleteStage()
    {
        
        if (pickaxe != null) pickaxe.SetActive(false);
        if (bucket != null) bucket.SetActive(false);
        if (remoteControl != null) remoteControl.SetActive(false);
        if (probe != null) probe.SetActive(false);
        if (terrainObject != null) terrainObject.SetActive(false);
        if (spaceship != null) spaceship.SetActive(true);
        if (plutoModel != null)
        {
            plutoModel.gameObject.SetActive(true);
            isStartStageActive = true;
        }

        if (cryovolcano != null)
        {
            cryovolcano.SetActive(false);
            Debug.Log("✅ Криовулкан деактивирован");
        }

        if (playerTransform != null && playerReturnPoint != null)
        {
            playerTransform.position = playerReturnPoint.position;
            Debug.Log("Игрок телепортирован на корабль.");
        }
        HUDManager.ShowHint("Добро пожаловать на борт! Отвечайте на вопросы квиза");

        QuizManager quiz = FindAnyObjectByType<QuizManager>();
        if (quiz != null && !quiz.IsRunning())
            quiz.StartQuiz();
    }

    public void PlayFinalVoiceOver()
    {
        if (finalVoiceOver != null && finalVoiceOver.clip != null)
        {
            finalVoiceOver.Play();
        }
    }

    public void LogCurrentState() => Debug.Log($"SceneManager текущий этап {currentState}");
    public ScenarioState GetCurrentState() => currentState;
}