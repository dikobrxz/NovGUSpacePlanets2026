using System.Collections;
using UnityEngine;

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
            uiManager.ShowSubtitles(
                "Сатурн назван в честь римского бога земледелия. В основном он состоит из водорода с примесями гелия, а также следами воды, метана, аммиака и более тяжёлых элементов."
            );

            yield return new WaitForSeconds(clipLength * 0.6f);

            uiManager.ShowSubtitles(
                "По строению Сатурн, как и Юпитер, является газовым гигантом. При этом ветра на нём в несколько раз сильнее. Человек не смог бы прожить здесь и доли секунды."
            );
        }
        else if (type == AudioType.Observation)
        {
            uiManager.ShowSubtitles(
                "Сатурн отличается от других планет прежде всего своими кольцами. По одной из теорий, в далёком прошлом рядом с ним произошло крупное столкновение,"
            );

            yield return new WaitForSeconds(clipLength / 2f);

            uiManager.ShowSubtitles(
                "после которого и сформировалась система колец и множество спутников. Один из самых известных спутников Сатурна — Титан, который по размерам даже больше Меркурия."
            );
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
            AudioType.Start =>
                "Сегодня, дорогой исследователь, ты познакомишься с шестой планетой от Солнца и второй по величине после Юпитера.",

            AudioType.Research =>
                "Титан — один из самых интересных объектов Солнечной системы. В период с 2004 по 2017 год Сатурн и его спутники изучал аппарат Кассини.",

            AudioType.Quest =>
                "Давай возьмём передатчик и попробуем настроиться на частоту космического аппарата, чтобы передать данные с планеты — годы её изучения аппаратом „Кассини“",

            AudioType.Return =>
                "Благодаря тебе мы отправили эти данные учёным. Теперь мы можем вернуться на корабль.",

            AudioType.QuizBad =>
                "Спасибо, юный друг! Думаю, нам стоит ещё раз посетить Сатурн, чтобы узнать немного больше.",

            AudioType.QuizGood =>
                "Спасибо, наш юный исследователь! Твоих знаний уже достаточно, чтобы помочь учёным организовать новую миссию по исследованию Сатурна.",

            AudioType.QuizPerfect =>
                "Это невероятный успех нашего с тобой исследования Сатурна! Твои знания помогут человечеству узнать больше о нашей Солнечной системе.",

            AudioType.End =>
                "Ну что ж, отправимся в новое приключение.",

            _ => ""
        };
    }

    public void PlayFinalResult(AudioType type)
    {
        StartCoroutine(PlayAndWait(type));
    }
}
