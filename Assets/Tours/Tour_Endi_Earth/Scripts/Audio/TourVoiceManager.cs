using System.Collections;
using UnityEngine;

/// <summary>
/// Plays voice narration for Earth tour stages and elemental actions.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class TourVoiceManager : MonoBehaviour
{
    public static TourVoiceManager Instance { get; private set; }

    [Header("Tour")]
    [SerializeField] private EarthTourManager tourManager;

    [Header("Stage Voice Clips")]
    [SerializeField] private AudioClip shipIntroClip;
    [SerializeField] private AudioClip elementColumnsClip;
    [SerializeField] private AudioClip matchingQuestClip;
    [SerializeField] private AudioClip quizClip;
    [SerializeField] private AudioClip endClip;

    [Header("Element Voice Clips")]
    [SerializeField] private AudioClip airElementClip;
    [SerializeField] private AudioClip waterElementClip;
    [SerializeField] private AudioClip fireElementClip;
    [SerializeField] private AudioClip earthElementClip;

    [Header("Quiz Voice Clips")]
    [SerializeField] private AudioClip wrongAnswerClip;

    [Header("Quiz Result Voice Clips")]
    [SerializeField] private AudioClip quizResultLowClip;
    [SerializeField] private AudioClip quizResultGoodClip;
    [SerializeField] private AudioClip quizResultPerfectClip;
    [SerializeField] private AudioClip nextAdventureClip;

    [Header("Settings")]
    [SerializeField] private float firstVoiceDelay = 0.5f;
    [SerializeField] private bool stopPreviousVoice = true;
    [SerializeField] private bool showDebugLogs = true;

    private AudioSource audioSource;
    private EarthTourStage lastStage;
    private bool initialized;
    private Coroutine sequenceRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("TourVoiceManager: another instance already exists.");
        }

        Instance = this;

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
    }

    private void Start()
    {
        Invoke(nameof(InitializeVoice), firstVoiceDelay);
    }

    private void Update()
    {
        if (!initialized || tourManager == null)
            return;

        if (tourManager.CurrentStage == lastStage)
            return;

        lastStage = tourManager.CurrentStage;
        PlayStageVoice(lastStage);
    }

    private void InitializeVoice()
    {
        if (tourManager == null)
        {
            Debug.LogWarning("TourVoiceManager: TourManager is not assigned.");
            return;
        }

        initialized = true;
        lastStage = tourManager.CurrentStage;
        PlayStageVoice(lastStage);
    }

    private void PlayStageVoice(EarthTourStage stage)
    {
        AudioClip clip = GetClipForStage(stage);

        if (clip == null)
        {
            if (showDebugLogs)
                Debug.Log("TourVoiceManager: no stage voice for " + stage);

            return;
        }

        PlayClip(clip, "stage " + stage);
    }

    public void PlayElementVoice(ElementType elementType)
    {
        AudioClip clip = GetClipForElement(elementType);

        if (clip == null)
        {
            if (showDebugLogs)
                Debug.Log("TourVoiceManager: no element voice for " + elementType);

            return;
        }

        PlayClip(clip, "element " + elementType);
    }

    public void PlayWrongAnswerVoice()
    {
        if (wrongAnswerClip == null)
            return;

        PlayClip(wrongAnswerClip, "wrong answer");
    }

    public void PlayQuizResultVoice(int correctAnswers, int totalQuestions)
    {
        if (sequenceRoutine != null)
            StopCoroutine(sequenceRoutine);

        sequenceRoutine = StartCoroutine(PlayQuizResultRoutine(correctAnswers, totalQuestions));
    }

    public float GetQuizResultSequenceDuration(int correctAnswers, int totalQuestions)
    {
        AudioClip resultClip = GetQuizResultClip(correctAnswers, totalQuestions);

        float duration = 0f;

        if (resultClip != null)
            duration += resultClip.length;

        if (nextAdventureClip != null)
            duration += 1f + nextAdventureClip.length;

        return duration;
    }

    private IEnumerator PlayQuizResultRoutine(int correctAnswers, int totalQuestions)
    {
        AudioClip resultClip = GetQuizResultClip(correctAnswers, totalQuestions);

        if (resultClip != null)
        {
            PlayClip(resultClip, "quiz result");
            yield return new WaitForSeconds(resultClip.length + 1f);
        }

        if (nextAdventureClip != null)
        {
            PlayClip(nextAdventureClip, "next adventure");
            yield return new WaitForSeconds(nextAdventureClip.length);
        }
    }

    private AudioClip GetQuizResultClip(int correctAnswers, int totalQuestions)
    {
        if (correctAnswers <= 2)
            return quizResultLowClip;

        if (totalQuestions > 0 && correctAnswers >= totalQuestions)
            return quizResultPerfectClip;

        return quizResultGoodClip;
    }

    private void PlayClip(AudioClip clip, string reason)
    {
        if (clip == null || audioSource == null)
            return;

        if (stopPreviousVoice)
            audioSource.Stop();

        audioSource.clip = clip;
        audioSource.Play();

        if (showDebugLogs)
            Debug.Log("TourVoiceManager: playing voice for " + reason);
    }

    private AudioClip GetClipForStage(EarthTourStage stage)
    {
        switch (stage)
        {
            case EarthTourStage.ShipIntro:
                return shipIntroClip;

            case EarthTourStage.ElementColumns:
                return elementColumnsClip;

            case EarthTourStage.MatchingQuest:
                return matchingQuestClip;

            case EarthTourStage.Quiz:
                return quizClip;

            case EarthTourStage.End:
                return endClip;

            default:
                return null;
        }
    }

    private AudioClip GetClipForElement(ElementType elementType)
    {
        switch (elementType)
        {
            case ElementType.Air:
                return airElementClip;

            case ElementType.Water:
                return waterElementClip;

            case ElementType.Fire:
                return fireElementClip;

            case ElementType.Earth:
                return earthElementClip;

            default:
                return null;
        }
    }
}