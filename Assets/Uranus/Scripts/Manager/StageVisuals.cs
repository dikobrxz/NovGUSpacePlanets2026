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
    private Transform returnPoint;
    private SceneState currentStage;
    private GameObject locomotionFolder;

    private float planetRotationSpeed = 5f;
    private bool isPlanetRotating = false;

    void Start()
    {
        scenarioManager = GetComponent<ScenarioManager>();
        xrOrigin = FindFirstObjectByType<XROrigin>();
        characterController = xrOrigin.GetComponent<CharacterController>();

        Transform locoTransform = xrOrigin.transform.Find("Locomotion");
        if (locoTransform != null) locomotionFolder = locoTransform.gameObject;

        SetupPlanet();
        SetupReturnPoint();

        sendButton.SetActive(false);
        exitButton.SetActive(false);
        returnPlatform.SetActive(false);

        HideAll();
        StartCoroutine(TeleportToShipAtStart());
        currentStage = SceneState.Start;
        ShowStartStage();
    }

    private IEnumerator TeleportToShipAtStart()
    {
        yield return null;
        TeleportToShip();
    }

    void Update()
    {
        if (scenarioManager == null) return;

        SceneState newStage = scenarioManager.GetCurrentState();
        if (currentStage != newStage)
        {
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

    private void SetupReturnPoint()
    {
        returnPoint = spaceship.transform.Find("PlayerReturnPoint");
        if (returnPoint == null)
        {
            GameObject point = new GameObject("PlayerReturnPoint");
            point.transform.SetParent(spaceship.transform);
            point.transform.localPosition = new Vector3(0, 1.5f, 0);
            returnPoint = point.transform;
        }
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

    private void ShowPlanet(Vector3 position)
    {
        uranusPlanet.SetActive(true);
        uranusPlanet.transform.position = position;
    }

    private void ShowStartStage()
    {
        HideAll();
        DisableLocomotion();

        spaceship.SetActive(true);
        skySphere.SetActive(true);
        directionalLight.SetActive(true);
        ShowPlanet(new Vector3(0, 130f, 170f));
        isPlanetRotating = true;

        PlayClip(startClip);
    }

    private void ShowLandingStage()
    {
        HideAll();
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
        ShowPlanet(new Vector3(0, 130f, 170f));
        isPlanetRotating = true;
        exitButton.SetActive(true);
    }

    private void TeleportToShip()
    {
        DisableLocomotion();
        xrOrigin.transform.position = returnPoint.position;
        xrOrigin.transform.rotation = returnPoint.rotation;
        EnableLocomotion();
    }

    public void TeleportPlayerToShip()
    {
        if (currentStage != SceneState.Return) return;

        TeleportToShip();
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