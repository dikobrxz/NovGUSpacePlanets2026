using System.Collections;
using UnityEngine;

namespace Tour_Endi_Moon
{
    /// <summary>
    /// Управляет автоматическими переходами первых трёх этапов сценария.
    ///
    /// Intro   (Spawn 1): показывает вращающуюся Луну, ждёт окончания вступительной
    ///                    озвучки → переходит к Landing.
    /// Landing (Spawn 1): ждёт окончания первого монолога о Луне → переходит к Monologue.
    ///                    TeleportController сделает fade + телепорт на Spawn 2.
    /// Monologue (Spawn 2): ждёт окончания второго монолога → переходит к Exploration.
    ///                    TeleportController сделает fade + телепорт на Spawn 3.
    /// </summary>
    public class IntroSequencer : MonoBehaviour
    {
        [Header("Объекты стартового экрана (видны только на этапе Intro)")]
        [Tooltip("Вращающаяся Луна внутри корабля")]
        [SerializeField] private GameObject introMoonObject;

        [Header("Паузы")]
        [Tooltip("Задержка после интро-озвучки перед телепортом на Spawn 1 (сек)")]
        [SerializeField] private float postIntroPause = 1.5f;

        [Tooltip("Задержка после fade-in на Spawn 2 перед началом второго монолога (сек)")]
        [SerializeField] private float postMonologueFadePause = 1.5f;

        private StoryManager story;
        private AudioManager audioManager;

        private void Start()
        {
            story = GameManager.Instance != null
                ? GameManager.Instance.Story
                : FindFirstObjectByType<StoryManager>();
            audioManager = GameManager.Instance != null
                ? GameManager.Instance.Audio
                : FindFirstObjectByType<AudioManager>();

            if (story != null)
                story.OnStateChanged += HandleStateChanged;

            // НЕ запускаем Intro вручную при старте сцены.
            // Сценарий стартует только после нажатия кнопки «Начать»
            // (GameManager.BeginStory → SetStageForce(Intro) → событие OnStateChanged).
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
                    StartCoroutine(RunIntroSequence());
                    break;
                case GameState.Landing:
                    StartCoroutine(RunLandingSequence());
                    break;
                case GameState.Monologue:
                    StartCoroutine(RunMonologueSequence());
                    break;
            }
        }

        // ─── Этап Intro ──────────────────────────────────────────────────────────

        private IEnumerator RunIntroSequence()
        {
            if (introMoonObject != null) introMoonObject.SetActive(true);

            float duration = audioManager != null ? audioManager.GetIntroClipDuration() : 30f;
            yield return new WaitForSeconds(duration + postIntroPause);

            if (introMoonObject != null) introMoonObject.SetActive(false);

            if (story != null && story.GetCurrentStage() == GameState.Intro)
                story.SetStage(GameState.Landing);
        }

        // ─── Этап Landing ─────────────────────────────────────────────────────────

        private IEnumerator RunLandingSequence()
        {
            float duration = audioManager != null ? audioManager.GetLandingClipDuration() : 55f;
            yield return new WaitForSeconds(duration + 1f);

            if (story != null && story.GetCurrentStage() == GameState.Landing)
                story.SetStage(GameState.Monologue);
        }

        // ─── Этап Monologue ───────────────────────────────────────────────────────

        private IEnumerator RunMonologueSequence()
        {
            yield return new WaitForSeconds(postMonologueFadePause);

            float duration = audioManager != null ? audioManager.GetMonologueClipDuration() : 40f;
            yield return new WaitForSeconds(duration + 1f);

            if (story != null && story.GetCurrentStage() == GameState.Monologue)
                story.SetStage(GameState.Exploration);
        }
    }
}