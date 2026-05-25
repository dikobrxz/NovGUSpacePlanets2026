using Unity.XR.CoreUtils;
using UnityEngine;

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

    [Header("Lighting")]
    public GameObject directionalLight;

    [Header("Audio")]
    public AudioSource voiceOver;
    public AudioClip startClip;
    public AudioClip orbitClip;
    public AudioClip landingClip;
    public AudioClip explorationClip;
    public AudioClip questClip;
    public AudioClip returnClip;

    private ScenarioManager scenarioManager;
    private XROrigin xrOrigin;
    private CharacterController characterController;
    private Transform returnPoint;
    private SceneState currentStage;
    private GameObject locomotionFolder;

    void Start()
    {
        scenarioManager = GetComponent<ScenarioManager>();
        xrOrigin = FindFirstObjectByType<XROrigin>();

        if (xrOrigin != null)
        {
            characterController = xrOrigin.GetComponent<CharacterController>();
            Transform locoTransform = xrOrigin.transform.Find("Locomotion");
            if (locoTransform != null) locomotionFolder = locoTransform.gameObject;
        }

        SetupPlanet();
        SetupReturnPoint();

        if (sendButton != null) sendButton.SetActive(false);

        HideAll();
        currentStage = SceneState.Start;
        ShowStartStage();
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
    }

    private void UpdateStageVisuals(SceneState stage)
    {
        switch (stage)
        {
            case SceneState.Start: ShowStartStage(); break;
            case SceneState.Orbit: ShowOrbitStage(); break;
            case SceneState.Landing: ShowLandingStage(); break;
            case SceneState.Exploration: ShowExplorationStage(); break;
            case SceneState.Quest: ShowQuestStage(); break;
            case SceneState.Return: ShowReturnStage(); break;
        }
    }

    private void SetupPlanet()
    {
        if (uranusPlanet != null)
        {
            uranusPlanet.transform.localScale = new Vector3(100f, 100f, 100f);
        }
    }

    private void SetupReturnPoint()
    {
        if (spaceship == null) return;

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
        if (uranusPlanet != null) uranusPlanet.SetActive(false);
        if (terrain != null) terrain.SetActive(false);
        if (skySphere != null) skySphere.SetActive(false);
        if (spaceship != null) spaceship.SetActive(false);
        if (drawingBoard != null) drawingBoard.SetActive(false);
        if (markerCub != null) markerCub.SetActive(false);
        if (directionalLight != null) directionalLight.SetActive(false);
        if (windParticles != null) windParticles.Stop();
        if (cloudParticles != null) cloudParticles.Stop();
        if (sendButton != null) sendButton.SetActive(false);
    }

    private void SetLocomotion(bool enabled)
    {
        if (characterController != null) characterController.enabled = enabled;
        if (locomotionFolder != null) locomotionFolder.SetActive(enabled);
        Debug.Log($"Локомоция: {(enabled ? "ВКЛ" : "ВЫКЛ")}");
    }

    private void ShowPlanet(Vector3 position)
    {
        if (uranusPlanet != null)
        {
            uranusPlanet.SetActive(true);
            uranusPlanet.transform.position = position;
        }
    }

    private void ShowStartStage()
    {
        Debug.Log("=== СТАРТОВЫЙ ЭТАП ===");
        HideAll();
        SetLocomotion(false);

        ShowPlanet(new Vector3(0, 130f, 170f));

        if (skySphere != null) skySphere.SetActive(true);
        if (directionalLight != null) directionalLight.SetActive(true);

        PlayClip(startClip);
    }

    private void ShowOrbitStage()
    {
        Debug.Log("=== ОРБИТА ===");
        HideAll();
        SetLocomotion(false);

        ShowPlanet(new Vector3(0, 125f, 170f));

        if (skySphere != null) skySphere.SetActive(true);
        if (directionalLight != null) directionalLight.SetActive(true);

        PlayClip(orbitClip);
    }

    private void ShowLandingStage()
    {
        Debug.Log("=== ВЫСАДКА ===");
        HideAll();
        SetLocomotion(true);

        if (terrain != null) terrain.SetActive(true);
        if (skySphere != null) skySphere.SetActive(true);
        if (directionalLight != null) directionalLight.SetActive(true);
        if (windParticles != null) windParticles.Play();
        if (cloudParticles != null) cloudParticles.Play();

        PlayClip(landingClip);
    }

    private void ShowExplorationStage()
    {
        Debug.Log("=== ИССЛЕДОВАНИЕ ===");
        HideAll();
        SetLocomotion(true);

        if (terrain != null) terrain.SetActive(true);
        if (skySphere != null) skySphere.SetActive(true);
        if (directionalLight != null) directionalLight.SetActive(true);
        if (windParticles != null) windParticles.Play();

        PlayClip(explorationClip);
    }

    private void ShowQuestStage()
    {
        Debug.Log("=== КВЕСТ ===");
        HideAll();
        SetLocomotion(true);

        if (terrain != null) terrain.SetActive(true);
        if (skySphere != null) skySphere.SetActive(true);
        if (directionalLight != null) directionalLight.SetActive(true);
        if (drawingBoard != null) drawingBoard.SetActive(true);
        if (markerCub != null) markerCub.SetActive(true);
        if (sendButton != null) sendButton.SetActive(true);

        PlayClip(questClip);
    }

    private void ShowReturnStage()
    {
        Debug.Log("=== ВОЗВРАЩЕНИЕ ===");
        HideAll();
        SetLocomotion(true);

        if (spaceship != null) spaceship.SetActive(true);
        if (skySphere != null) skySphere.SetActive(true);
        if (directionalLight != null) directionalLight.SetActive(true);

        PlayClip(returnClip);
        TeleportToShip();
    }

    private void TeleportToShip()
    {
        if (xrOrigin == null || returnPoint == null)
        {
            Debug.LogError("Телепорт не удался!");
            return;
        }

        // Сохраняем состояние CharacterController
        bool wasEnabled = characterController != null && characterController.enabled;
        if (characterController != null) characterController.enabled = false;

        xrOrigin.transform.position = returnPoint.position;
        xrOrigin.transform.rotation = returnPoint.rotation;

        if (characterController != null) characterController.enabled = wasEnabled;

        Debug.Log($"Телепорт на корабль: {returnPoint.position}");
    }

    private void PlayClip(AudioClip clip)
    {
        if (voiceOver != null && clip != null)
        {
            voiceOver.Stop();
            voiceOver.clip = clip;
            voiceOver.Play();
        }
    }

    public void OnSendButtonPressed()
    {
        Debug.Log("Кнопка отправки нажата!");
        if (scenarioManager != null && scenarioManager.GetCurrentState() == SceneState.Quest)
        {
            GameManager gm = FindFirstObjectByType<GameManager>();
            gm?.OnSendDrawingButtonPressed();
            if (sendButton != null) sendButton.SetActive(false);
        }
    }
}