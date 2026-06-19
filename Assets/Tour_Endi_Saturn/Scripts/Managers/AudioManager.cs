using System.Collections;
using UnityEngine;
namespace Tour_Endi_Saturn
{

public enum AudioType
{
    Start,
    Atmosphere,
    Observation,
    Research,
    Quest,
    Return,
    SuccessSignal,
    QuizGood,
    QuizBad,
    QuizPerfect,
    End,
    ButtonPress,
    DataSend,
    ContinueButton
}

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource voiceSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("UI")]
    [SerializeField] private UIManager uiManager;

    [Header("Text")]
    [TextArea]
    [SerializeField] private string startText;
    [TextArea]
    [SerializeField] private string atmosphereFirstText;
    [TextArea]
    [SerializeField] private string atmosphereSecondText;
    [TextArea]
    [SerializeField] private string observationFirstText;
    [TextArea]
    [SerializeField] private string observationSecondText;
    [TextArea]
    [SerializeField] private string researchText;
    [TextArea]
    [SerializeField] private string questText;
    [TextArea]
    [SerializeField] private string returnText;
    [TextArea]
    [SerializeField] private string quizGoodText;
    [TextArea]
    [SerializeField] private string quizBadText;
    [TextArea]
    [SerializeField] private string quizPerfectText;
    [TextArea]
    [SerializeField] private string endText;

    [Header("Voice Clips")]
    [SerializeField] private AudioClip startClip;
    [SerializeField] private AudioClip atmosphereClip;
    [SerializeField] private AudioClip observationClip;
    [SerializeField] private AudioClip researchClip;
    [SerializeField] private AudioClip questClip;
    [SerializeField] private AudioClip returnClip;
    [SerializeField] private AudioClip quizGoodClip;
    [SerializeField] private AudioClip quizBadClip;
    [SerializeField] private AudioClip quizPerfectClip;
    [SerializeField] private AudioClip endClip;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip successSignalClip;
    [SerializeField] private AudioClip buttonPressClip;
    [SerializeField] private AudioClip dataSendClip;
    [SerializeField] private AudioClip continueButtonClip;

    public IEnumerator PlayAndWait(AudioType type)
    {
        AudioClip clip = GetClip(type);

        if (clip == null)
            yield break;

        voiceSource.Stop();

        StartCoroutine(ShowSubtitlesForClip(type, clip.length));

        voiceSource.clip = clip;
        voiceSource.Play();

        yield return new WaitForSeconds(clip.length);

        yield return new WaitForSeconds(1f);

        uiManager.HideTextPanel();
    }

    public void Play(AudioType type)
    {
        AudioClip clip = GetClip(type);

        if (clip == null)
            return;

        sfxSource.PlayOneShot(clip);
    }

    private AudioClip GetClip(AudioType type)
    {
        return type switch
        {
            AudioType.Start => startClip,
            AudioType.Atmosphere => atmosphereClip,
            AudioType.Observation => observationClip,
            AudioType.Research => researchClip,
            AudioType.Quest => questClip,
            AudioType.Return => returnClip,
            AudioType.QuizBad => quizBadClip,
            AudioType.QuizGood => quizGoodClip,
            AudioType.QuizPerfect => quizPerfectClip,
            AudioType.End => endClip,
            AudioType.SuccessSignal => successSignalClip,
            AudioType.ButtonPress => buttonPressClip,
            AudioType.DataSend => dataSendClip,
            AudioType.ContinueButton => continueButtonClip,
            _ => null
        };
    }

    private IEnumerator ShowSubtitlesForClip(AudioType type, float clipLength)
    {
        if (type == AudioType.Atmosphere)
        {
            uiManager.ShowSubtitles(atmosphereFirstText);

            yield return new WaitForSeconds(clipLength * 0.6f);

            uiManager.ShowSubtitles(atmosphereSecondText);
        }
        else if (type == AudioType.Observation)
        {
            uiManager.ShowSubtitles(observationFirstText);

            yield return new WaitForSeconds(clipLength / 2f);

            uiManager.ShowSubtitles(observationSecondText);
        }
        else
        {
            string subtitle = GetSubtitle(type);

            if (!string.IsNullOrEmpty(subtitle))
                uiManager.ShowSubtitles(subtitle);
        }
    }

    private string GetSubtitle(AudioType type)
    {
        return type switch
        {
            AudioType.Start => startText,

            AudioType.Research => researchText,

            AudioType.Quest => questText,

            AudioType.Return => returnText,

            AudioType.QuizBad => quizBadText,

            AudioType.QuizGood => quizGoodText,

            AudioType.QuizPerfect => quizPerfectText,

            AudioType.End => endText,

            _ => ""
        };
    }

    public void PlayFinalResult(AudioType type)
    {
        StartCoroutine(PlayAndWait(type));
    }
}
}
