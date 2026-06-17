using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plays voice narration for Earth tour stages and elemental actions.
/// Uses a queue so phrases do not cut each other off.
/// Supports a small hand menu for choosing: listen, skip, or speed up current narration.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class TourVoiceManager : MonoBehaviour
{
    public static TourVoiceManager Instance { get; private set; }

    private struct VoiceRequest
    {
        public AudioClip Clip;
        public string Reason;

        public VoiceRequest(AudioClip clip, string reason)
        {
            Clip = clip;
            Reason = reason;
        }
    }

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

    [Header("Queue Settings")]
    [SerializeField] private bool queueStageVoice = true;
    [SerializeField] private bool showChoiceMenuForElementVoice = true;
    [SerializeField] private VoiceChoiceMenu voiceChoiceMenu;

    [Header("Playback")]
    [SerializeField] private float normalPitch = 1f;
    [SerializeField] private float fastPitch = 1.5f;
    [SerializeField] private float pauseBetweenClips = 0.15f;

    [Header("Settings")]
    [SerializeField] private float firstVoiceDelay = 0.5f;
    [SerializeField] private bool showDebugLogs = true;

    private readonly Queue<VoiceRequest> voiceQueue = new Queue<VoiceRequest>();

    private AudioSource audioSource;
    private EarthTourStage lastStage;
    private bool initialized;
    private Coroutine playbackRoutine;
    private VoiceRequest? pendingChoiceRequest;

    private bool suppressNextStageVoice;
    private EarthTourStage suppressedStage;

    public bool IsVoicePlaying
    {
        get { return audioSource != null && audioSource.isPlaying; }
    }

    public bool HasQueuedVoice
    {
        get { return voiceQueue.Count > 0 || pendingChoiceRequest.HasValue; }
    }

    public bool IsBusy
    {
        get { return IsVoicePlaying || HasQueuedVoice || playbackRoutine != null; }
    }

    private bool IsNarrationCurrentlyActive
    {
        get { return IsVoicePlaying || voiceQueue.Count > 0 || playbackRoutine != null; }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Debug.LogWarning("TourVoiceManager: another instance already exists.");

        Instance = this;

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
        audioSource.pitch = normalPitch;
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

        if (suppressNextStageVoice && lastStage == suppressedStage)
        {
            suppressNextStageVoice = false;

            if (showDebugLogs)
                Debug.Log("TourVoiceManager: suppressed stage voice for " + lastStage);

            return;
        }

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

    public void SuppressNextStageVoice(EarthTourStage stage)
    {
        suppressNextStageVoice = true;
        suppressedStage = stage;
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

        if (queueStageVoice)
            EnqueueVoice(clip, "stage " + stage);
        else
            PlayVoiceImmediately(clip, "stage " + stage);
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

        VoiceRequest request = new VoiceRequest(clip, "element " + elementType);

        if (pendingChoiceRequest.HasValue)
            InsertPendingChoiceAsNext("another element voice was requested");

        if (IsNarrationCurrentlyActive && showChoiceMenuForElementVoice && voiceChoiceMenu != null)
        {
            pendingChoiceRequest = request;
            voiceChoiceMenu.Show("Диктор ещё говорит. Что сделать с новым рассказом?");
            return;
        }

        EnqueueRequest(request);
    }

    public void PlayWrongAnswerVoice()
    {
        if (wrongAnswerClip == null)
            return;

        EnqueueVoice(wrongAnswerClip, "wrong answer");
    }

    public void PlayQuizResultVoice(int correctAnswers, int totalQuestions)
    {
        AudioClip resultClip = GetQuizResultClip(correctAnswers, totalQuestions);

        if (resultClip != null)
            EnqueueVoice(resultClip, "quiz result");
    }

    public void PlayNextAdventureVoice()
    {
        if (nextAdventureClip == null)
        {
            if (showDebugLogs)
                Debug.LogWarning("TourVoiceManager: Next Adventure Clip is not assigned.");

            return;
        }

        EnqueueVoice(nextAdventureClip, "next adventure");
    }

    public float GetQuizResultSequenceDuration(int correctAnswers, int totalQuestions)
    {
        AudioClip resultClip = GetQuizResultClip(correctAnswers, totalQuestions);

        float duration = 0f;

        if (resultClip != null)
            duration += resultClip.length;

        return duration;
    }

    public float GetNextAdventureDuration()
    {
        if (nextAdventureClip == null)
            return 0f;

        return nextAdventureClip.length;
    }

    public void EnqueueVoice(AudioClip clip, string reason)
    {
        if (clip == null)
            return;

        EnqueueRequest(new VoiceRequest(clip, reason));
    }

    public void PlayVoiceImmediately(AudioClip clip, string reason)
    {
        if (clip == null)
            return;

        StopAllVoice();

        voiceQueue.Enqueue(new VoiceRequest(clip, reason));
        playbackRoutine = StartCoroutine(PlaybackLoop());
    }

    public IEnumerator PlaySequenceAndWait(AudioClip[] clips, string reasonPrefix, bool interruptCurrent)
    {
        if (clips == null || clips.Length == 0)
            yield break;

        if (interruptCurrent)
            StopAllVoice();

        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i] != null)
                voiceQueue.Enqueue(new VoiceRequest(clips[i], reasonPrefix + " " + (i + 1)));
        }

        if (playbackRoutine == null)
            playbackRoutine = StartCoroutine(PlaybackLoop());

        while (IsBusy)
            yield return null;
    }

    public IEnumerator WaitUntilIdle()
    {
        while (IsBusy)
            yield return null;
    }

    public IEnumerator WaitUntilNarratorFinished()
    {
        yield return WaitUntilIdle();
    }

    public void StopAllVoice()
    {
        voiceQueue.Clear();
        pendingChoiceRequest = null;

        if (voiceChoiceMenu != null)
            voiceChoiceMenu.Hide();

        if (playbackRoutine != null)
        {
            StopCoroutine(playbackRoutine);
            playbackRoutine = null;
        }

        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = null;
            audioSource.pitch = normalPitch;
        }

        if (showDebugLogs)
            Debug.Log("TourVoiceManager: all voice stopped.");
    }

    public void SkipCurrentVoice()
    {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }

    public void SpeedUpCurrentVoice()
    {
        if (audioSource == null || !audioSource.isPlaying)
            return;

        audioSource.pitch = fastPitch;

        if (showDebugLogs)
            Debug.Log("TourVoiceManager: current voice speed set to " + fastPitch + "x.");
    }

    public void ConfirmPendingVoiceQueue()
    {
        if (!pendingChoiceRequest.HasValue)
            return;

        InsertPendingChoiceAsNext("player chose to listen current voice first");
    }

    public void ConfirmPendingVoiceSkip()
    {
        if (!pendingChoiceRequest.HasValue)
            return;

        InsertPendingChoiceAsNext("player chose to skip current voice");

        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();

        if (playbackRoutine == null)
            playbackRoutine = StartCoroutine(PlaybackLoop());
    }

    public void ConfirmPendingVoiceSpeedUpThenQueue()
    {
        if (pendingChoiceRequest.HasValue)
            InsertPendingChoiceAsNext("player chose to speed up current voice");

        SpeedUpCurrentVoice();
    }

    public void ChooseListenCurrentThenPlayNew()
    {
        ConfirmPendingVoiceQueue();
    }

    public void ChooseSkipCurrentAndPlayNew()
    {
        ConfirmPendingVoiceSkip();
    }

    public void ChooseSpeedUpCurrentThenPlayNew()
    {
        ConfirmPendingVoiceSpeedUpThenQueue();
    }

    public void ConfirmPendingVoiceSpeedUp()
    {
        ConfirmPendingVoiceSpeedUpThenQueue();
    }

    private void EnqueueRequest(VoiceRequest request)
    {
        if (request.Clip == null)
            return;

        voiceQueue.Enqueue(request);

        if (playbackRoutine == null)
            playbackRoutine = StartCoroutine(PlaybackLoop());
    }

    private void InsertPendingChoiceAsNext(string reason)
    {
        if (!pendingChoiceRequest.HasValue)
            return;

        VoiceRequest request = pendingChoiceRequest.Value;
        pendingChoiceRequest = null;

        if (voiceChoiceMenu != null)
            voiceChoiceMenu.Hide();

        InsertRequestAsNext(request);

        if (playbackRoutine == null)
            playbackRoutine = StartCoroutine(PlaybackLoop());

        if (showDebugLogs)
            Debug.Log("TourVoiceManager: pending voice inserted as next because " + reason);
    }

    private void InsertRequestAsNext(VoiceRequest request)
    {
        if (request.Clip == null)
            return;

        Queue<VoiceRequest> reorderedQueue = new Queue<VoiceRequest>();

        reorderedQueue.Enqueue(request);

        while (voiceQueue.Count > 0)
            reorderedQueue.Enqueue(voiceQueue.Dequeue());

        while (reorderedQueue.Count > 0)
            voiceQueue.Enqueue(reorderedQueue.Dequeue());
    }

    private IEnumerator PlaybackLoop()
    {
        while (voiceQueue.Count > 0 || pendingChoiceRequest.HasValue)
        {
            if (voiceQueue.Count == 0 && pendingChoiceRequest.HasValue)
                InsertPendingChoiceAsNext("current voice ended and player did not choose menu option");

            if (voiceQueue.Count == 0)
                break;

            VoiceRequest request = voiceQueue.Dequeue();

            if (request.Clip == null)
                continue;

            audioSource.pitch = normalPitch;
            audioSource.clip = request.Clip;
            audioSource.Play();

            if (showDebugLogs)
                Debug.Log("TourVoiceManager: playing voice for " + request.Reason);

            while (audioSource != null && audioSource.isPlaying)
                yield return null;

            if (audioSource != null)
                audioSource.pitch = normalPitch;

            if (pendingChoiceRequest.HasValue)
                InsertPendingChoiceAsNext("current voice ended and player did not choose menu option");

            yield return new WaitForSeconds(pauseBetweenClips);
        }

        playbackRoutine = null;
    }

    private AudioClip GetQuizResultClip(int correctAnswers, int totalQuestions)
    {
        if (correctAnswers <= 2)
            return quizResultLowClip;

        if (totalQuestions > 0 && correctAnswers >= totalQuestions)
            return quizResultPerfectClip;

        return quizResultGoodClip;
    }

    private AudioClip GetClipForStage(EarthTourStage stage)
    {
        switch (stage)
        {
            case EarthTourStage.ShipIntro:
                return shipIntroClip;

            case EarthTourStage.PlanetIntro:
                return null;

            case EarthTourStage.SurfaceIntro:
                return null;

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