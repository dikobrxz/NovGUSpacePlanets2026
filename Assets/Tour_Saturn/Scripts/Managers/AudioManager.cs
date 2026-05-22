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
    End,
    SuccessSignal
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
    [SerializeField] private AudioClip endClip;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip successSignalClip;

    public IEnumerator PlayAndWait(AudioType type)
    {
        AudioClip clip = GetClip(type);
        string subtitle = GetSubtitle(type);

        if (clip == null)
            yield break;

        voiceSource.Stop();

        if (!string.IsNullOrEmpty(subtitle))
            uiManager.ShowSubtitles(subtitle);

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
            AudioType.End => endClip,
            AudioType.SuccessSignal => successSignalClip,
            _ => null
        };
    }

    private string GetSubtitle(AudioType type)
    {
        return type switch
        {
            AudioType.Start =>
                "Сегодня, дорогой исследователь, ты познакомишься с шестой планетой от Солнца и второй по величине после Юпитера.",

            AudioType.Atmosphere =>
                "Сатурн назван в честь римского бога земледелия. В основном он состоит из водорода с примесями гелия. По строению Сатурн является газовым гигантом. Человек не смог бы прожить здесь и доли секунды.",

            AudioType.Observation =>
                "Сатурн отличается от других планет прежде всего своими кольцами. Один из самых известных спутников Сатурна — Титан, который по размерам даже больше Меркурия.",

            AudioType.Research =>
                "Титан — один из самых интересных объектов Солнечной системы. В период с 2004 по 2017 год Сатурн и его спутники изучал аппарат Кассини.",

            AudioType.Quest =>
                "Давай возьмём передатчик и попробуем настроиться на частоту космического аппарата. Первый рычажок поверни до значения 2004, второй — до 2017.",

            AudioType.Return =>
                "Благодаря тебе мы отправили эти данные учёным. Теперь мы можем вернуться на корабль.",

            AudioType.End =>
                "Ну что ж, отправимся в новое приключение.",

            _ => ""
        };
    }
}
