using Unity.XR.CoreUtils;
using UnityEngine;
using System.Collections;

public class StageVisuals : MonoBehaviour
{
    [Header("Environment")]
    public GameObject uranusPlanet;
    public GameObject terrain;
    public GameObject skySphere;
    public GameObject spaceship;

    [Header("Effects")]
    public ParticleSystem windParticles;
    public ParticleSystem cloudParticles;

    [Header("Quest")]
    public GameObject drawingBoard;
    public GameObject markerCub;

    [Header("UI")]
    public GameObject sendButton;
    public GameObject exitButton;
    public GameObject returnPlatform;

    [Header("Quiz")]
    public GameObject quizCanvas;
    public QuizManager quizManager;

    [Header("Lighting")]
    public GameObject directionalLight;

    [Header("Audio")]
    public AudioSource voiceOver;
    public AudioClip startClip;
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

        SetupPlanet();

        sendButton.SetActive(false);
        exitButton.SetActive(false);
        returnPlatform.SetActive(false);

        HideAll();
        StartCoroutine(TeleportToStart());
        currentStage = SceneState.Start;
        ShowStartStage();
    }

    private IEnumerator TeleportToStart()
    {
        yield return null;
        TeleportToPosition(startPosition, startRotation);
    }

    void Update()
    {
        if (scenarioManager == null) return;

        SceneState newStage = scenarioManager.GetCurrentState();
        if (currentStage != newStage)
        {
            SceneState previousStage = currentStage;
            currentStage = newStage;
            UpdateStageVisuals(newStage);
        }

        if (isPlanetRotating)
        {
            uranusPlanet.transform.Rotate(Vector3.up, planetRotationSpeed * Time.deltaTime);
        }
    }

    private void UpdateStageVisuals(SceneState stage)
    {
        switch (stage)
        {
            case SceneState.Start: ShowStartStage(); break;
            case SceneState.Landing: ShowLandingStage(); break;
            case SceneState.Exploration: ShowExplorationStage(); break;
            case SceneState.Historical: ShowHistoricalStage(); break;
            case SceneState.Quest: ShowQuestStage(); break;
            case SceneState.Return: ShowReturnStage(); break;
            case SceneState.End: ShowEndStage(); break;
        }
    }

    private void SetupPlanet()
    {
        uranusPlanet.transform.localScale = new Vector3(100f, 100f, 100f);
        uranusPlanet.transform.rotation = Quaternion.Euler(0f, 0f, 25f);
    }

    private void HideAll()
    {
        uranusPlanet.SetActive(false);
        terrain.SetActive(false);
        skySphere.SetActive(false);
        spaceship.SetActive(false);
        drawingBoard.SetActive(false);
        markerCub.SetActive(false);
        directionalLight.SetActive(false);
        returnPlatform.SetActive(false);
        sendButton.SetActive(false);
        exitButton.SetActive(false);
        windParticles.Stop();
        cloudParticles.Stop();
        isPlanetRotating = false;
    }

    private void DisableLocomotion()
    {
        characterController.enabled = false;
        if (locomotionFolder != null) locomotionFolder.SetActive(false);
    }

    private void EnableLocomotion()
    {
        characterController.enabled = true;
        if (locomotionFolder != null) locomotionFolder.SetActive(true);
    }

    private void TeleportToPosition(Vector3 position, Quaternion rotation)
    {
        DisableLocomotion();

        Transform cameraOffset = xrOrigin.GetComponentInChildren<Camera>().transform.parent;
        float cameraYOffset = xrOrigin.CameraYOffset;

        xrOrigin.transform.position = position;
        xrOrigin.transform.rotation = rotation;
        cameraOffset.position = position;
        cameraOffset.rotation = rotation;

        cameraOffset.localPosition = new Vector3(0, 1.7f, 0);
        xrOrigin.CameraYOffset = 1.7f;

        EnableLocomotion();
    }

    private void ShowStartStage()
    {
        HideAll();
        DisableLocomotion();

        spaceship.SetActive(true);
        skySphere.SetActive(true);
        directionalLight.SetActive(true);
        uranusPlanet.SetActive(true);
        isPlanetRotating = true;

        PlayClip(startClip);
    }

    private void ShowLandingStage()
    {
        HideAll();
        TeleportToPosition(landingPosition, startRotation);
        EnableLocomotion();

        terrain.SetActive(true);
        skySphere.SetActive(true);
        directionalLight.SetActive(true);
        windParticles.Play();
        cloudParticles.Play();

        PlayClip(landingClip);
    }

    private void ShowExplorationStage()
    {
        HideAll();
        EnableLocomotion();

        terrain.SetActive(true);
        skySphere.SetActive(true);
        directionalLight.SetActive(true);
        windParticles.Play();

        PlayClip(explorationClip);
    }

    private void ShowHistoricalStage()
    {
        HideAll();
        EnableLocomotion();

        terrain.SetActive(true);
        skySphere.SetActive(true);
        directionalLight.SetActive(true);
        windParticles.Play();

        PlayClip(historicalClip);
    }

    private void ShowQuestStage()
    {
        HideAll();
        EnableLocomotion();

        terrain.SetActive(true);
        skySphere.SetActive(true);
        directionalLight.SetActive(true);
        drawingBoard.SetActive(true);
        markerCub.SetActive(true);
        sendButton.SetActive(true);

        PlayClip(questClip);
    }

    private void ShowReturnStage()
    {
        HideAll();
        EnableLocomotion();

        terrain.SetActive(true);
        skySphere.SetActive(true);
        directionalLight.SetActive(true);
        returnPlatform.SetActive(true);

        PlayClip(returnClip);
    }

    private void ShowEndStage()
    {
        HideAll();
        EnableLocomotion();

        spaceship.SetActive(true);
        skySphere.SetActive(true);
        directionalLight.SetActive(true);
        uranusPlanet.SetActive(true);
        isPlanetRotating = true;
        exitButton.SetActive(true);

        if (quizCanvas != null)
        {
            quizCanvas.SetActive(true);
        }
        if (quizManager != null)
        {
            quizManager.StartQuiz();
        }
    }

    public void TeleportPlayerToShip()
    {
        if (currentStage != SceneState.Return) return;

        TeleportToPosition(startPosition, startRotation);
        returnPlatform.SetActive(false);

        GameManager gm = FindFirstObjectByType<GameManager>();
        gm?.OnPlayerReturnedToShip();
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

        GameManager gm = FindFirstObjectByType<GameManager>();
        gm?.OnSendDrawingButtonPressed();
        sendButton.SetActive(false);
    }

    public void OnExitButtonPressed()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}