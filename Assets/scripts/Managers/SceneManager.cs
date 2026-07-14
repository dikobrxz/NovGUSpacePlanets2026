using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Unity.XR.CoreUtils;

namespace Tour_ENDI_TourStub5
{

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
        [SerializeField] private Transform charonStartPosition;// = new Vector3(208.4f, 15f, 161.5f);
        [SerializeField] private Transform charonEndPosition;// = new Vector3(-34.3f, 15f, 98.6f);
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

        [Header("Quest Complete Stage")]
        [SerializeField] private AudioSource questCompleteAudioSource;
        [SerializeField] private Transform playerReturnPoint;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private AudioSource finalVoiceOver;

        [Header("Effects")]
        [SerializeField] private ParticleSystem iceHitParticles;

        [Header("Audio")]
        [SerializeField] private AudioSource _uiAudio;
        [SerializeField] private AudioClip _uiHintClip;
        [SerializeField] private AudioSource pickaxeHitAudioSource;
        [SerializeField] private AudioClip pickaxeHitSound;
        [SerializeField] private AudioSource bucketAudioSource;
        [SerializeField] private AudioClip iceInBucketSound;
        [SerializeField] private AudioClip _startIntroClip;
        [SerializeField] private AudioClip _landingFactClip;
        [SerializeField] private AudioClip _observationFactClip;
        [SerializeField] private AudioClip _questInstructionClip;
        [SerializeField] private AudioClip _questCompleteClip;

        [Header("Cryovolcano")]
        [SerializeField] private GameObject cryovolcano;
        [SerializeField] private GameObject activatorPrefab;
        [SerializeField] private Transform activatorSpawnPoint;

        [Space, Header("Other")]
        [SerializeField] private GameObject _playerMove;
        [SerializeField] private GameObject _hintMine;
        [SerializeField] private GameObject _hintGrab;
        [SerializeField] private GameObject _hintProbe;
        [SerializeField] private GameObject _hintComplete;
        [SerializeField] private XRGrabInteractable _axeInteract;
        [SerializeField] private XRGrabInteractable _remoteInteract;
        [SerializeField] private GameObject _world;
        [SerializeField] private GameObject _space;
        [SerializeField] private XROrigin _xrOrigin;
        [SerializeField] private Transform _worldPosition;
        [SerializeField] private Transform _spacePosition;
        [SerializeField] private GameObject _platform;

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
            plutoModel.Rotate(Vector3.up, plutoRotationSpeed * Time.deltaTime);
            if (isCharonMoving && charonModel != null) MoveCharon();
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
            if (startAudioSource != null && startAudioSource.clip != null) startAudioSource.PlayOneShot(_startIntroClip);

            StartCoroutine(StartLandingDelayCoroutine(_startIntroClip.length));
        }

        private IEnumerator StartLandingDelayCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay + 1f);
            AdvanceToNextStage();

            _playerMove.SetActive(true);

            startAudioSource.PlayOneShot(_landingFactClip);
            yield return new WaitForSeconds(_landingFactClip.length + 1f);
            AdvanceToNextStage();

            startAudioSource.PlayOneShot(_observationFactClip);
            yield return new WaitForSeconds(_observationFactClip.length + 1f);
            AdvanceToNextStage();

            startAudioSource.PlayOneShot(_questInstructionClip);
            _hintMine.SetActive(true);
            _uiAudio.PlayOneShot(_uiHintClip);

            _axeInteract.enabled = true;
            _remoteInteract.enabled = true;

            yield return new WaitForSeconds(_questInstructionClip.length + 1f);
            AdvanceToNextStage();
        }

        public void RestartStage()
        {
            _space.SetActive(false);
            _world.SetActive(true);
            HideAllHints();
            _platform.SetActive(false);

            StartCoroutine(RestartStageCoroutine());
        }

        private IEnumerator RestartStageCoroutine()
        {
            yield return null;

            _xrOrigin.MoveCameraToWorldLocation(_worldPosition.transform.position);
            _xrOrigin.MatchOriginUpCameraForward(Vector3.up, _worldPosition.transform.forward.normalized);
            _playerMove.SetActive(true);

            yield return new WaitForSeconds(1f);

            startAudioSource.PlayOneShot(_landingFactClip);
            yield return new WaitForSeconds(_landingFactClip.length + 1f);

            startAudioSource.PlayOneShot(_observationFactClip);
            yield return new WaitForSeconds(_observationFactClip.length + 3f);

            _platform.SetActive(true);
        }

        public void SetGrabHint()
        {
            _uiAudio.PlayOneShot(_uiHintClip);
            HideAllHints();
            _hintGrab.SetActive(true);
        }

        public void SetProbeHint()
        {
            _uiAudio.PlayOneShot(_uiHintClip);
            HideAllHints();
            _hintProbe.SetActive(true);
        }

        public void SetCompleteHint()
        {
            _uiAudio.PlayOneShot(_uiHintClip);
            HideAllHints();
            _hintComplete.SetActive(true);
        }

        public void HideAllHints()
        {
            _hintMine.SetActive(false);
            _hintGrab.SetActive(false);
            _hintProbe.SetActive(false);
            _hintComplete.SetActive(false);
        }

        private IEnumerator TransitionToLanding()
        {
            isTransitioning = true;

            if (startAudioSource != null && startAudioSource.isPlaying) startAudioSource.Stop();
            _space.SetActive(false);
            _world.SetActive(true);

            _xrOrigin.MoveCameraToWorldLocation(_worldPosition.transform.position);
            _xrOrigin.MatchOriginUpCameraForward(Vector3.up, _worldPosition.transform.forward.normalized);

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
                //landingAudioSource.Play();
                Debug.Log("Landing аудио играет");
            }

            isTransitioning = false;
        }

        public void TeleportPlayerToShip()
        {
            _space.SetActive(true);
            _world.SetActive(false);
            probe.SetActive(false);
            StartCoroutine(TransitionCoroutine());

            _playerMove.SetActive(false);

            QuizManager quiz = FindAnyObjectByType<QuizManager>();
            if (quiz != null && !quiz.IsRunning())
                quiz.StartQuiz();
        }

        private IEnumerator TransitionCoroutine()
        {
            yield return null;

            _xrOrigin.MoveCameraToWorldLocation(_spacePosition.transform.position);
            _xrOrigin.MatchOriginUpCameraForward(Vector3.up, _spacePosition.transform.forward.normalized);
        }

        public void CompleteProbeQuest()
        {
            startAudioSource.PlayOneShot(_questCompleteClip);
            _platform.SetActive(true);
            SetCompleteHint();
        }

        private void ActivateOrbitStage()
        {
            if (charonModel != null)
            {
                charonModel.SetActive(true);
                charonProgress = 0f;
                arcControlPoint = ((charonStartPosition.position + charonEndPosition.position) * 0.5f) + Vector3.up * charonArcHeight;
                charonModel.transform.position = charonStartPosition.position;
                isCharonMoving = true;
            }
            //if (orbitAudioSource != null) orbitAudioSource.Play();
        }

        private void MoveCharon()
        {
            charonProgress += charonSpeed * Time.deltaTime;
            if (charonProgress >= 1f) { charonProgress = 1f; isCharonMoving = false; }
            float t = charonProgress;
            float u = 1f - t;
            charonModel.transform.position = (u * u * charonStartPosition.position) + (2 * u * t * arcControlPoint) + (t * t * charonEndPosition.position);
            charonModel.transform.Rotate(Vector3.up, 5f * Time.deltaTime);
        }

        public void AdvanceToNextStage()
        {
            if (isTransitioning) return;

            if (currentState == ScenarioState.Exploration)
            {
                if (!isProbeDone)
                {
                    Debug.Log($"Миссия не завершена! Сначала собиерите лёд");
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

            //if (explorationAudioSource != null) explorationAudioSource.Play();
            if (pickaxe != null) pickaxe.SetActive(true);
            //if (bucket != null) bucket.SetActive(false);
            //if (remoteControl != null) remoteControl.SetActive(false);
            //if (probe != null) probe.SetActive(false);
        }

        public void ActivateQuestCompleteStage()
        {

            if (pickaxe != null) pickaxe.SetActive(false);
            if (bucket != null) bucket.SetActive(false);
            if (remoteControl != null) remoteControl.SetActive(false);
            //if (probe != null) probe.SetActive(false);
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

            QuizManager quiz = FindAnyObjectByType<QuizManager>();
            if (quiz != null && !quiz.IsRunning())
                quiz.StartQuiz();
        }

        public void LogCurrentState() => Debug.Log($"SceneManager текущий этап {currentState}");
    }

}