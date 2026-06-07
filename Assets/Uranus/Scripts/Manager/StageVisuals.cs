using Unity.XR.CoreUtils;
using UnityEngine;
using System.Collections;

public class StageVisuals : MonoBehaviour
{
    [Header("Environment")]
    public GameObject uranusPlanet;
    public GameObject terrain;
    public GameObject spaceship;

    [Header("Effects")]
    public ParticleSystem windParticles;

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

    private ScenarioManager scenarioManager;
    private XROrigin xrOrigin;
    private CharacterController characterController;
    private SceneState currentStage;
    private GameObject locomotionFolder;
    private float planetRotationSpeed = 5f;
    private bool isPlanetRotating = false;

    private readonly Vector3 startPosition = new Vector3(-4f, 100f, -15f);
    private readonly Vector3 landingPosition = new Vector3(83f, 74f, -90f);
    private readonly Quaternion startRotation = Quaternion.identity;

    void Start()
    {
        scenarioManager = GetComponent<ScenarioManager>();
        xrOrigin = FindFirstObjectByType<XROrigin>();
        characterController = xrOrigin.GetComponent<CharacterController>();

        Transform locoTransform = xrOrigin.transform.Find("Locomotion");
        if (locoTransform != null) locomotionFolder = locoTransform.gameObject;

        uranusPlanet.transform.localScale = new Vector3(100f, 100f, 100f);
        uranusPlanet.transform.rotation = Quaternion.Euler(0f, 0f, 25f);

        sendButton.SetActive(false);
        returnPlatform.SetActive(false);
        volCloudsObject.SetActive(false);

        if (actionButton != null)
        {
            var btn = actionButton.GetComponent<UnityEngine.UI.Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(OnActionButtonPressed);
            }
            SetButtonText("Начать");
        }

        HideAll();
        StartCoroutine(TeleportToStart());
        currentStage = SceneState.Start;
        ShowStartStage();
    }

    private void SetButtonText(string text)
    {
        if (actionButton != null)
        {
            var tmpText = actionButton.GetComponentInChildren<TMPro.TMP_Text>();
            if (tmpText != null) tmpText.text = text;
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
            switch (newStage)
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

        if (isPlanetRotating) uranusPlanet.transform.Rotate(Vector3.up, planetRotationSpeed * Time.deltaTime);
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

    private void TeleportToPosition(Vector3 position, Quaternion rotation, bool enableLocomotionAfter = true)
    {
        SetLocomotion(false);

        var cameraOffset = xrOrigin.GetComponentInChildren<Camera>().transform.parent;
        float cameraYOffset = xrOrigin.CameraYOffset;

        xrOrigin.transform.SetPositionAndRotation(position, rotation);
        cameraOffset.SetPositionAndRotation(position, rotation);
        cameraOffset.localPosition = new Vector3(0, 1.7f, 0);
        xrOrigin.CameraYOffset = 1.7f;

        if (enableLocomotionAfter) SetLocomotion(true);
    }

    private void ShowStartStage()
    {
        HideAll();
        SetLocomotion(false);
        spaceship.SetActive(true);
        directionalLight.SetActive(true);
        uranusPlanet.SetActive(true);
        isPlanetRotating = true;

        if (actionButton != null)
        {
            SetButtonText("Начать");
            actionButton.SetActive(true);
            var parentCanvas = actionButton.GetComponentInParent<Canvas>();
            if (parentCanvas != null) parentCanvas.gameObject.SetActive(true);
            var parentGroup = actionButton.GetComponentInParent<CanvasGroup>();
            if (parentGroup != null)
            {
                parentGroup.alpha = 1f;
                parentGroup.interactable = true;
                parentGroup.blocksRaycasts = true;
            }
        }
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
        terrain.SetActive(true);
        directionalLight.SetActive(true);
        windParticles.Play();
        volCloudsObject.SetActive(true);
        PlayClip(landingClip);
    }

    private void ShowExplorationStage()
    {
        HideAll();
        SetLocomotion(true);
        terrain.SetActive(true);
        directionalLight.SetActive(true);
        windParticles.Play();
        volCloudsObject.SetActive(true);
        PlayClip(explorationClip);
    }

    private void ShowHistoricalStage()
    {
        HideAll();
        SetLocomotion(true);
        terrain.SetActive(true);
        directionalLight.SetActive(true);
        windParticles.Play();
        volCloudsObject.SetActive(true);
        PlayClip(historicalClip);
    }

    private void ShowQuestStage()
    {
        HideAll();
        SetLocomotion(true);
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
        SetLocomotion(true);
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
        volCloudsObject.SetActive(true);
        actionButton.SetActive(false);
        quizCanvas.SetActive(true);
        quizManager?.StartQuiz();
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip == null) return;
        voiceOver.Stop();
        voiceOver.clip = clip;
        voiceOver.Play();
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
        SetButtonText("Начать");
        currentStage = SceneState.Start;
        ShowStartStage();
        quizCanvas.SetActive(false);
        if (quizManager?.quizCanvasGroup != null)
        {
            quizManager.quizCanvasGroup.alpha = 0f;
            quizManager.quizCanvasGroup.interactable = false;
            quizManager.quizCanvasGroup.blocksRaycasts = false;
        }
        TeleportToPosition(startPosition, startRotation, false);
    }

    public void EndScenario()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}