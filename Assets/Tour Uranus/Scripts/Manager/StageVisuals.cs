using System.Collections;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;

namespace Tour_ENDI_PlanetsUranus
{

    public class StageVisuals : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] private ScenarioManager scenarioManager;

        [Header("Environment")]
        public GameObject uranusPlanet;
        public GameObject terrain;
        public GameObject spaceship;

        [Header("Effects")]
        public ParticleSystem windParticles;

        [Header("Wind Audio")]
        public AudioSource windAudioSource;
        public AudioClip windClip;
        public float windVolume = 0.35f;

        [Header("VolClouds")]
        public GameObject volCloudsObject;

        [Header("Quest")]
        public GameObject drawingBoard;
        public GameObject markerCub;
        public GameObject hints;

        [Header("UI")]
        public GameObject sendButton;
        public GameObject actionButton;
        public GameObject returnPlatform;

        [Header("Button Labels")]
        [SerializeField] private string startButtonLabel;

        [Header("Quiz")]
        public GameObject quizCanvas;
        public QuizManager quizManager;

        [Header("Lighting")]
        public GameObject directionalLight;

        [Header("Audio")]
        public AudioSource voiceOver;
        public AudioClip introductionClip;
        public AudioClip landingClip;
        public AudioClip explorationClip;
        public AudioClip historicalClip;
        public AudioClip questClip;
        public AudioClip returnClip;

        private XROrigin xrOrigin;
        private CharacterController characterController;
        private SceneState currentStage;
        private GameObject locomotionFolder;
        private bool isPlanetRotating;

        private readonly Vector3 startPosition = new Vector3(-3.1f, 100f, -15f);
        private readonly Vector3 landingPosition = new Vector3(9f, 96.5f, -146f);
        private readonly Quaternion startRotation = Quaternion.identity;

        void Start()
        {
            xrOrigin = FindFirstObjectByType<XROrigin>();
            characterController = xrOrigin.GetComponent<CharacterController>();

            Transform loco = xrOrigin.transform.Find("Locomotion");
            locomotionFolder = loco.gameObject;

            uranusPlanet.transform.localScale = Vector3.one * 100f;
            uranusPlanet.transform.rotation = Quaternion.Euler(0f, 0f, 25f);

            sendButton.SetActive(false);
            returnPlatform.SetActive(false);
            volCloudsObject.SetActive(false);

            SetupActionButton(startButtonLabel, OnActionButtonPressed);

            HideAll();
            StartCoroutine(TeleportToStart());
            currentStage = SceneState.Start;
            ShowStartStage();
        }

        private void SetupActionButton(string text, UnityEngine.Events.UnityAction callback)
        {
            if (actionButton == null) return;

            Button btn = actionButton.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(callback);

            TMP_Text label = actionButton.GetComponentInChildren<TMP_Text>();
            label.text = text;

            actionButton.SetActive(true);

            Canvas canvas = actionButton.GetComponentInParent<Canvas>();
            canvas.gameObject.SetActive(true);

            CanvasGroup group = actionButton.GetComponentInParent<CanvasGroup>();
            if (group != null)
            {
                group.alpha = 1f;
                group.interactable = true;
                group.blocksRaycasts = true;
            }
        }

        private IEnumerator TeleportToStart()
        {
            yield return null;
            TeleportToPosition(startPosition, startRotation, false);
        }

        void Update()
        {
            if (scenarioManager == null) return;

            SceneState newStage = scenarioManager.GetCurrentState();
            if (currentStage != newStage)
            {
                currentStage = newStage;
                UpdateStage(newStage);
            }

            if (isPlanetRotating)
                uranusPlanet.transform.Rotate(Vector3.up, 5f * Time.deltaTime);
        }

        private void UpdateStage(SceneState stage)
        {
            switch (stage)
            {
                case SceneState.Start: ShowStartStage(); break;
                case SceneState.Introduction: ShowIntroductionStage(); break;
                case SceneState.Landing: ShowLandingStage(); break;
                case SceneState.Exploration: ShowExplorationStage(); break;
                case SceneState.Historical: ShowHistoricalStage(); break;
                case SceneState.Quest: ShowQuestStage(); break;
                case SceneState.Return: ShowReturnStage(); break;
                case SceneState.End: ShowEndStage(); break;
            }
        }

        private void HideAll()
        {
            uranusPlanet.SetActive(false);
            terrain.SetActive(false);
            spaceship.SetActive(false);
            drawingBoard.SetActive(false);
            markerCub.SetActive(false);
            directionalLight.SetActive(false);
            returnPlatform.SetActive(false);
            sendButton.SetActive(false);
            quizCanvas.SetActive(false);
            actionButton.SetActive(false);
            volCloudsObject.SetActive(false);
            windParticles.Stop();
            isPlanetRotating = false;
            volCloudsObject.SetActive(false);
            hints.SetActive(false);
        }

        private void SetLocomotion(bool enabled)
        {
            if (characterController != null)
            {
                characterController.enabled = enabled;
                characterController.detectCollisions = enabled;
            }
            if (locomotionFolder != null) locomotionFolder.SetActive(enabled);
        }

        private void TeleportToPosition(Vector3 position, Quaternion rotation, bool enableLoco = true)
        {
            SetLocomotion(false);

            Transform offset = xrOrigin.GetComponentInChildren<Camera>().transform.parent;
            float yOffset = xrOrigin.CameraYOffset;

            xrOrigin.transform.SetPositionAndRotation(position, rotation);
            offset.SetPositionAndRotation(position, rotation);
            offset.localPosition = new Vector3(0, 1.7f, 0);
            xrOrigin.CameraYOffset = 1.7f;

            if (enableLoco) SetLocomotion(true);
        }

        private void ShowStartStage()
        {
            HideAll();
            SetLocomotion(false);

            spaceship.SetActive(true);
            directionalLight.SetActive(true);
            uranusPlanet.SetActive(true);
            isPlanetRotating = true;

            SetupActionButton(startButtonLabel, OnActionButtonPressed);
            SetWindAudio(false);
        }

        private void ShowIntroductionStage()
        {
            HideAll();
            SetLocomotion(false);
            spaceship.SetActive(true);
            directionalLight.SetActive(true);
            uranusPlanet.SetActive(true);
            isPlanetRotating = true;
            actionButton.SetActive(false);
            PlayClip(introductionClip);
        }

        private void ShowLandingStage()
        {
            HideAll();
            TeleportToPosition(landingPosition, startRotation, true);
            SetLocomotion(true);
            SetWindAudio(true);
            terrain.SetActive(true);
            directionalLight.SetActive(true);
            windParticles.Play();
            volCloudsObject.SetActive(true);
            PlayClip(landingClip);
        }

        private void ShowExplorationStage()
        {
            HideAll();
            terrain.SetActive(true);
            directionalLight.SetActive(true);
            windParticles.Play();
            volCloudsObject.SetActive(true);
            PlayClip(explorationClip);
        }

        private void ShowHistoricalStage()
        {
            HideAll();
            terrain.SetActive(true);
            directionalLight.SetActive(true);
            windParticles.Play();
            volCloudsObject.SetActive(true);
            PlayClip(historicalClip);
        }

        private void ShowQuestStage()
        {
            HideAll();
            terrain.SetActive(true);
            directionalLight.SetActive(true);
            drawingBoard.SetActive(true);
            markerCub.SetActive(true);
            sendButton.SetActive(true);
            hints.SetActive(true);
            windParticles.Play();
            volCloudsObject.SetActive(true);
            PlayClip(questClip);
        }

        private void ShowReturnStage()
        {
            HideAll();
            terrain.SetActive(true);
            directionalLight.SetActive(true);
            returnPlatform.SetActive(true);
            windParticles.Play();
            volCloudsObject.SetActive(true);
            PlayClip(returnClip);
        }

        private void ShowEndStage()
        {
            HideAll();
            SetLocomotion(false);
            spaceship.SetActive(true);
            directionalLight.SetActive(true);
            uranusPlanet.SetActive(true);
            isPlanetRotating = true;
            actionButton.SetActive(false);
            quizCanvas.SetActive(true);
            SetWindAudio(false);
            volCloudsObject.SetActive(false);

            quizManager?.StartQuiz();
        }

        private void PlayClip(AudioClip clip)
        {
            if (clip == null) return;
            voiceOver.Stop();
            voiceOver.clip = clip;
            voiceOver.Play();
        }

        private void SetWindAudio(bool enable)
        {
            if (windAudioSource == null) return;

            if (enable && !windAudioSource.isPlaying && windClip != null)
            {
                windAudioSource.clip = windClip;
                windAudioSource.volume = windVolume;
                windAudioSource.loop = true;
                windAudioSource.Play();
            }
            else if (!enable && windAudioSource.isPlaying)
            {
                windAudioSource.Stop();
            }
        }

        public void OnSendButtonPressed()
        {
            if (scenarioManager.GetCurrentState() != SceneState.Quest) return;
            FindFirstObjectByType<GameManager>()?.OnSendDrawingButtonPressed();
            sendButton.SetActive(false);
        }

        public void OnActionButtonPressed()
        {
            if (scenarioManager.GetCurrentState() == SceneState.Start)
            {
                scenarioManager.NextStage();
                actionButton.SetActive(false);
            }
        }

        public void TeleportPlayerToShip()
        {
            if (currentStage != SceneState.Return) return;
            TeleportToPosition(startPosition, startRotation, true);
            returnPlatform.SetActive(false);
            FindFirstObjectByType<GameManager>()?.OnPlayerReturnedToShip();
        }

        public void RestartScenario()
        {
            scenarioManager.RestartScenario();
            currentStage = SceneState.Start;
            ShowStartStage();
            SetWindAudio(false);
            quizCanvas.SetActive(false);

            if (quizManager?.quizCanvasGroup != null)
            {
                quizManager.quizCanvasGroup.alpha = 0f;
                quizManager.quizCanvasGroup.interactable = false;
                quizManager.quizCanvasGroup.blocksRaycasts = false;
            }

            TeleportToPosition(startPosition, startRotation, false);
        }
    }

}