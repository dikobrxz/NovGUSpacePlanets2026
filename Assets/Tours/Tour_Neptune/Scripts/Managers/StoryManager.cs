using UnityEngine;
using System.Collections;

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
        if (startMusic != null) startMusic.Stop();
        StartCoroutine(RunStory());
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
        TeleportPlayer(spawnShipNeptune);
        yield return Fade(false);

        PlayClip(clip2);
        yield return WaitForClip(clip2);

        currentState = GameState.Surface;
        yield return Fade(true);
        SetActiveIfNotNull(shipObject, false);
        TeleportPlayer(spawnNeptuneSurface);
        yield return Fade(false);

        yield return new WaitForSeconds(2f);
        PlayClip(clip3);
        yield return WaitForClip(clip3);

        yield return new WaitForSeconds(2f);

        SetActiveIfNotNull(compassObject, true);
        if (guideArrow != null)
            guideArrow.Show();

        PlayClip(clip4);
        yield return WaitForClip(clip4);

        CoordinateDevice device = FindFirstObjectByType<CoordinateDevice>();
        if (device != null)
            device.EnableInteraction();

        yield return new WaitUntil(() => missionDone);

        yield return new WaitForSeconds(1f);
        PlayClip(clip5);

        yield return new WaitForSeconds(10f);
        if (atmosphere != null)
            StartCoroutine(atmosphere.Dissipate(5f));
        if (fogController != null)
            StartCoroutine(fogController.DissolveFog());

        if (clip5 != null && clip5.length > 10f)
            yield return new WaitForSeconds(clip5.length - 10f);

        yield return new WaitForSeconds(2f);

        currentState = GameState.Quiz;
        yield return Fade(true);
        SetActiveIfNotNull(neptuneAll, false);
        SetActiveIfNotNull(compassObject, false);
        SetActiveIfNotNull(shipObject, true);
        SetActiveIfNotNull(quizUI, true);
        TeleportPlayer(spawnShipQuiz);
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

    void TeleportPlayer(Transform point)
    {
        if (point != null && playerTransform != null)
        {
            playerTransform.position = point.position;
            playerTransform.rotation = point.rotation;
        }
    }

    void SetActiveIfNotNull(GameObject obj, bool active)
    {
        if (obj != null) obj.SetActive(active);
    }

    public GameState GetCurrentStage() { return currentState; }
}