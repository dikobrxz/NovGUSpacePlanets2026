using System;
using UnityEngine;

namespace Tour_ENDI_TourStub4
{
    /// <summary>
    /// Менеджер сценария. Хранит текущее состояние и оповещает подписчиков о смене этапов.
    /// </summary>
    public class StoryManager : MonoBehaviour
    {
        [Header("Текущее состояние (для отладки в Inspector)")]
        [SerializeField] private GameState currentState = GameState.Intro;

        /// <summary>Событие смены этапа.</summary>
        public event Action<GameState> OnStateChanged;

        /// <summary>
        /// Устанавливает этап без проверки на повтор. Используется при старте сценария.
        /// </summary>
        public void SetStageForce(GameState newState)
        {
            currentState = newState;
            Debug.Log($"[StoryManager] Начальный этап: {currentState}");
            OnStateChanged?.Invoke(currentState);
        }

        /// <summary>Переход на конкретный этап.</summary>
        public void SetStage(GameState newState)
        {
            if (newState == currentState)
            {
                Debug.LogWarning($"[StoryManager] Попытка установить тот же этап: {newState}");
                return;
            }

            currentState = newState;
            Debug.Log($"[StoryManager] Этап: {currentState}");
            OnStateChanged?.Invoke(currentState);
        }

        /// <summary>Переход на следующий этап по порядку enum.</summary>
        public void NextStage()
        {
            int next = (int)currentState + 1;
            if (next > (int)GameState.End)
            {
                Debug.Log("[StoryManager] Сценарий уже завершён.");
                return;
            }
            SetStage((GameState)next);
        }

        /// <summary>Получить текущий этап.</summary>
        public GameState GetCurrentStage() => currentState;
    }
}
