using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tour_Endi_Moon
{
    /// <summary>
    /// Менеджер озвучки. Проигрывает голос за кадром на каждом этапе сценария.
    /// Реплики ставятся в очередь и никогда не перебивают друг друга.
    ///
    /// Маршрут озвучки (порядок задаётся порядком этапов в enum GameState):
    ///   Intro     — clipIntro      (Spawn 1, внутри корабля)
    ///   Landing   — clipLanding    (первый монолог о Луне)
    ///   Monologue — clipMonologue  (Spawn 2, второй монолог)
    ///   Exploration — clipExplorationIntro
    ///   Collecting  — clipCollecting
    ///   End         — clipEnd* + clipNextAdventure
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        [Header("AudioSource для голоса за кадром")]
        [SerializeField] private AudioSource narratorSource;

        [Header("Intro — Spawn 1, внутри корабля")]
        [Tooltip("«Сегодня, дорогой исследователь, ты познакомишься с небесным телом — Луной.»")]
        [SerializeField] private AudioClip clipIntro;

        [Header("Landing — первый монолог о Луне")]
        [Tooltip("«Луна — самый близкий к Земле объект...»")]
        [SerializeField] private AudioClip clipLanding;

        [Header("Monologue — Spawn 2, второй монолог")]
        [Tooltip("Второй монолог после телепорта на Spawn 2")]
        [SerializeField] private AudioClip clipMonologue;

        [Header("Exploration — начало раскопок (Spawn 3)")]
        [Tooltip("«Давай исследуем поверхность Луны и найдём доказательства пребывания здесь человека»")]
        [SerializeField] private AudioClip clipExplorationIntro;

        [Header("Артефакты — реплики при находке")]
        [Tooltip("При находке ботинка")]
        [SerializeField] private AudioClip clipBootFound;

        [Tooltip("При находке детали лунохода")]
        [SerializeField] private AudioClip clipMetalFound;

        [Tooltip("При находке блокнота")]
        [SerializeField] private AudioClip clipNotebookFound;

        [Header("Collecting — все найдены, складываем в ящик")]
        [Tooltip("«Давай заберём всё это с собой и передадим учёным для исследования.»")]
        [SerializeField] private AudioClip clipCollecting;

        [Header("End — финальные реплики по результату квиза")]
        [Tooltip("Финальная реплика — отличный результат (все 5 верных)")]
        [SerializeField] private AudioClip clipEndPerfect;

        [Tooltip("Финальная реплика — хороший результат (3-4 верных)")]
        [SerializeField] private AudioClip clipEndGood;

        [Tooltip("Финальная реплика — плохой результат (0-2 верных)")]
        [SerializeField] private AudioClip clipEndBad;

        [Tooltip("«Ну что ж, отправимся в новое приключение.»")]
        [SerializeField] private AudioClip clipNextAdventure;

        // Очередь клипов — реплики никогда не перебивают друг друга
        private readonly Queue<AudioClip> clipQueue = new();
        private Coroutine playbackCoroutine;

        private StoryManager story;

        private void Start()
        {
            if (narratorSource == null)
            {
                narratorSource = gameObject.AddComponent<AudioSource>();
                narratorSource.playOnAwake = false;
                narratorSource.spatialBlend = 0f; // 2D — голос за кадром
            }

            story = GameManager.Instance != null ? GameManager.Instance.Story : FindFirstObjectByType<StoryManager>();
            if (story != null)
                story.OnStateChanged += HandleStateChanged;
            else
                Debug.LogWarning("[AudioManager] StoryManager не найден.");
        }

        private void OnDestroy()
        {
            if (story != null)
                story.OnStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.Intro:
                    Enqueue(clipIntro);
                    break;

                case GameState.Landing:
                    Enqueue(clipLanding);
                    break;

                case GameState.Monologue:
                    Enqueue(clipMonologue);
                    break;

                case GameState.Exploration:
                    Enqueue(clipExplorationIntro);
                    break;

                case GameState.Collecting:
                    Enqueue(clipCollecting);
                    break;

                case GameState.End:
                    PlayEnd();
                    break;
            }
        }

        /// <summary>Проигрывает звук по типу найденного артефакта (ставит в очередь).</summary>
        public void PlayArtifactFoundClip(ArtifactType type)
        {
            switch (type)
            {
                case ArtifactType.Boot: Enqueue(clipBootFound); break;
                case ArtifactType.Metal: Enqueue(clipMetalFound); break;
                case ArtifactType.Notebook: Enqueue(clipNotebookFound); break;
            }
        }

        /// <summary>Длина вступительного клипа Intro. Используется IntroSequencer.</summary>
        public float GetIntroClipDuration() => clipIntro != null ? clipIntro.length : 30f;

        /// <summary>Длина клипа Landing (первый монолог о Луне). Используется IntroSequencer.</summary>
        public float GetLandingClipDuration() => clipLanding != null ? clipLanding.length : 55f;

        /// <summary>Длина клипа Monologue (второй монолог). Используется IntroSequencer.</summary>
        public float GetMonologueClipDuration() => clipMonologue != null ? clipMonologue.length : 40f;

        // ─── Очередь воспроизведения ─────────────────────────────────────────────

        private void Enqueue(AudioClip clip)
        {
            if (clip == null) return;
            clipQueue.Enqueue(clip);
            Debug.Log($"[AudioManager] Очередь +«{clip.name}» (в очереди: {clipQueue.Count})");
            if (playbackCoroutine == null)
                playbackCoroutine = StartCoroutine(PlayQueue());
        }

        private IEnumerator PlayQueue()
        {
            while (clipQueue.Count > 0)
            {
                AudioClip next = clipQueue.Dequeue();
                if (next == null) continue;

                narratorSource.clip = next;
                narratorSource.Play();
                Debug.Log($"[AudioManager] Играет: «{next.name}»");

                yield return new WaitWhile(() => narratorSource.isPlaying);
                yield return new WaitForSeconds(0.3f);
            }
            playbackCoroutine = null;
        }

        private void PlayEnd()
        {
            int correct = GameManager.Instance != null && GameManager.Instance.Quiz != null
                ? GameManager.Instance.Quiz.GetCorrectAnswersCount()
                : 0;

            AudioClip resultClip = correct == 5 ? clipEndPerfect
                                 : correct >= 3 ? clipEndGood
                                 : clipEndBad;

            Enqueue(resultClip);
            Enqueue(clipNextAdventure);
        }
    }
}