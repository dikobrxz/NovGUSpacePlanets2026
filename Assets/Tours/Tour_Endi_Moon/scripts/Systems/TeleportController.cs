using System.Collections;
using UnityEngine;

namespace MoonGame
{
    /// <summary>
    /// Управляет телепортацией игрока между тремя спавн-точками.
    /// Каждый переход сопровождается затемнением экрана и звуком телепорта.
    ///
    /// Точки:
    ///   spawn1 — посадка (Intro / Landing)
    ///   spawn2 — монолог и квиз (Monologue / Quiz)
    ///   spawn3 — раскопки (Exploration / Collecting)
    ///
    /// Автоматические переходы:
    ///   Monologue → teleport spawn1 → spawn2
    ///   Exploration → teleport spawn2 → spawn3
    ///   Return     → teleport spawn3 → spawn2 → Quiz (с задержкой)
    /// </summary>
    public class TeleportController : MonoBehaviour
    {
        [Header("Три спавн-точки")]
        [Tooltip("Spawn 1 — посадка на Луне (Intro / Landing)")]
        [SerializeField] private Transform spawn1;

        [Tooltip("Spawn 2 — монолог и финальный квиз (внутри/у корабля)")]
        [SerializeField] private Transform spawn2;

        [Tooltip("Spawn 3 — зона раскопок")]
        [SerializeField] private Transform spawn3;

        [Header("XR Rig — объект, который телепортируем")]
        [SerializeField] private Transform xrRig;

        [Header("Фейдер экрана")]
        [SerializeField] private ScreenFader fader;

        [Header("Звук телепорта")]
        [SerializeField] private AudioSource teleportAudioSource;
        [SerializeField] private AudioClip teleportClip;

        [Header("Задержка после появления перед снятием чёрного (сек)")]
        [SerializeField] private float settleDelay = 0.2f;

        [Header("Задержка перед запуском квиза после возврата (сек)")]
        [SerializeField] private float quizStartDelay = 1.5f;

        [Header("Locomotion")]
        [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement.ContinuousMoveProvider moveProvider;

        private StoryManager story;

        private void Start()
        {
            if (moveProvider != null)
                moveProvider.enabled = false;
            story = GameManager.Instance != null
                ? GameManager.Instance.Story
                : FindFirstObjectByType<StoryManager>();

            if (story != null)
                story.OnStateChanged += HandleStateChanged;

            if (xrRig == null)
            {
                var rig = FindFirstObjectByType<Unity.XR.CoreUtils.XROrigin>();
                if (rig != null) xrRig = rig.transform;
            }

            // Всегда ставим игрока на Spawn 1 при старте — без fade и звука
            if (spawn1 != null)
                MoveRig(spawn1);
        }

        private void OnDestroy()
        {
            if (story != null)
                story.OnStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState state)
        {
            if (moveProvider != null)
                moveProvider.enabled = state == GameState.Exploration
                                    || state == GameState.Collecting
                                    || state == GameState.Return;

            switch (state)
            {
                // Spawn 1 → Spawn 2: второй монолог
                case GameState.Monologue when spawn2 != null:
                    StartCoroutine(DoTeleport(spawn2, afterTeleport: null));
                    break;

                // Spawn 2 → Spawn 3: раскопки
                case GameState.Exploration when spawn3 != null:
                    StartCoroutine(DoTeleport(spawn3, afterTeleport: null));
                    break;

                // Return — ждём, когда игрок сам встанет на платформу (ReturnPlatform).
                // Телепорт будет вызван через TriggerReturnTeleport().
                case GameState.Return:
                    break;
            }
        }

        /// <summary>Ручной телепорт в произвольную точку (без смены состояния).</summary>
        public void Teleport(Transform destination)
        {
            if (destination == null) return;
            StartCoroutine(DoTeleport(destination, afterTeleport: null));
        }

        /// <summary>
        /// Вызывается из ReturnPlatform, когда игрок встаёт на платформу в стадии Return.
        /// Делает fade-телепорт на Spawn 2, затем запускает квиз.
        /// </summary>
        public void TriggerReturnTeleport()
        {
            Debug.Log("[TeleportController] TriggerReturnTeleport вызван!");
            if (spawn2 == null)
            {
                Debug.LogError("[TeleportController] spawn2 не назначен!");
                return;
            }
            StartCoroutine(DoTeleport(spawn2, afterTeleport: StartQuizDelayed));
        }

        private IEnumerator DoTeleport(Transform destination, System.Action afterTeleport)
        {
            Debug.Log($"[TeleportController] DoTeleport → {destination.name}, fader: {fader}");
            if (fader == null)
            {
                MoveRig(destination);
                afterTeleport?.Invoke();
                yield break;
            }

            yield return StartCoroutine(fader.FadeOut());
            PlayTeleportSound();
            yield return new WaitForSeconds(settleDelay);
            MoveRig(destination);
            yield return StartCoroutine(fader.FadeIn());

            afterTeleport?.Invoke();
        }

        private void StartQuizDelayed()
        {
            StartCoroutine(QuizAfterDelay());
        }

        private IEnumerator QuizAfterDelay()
        {
            yield return new WaitForSeconds(quizStartDelay);

            // Принимаем Quiz из состояний Return и Collecting —
            // не проверяем строго Return, так как состояние могло уже измениться
            var stage = story != null ? story.GetCurrentStage() : GameState.End;
            if (stage == GameState.Return || stage == GameState.Collecting)
            {
                Debug.Log("[TeleportController] Запуск квиза после возврата на Spawn 2.");
                story.SetStage(GameState.Quiz);
            }
            else
            {
                Debug.LogWarning($"[TeleportController] QuizAfterDelay: неожиданное состояние {stage}, квиз пропущен.");
            }
        }

        private void MoveRig(Transform destination)
        {
            if (xrRig == null)
            {
                Debug.LogError("[TeleportController] xrRig не задан.");
                return;
            }

            // Компенсируем горизонтальное смещение камеры относительно рута рига.
            // Это устраняет сдвиг, вызванный трекингом гарнитуры в реальном пространстве.
            var xrOrigin = xrRig.GetComponent<Unity.XR.CoreUtils.XROrigin>();
            if (xrOrigin != null && xrOrigin.Camera != null)
            {
                Vector3 cameraOffset = xrOrigin.Camera.transform.position - xrRig.position;
                cameraOffset.y = 0f; // компенсируем только горизонталь; высоту рига не трогаем
                xrRig.position = destination.position - cameraOffset;
            }
            else
            {
                xrRig.position = destination.position;
            }

            xrRig.rotation = destination.rotation;
            Debug.Log($"[TeleportController] Телепорт → {destination.name}");
        }

        private void PlayTeleportSound()
        {
            if (teleportAudioSource == null || teleportClip == null) return;
            teleportAudioSource.PlayOneShot(teleportClip);
        }
    }
}
