using UnityEngine;
using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine.Events;
using System.Collections.Generic;

public class StoryManager : MonoBehaviour
{
    private FogController fogController;
    private FadeScreen fadeScreen;
    private Transform playerTransform;
    private bool missionDone = false;

    [Header("Спавн-точки")]
    public Transform spawnShipStart;
    public Transform spawnShipNeptune;
    public Transform spawnNeptuneSurface;
    public Transform spawnShipQuiz;

    [Header("Группы объектов")]
    public GameObject shipObject;
    public GameObject startUI;
    public GameObject startDecoration;  // final_scene (1) — декоративный Нептун
    public GameObject neptuneAll;
    public GameObject compassObject;
    public GameObject quizUI;
    public NeptuneAtmosphere atmosphere;
    public CompassGuideArrow guideArrow;

    [Header("Музыка и озвучка")]
    public AudioSource startMusic;
    public AudioSource narrator;
    public AudioClip clip1;
    public AudioClip clip2;
    public AudioClip clip3;
    public AudioClip clip4;
    public AudioClip clip5;

    [Space, Header("Other")]
    [SerializeField] private XROrigin _xrOrigin;
    [SerializeField] private GameObject _playerMove;
    [SerializeField] private Transform _shipPlayerPoint;
    [SerializeField] private Transform _shipObservePlayerPoint;
    [SerializeField] private Transform _surfacePlayerPoint;
    [SerializeField] private ParticleSystem[] _snowParticles;
    [SerializeField] private AudioSource[] _snowSounds;
    [SerializeField] private GameObject _hint;

    [Space]
    [SerializeField] private UnityEvent _onStart;

    public enum GameState { Start, ShipToNeptune, Surface, Quiz, End }
    private GameState currentState;

    void Start()
    {
        fogController = GetComponent<FogController>();
        fadeScreen = FindFirstObjectByType<FadeScreen>();
        playerTransform = FindFirstObjectByType<Unity.XR.CoreUtils.XROrigin>().transform;
        currentState = GameState.Start;

        SetActiveIfNotNull(shipObject, true);
        SetActiveIfNotNull(startUI, true);
        SetActiveIfNotNull(startDecoration, true);
        SetActiveIfNotNull(neptuneAll, false);
        SetActiveIfNotNull(compassObject, false);
        SetActiveIfNotNull(quizUI, false);
    }

    public void OnStartPressed()
    {
        SetActiveIfNotNull(startUI, false);
        SetActiveIfNotNull(quizUI, false);
        if (startMusic != null) startMusic.Stop();
        StartCoroutine(RunStory());

        _onStart?.Invoke();
    }

    IEnumerator RunStory()
    {
        PlayClip(clip1);
        yield return WaitForClip(clip1);

        currentState = GameState.ShipToNeptune;
        yield return Fade(true);

        // Прячем декоративную планету при переходе
        SetActiveIfNotNull(startDecoration, false);
        SetActiveIfNotNull(neptuneAll, true);
        TeleportPlayer(_shipObservePlayerPoint, false);
        yield return Fade(false);

        PlayClip(clip2);
        yield return WaitForClip(clip2);

        currentState = GameState.Surface;
        yield return Fade(true);
        SetActiveIfNotNull(shipObject, false);
        TeleportPlayer(_surfacePlayerPoint, true);
        yield return Fade(false);

        yield return new WaitForSeconds(2f);
        PlayClip(clip3);
        yield return WaitForClip(clip3);

        yield return new WaitForSeconds(2f);

        SetActiveIfNotNull(compassObject, true);
        _hint.SetActive(true);

        PlayClip(clip4);
        yield return WaitForClip(clip4);

        CoordinateDevice device = FindFirstObjectByType<CoordinateDevice>();
        if (device != null)
            device.EnableInteraction();

        yield return new WaitUntil(() => missionDone);
        DisableSnow();
        yield return new WaitForSeconds(1f);
        PlayClip(clip5);

        yield return new WaitForSeconds(10f);
        /*
        if (atmosphere != null)
            StartCoroutine(atmosphere.Dissipate(5f));
        if (fogController != null)
            StartCoroutine(fogController.DissolveFog());
        */
        

        if (clip5 != null && clip5.length > 10f)
            yield return new WaitForSeconds(clip5.length - 10f);

        yield return new WaitForSeconds(2f);

        currentState = GameState.Quiz;
        yield return Fade(true);
        SetActiveIfNotNull(startDecoration, true);
        SetActiveIfNotNull(neptuneAll, false);
        SetActiveIfNotNull(compassObject, false);
        SetActiveIfNotNull(shipObject, true);
        SetActiveIfNotNull(quizUI, true);
        TeleportPlayer(_shipPlayerPoint, false);
        yield return Fade(false);

        QuizManager qm = FindFirstObjectByType<QuizManager>();
        if (qm != null) qm.StartQuiz();
    }

    public void OnMissionComplete()
    {
        missionDone = true;
    }

    void PlayClip(AudioClip clip)
    {
        if (narrator != null && clip != null)
        {
            narrator.clip = clip;
            narrator.Play();
        }
    }

    IEnumerator WaitForClip(AudioClip clip)
    {
        if (clip != null)
            yield return new WaitForSeconds(clip.length);
        else
            yield return new WaitForSeconds(3f);
    }

    IEnumerator Fade(bool toBlack)
    {
        if (fadeScreen == null) yield break;
        if (toBlack)
            yield return StartCoroutine(fadeScreen.FadeOut());
        else
            yield return StartCoroutine(fadeScreen.FadeIn());
    }

    void TeleportPlayer(Transform point, bool isMove)
    {
        StartCoroutine(TeleportPlayerCoroutine(point, isMove));
    }

    private IEnumerator TeleportPlayerCoroutine(Transform point, bool isMove)
    {
        yield return null;

        _xrOrigin.MoveCameraToWorldLocation(point.transform.position);
        _xrOrigin.MatchOriginUpCameraForward(Vector3.up, point.transform.forward.normalized);

        yield return null;

        _playerMove.SetActive(isMove);
    }

    void SetActiveIfNotNull(GameObject obj, bool active)
    {
        if (obj != null) obj.SetActive(active);
    }

    public GameState GetCurrentStage() { return currentState; }

    private void DisableSnow()
    {
        foreach (var particle in _snowParticles)
        {
            particle.emissionRate = 0;
        }

        StartCoroutine(FadeSnowAudio());
    }

    private IEnumerator FadeSnowAudio()
    {
        float elapsed = 0f;

        List<float> startVolumes = new List<float>();
        foreach (var source in _snowSounds)
        {
            startVolumes.Add(source.volume);
        }

        while (elapsed < 3)
        {
            elapsed += Time.deltaTime;
            float delta = elapsed / 3;

            for (int i = 0; i < _snowSounds.Length; i++)
            {
                if (_snowSounds[i] != null)
                {
                    _snowSounds[i].volume = Mathf.Lerp(startVolumes[i], 0, delta);
                }
            }

            yield return null;
        }

        for (int i = 0; i < _snowSounds.Length; i++)
        {
            if (_snowSounds[i] != null)
            {
                _snowSounds[i].volume = 0;
                _snowSounds[i].Stop();
            }
        }
    }

    public void ResetAudioSnow()
    {
        foreach (var source in _snowSounds)
        {
            source.volume = .25f;
            source.Play();
        }
    }
}