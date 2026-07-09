using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tour_Endi_Moon
{
    /// <summary>
    /// Менеджер квеста: отслеживает откопанные и сложенные в ящик артефакты.
    /// Использует HashSet для гарантии уникальности — один и тот же артефакт
    /// не может быть засчитан дважды ни на стадии откопки, ни на стадии укладки.
    /// </summary>
    public class QuestManager : MonoBehaviour
    {
        public const int TotalArtifacts = 3;

        public int UncoveredCount => uncoveredTypes.Count;
        public int StoredCount => storedTypes.Count;

        public event Action<ArtifactType> OnArtifactUncovered;
        public event Action<ArtifactType> OnArtifactStored;

        // HashSet по типу: не дублируем один и тот же тип
        private readonly HashSet<ArtifactType> uncoveredTypes = new();
        private readonly HashSet<ArtifactType> storedTypes = new();

        private StoryManager story;

        private void Start()
        {
            story = GameManager.Instance != null ? GameManager.Instance.Story : FindFirstObjectByType<StoryManager>();
        }

        /// <summary>Вызывается из Artifact, когда его откопали.</summary>
        public void RegisterUncovered(ArtifactType type)
        {
            if (!uncoveredTypes.Add(type))
            {
                Debug.LogWarning($"[QuestManager] {type} уже был откопан ранее — пропускаем.");
                return;
            }

            Debug.Log($"[QuestManager] Откопан: {type}. Всего откопано: {UncoveredCount}/{TotalArtifacts}");
            OnArtifactUncovered?.Invoke(type);

            if (UncoveredCount >= TotalArtifacts && story != null && story.GetCurrentStage() == GameState.Exploration)
                story.SetStage(GameState.Collecting);
        }

        /// <summary>Вызывается из ArtifactBox при попадании артефакта в ящик.</summary>
        public void RegisterStored(ArtifactType type)
        {
            if (!storedTypes.Add(type))
            {
                Debug.LogWarning($"[QuestManager] {type} уже в ящике — пропускаем.");
                return;
            }

            Debug.Log($"[QuestManager] В ящик: {type}. Всего в ящике: {StoredCount}/{TotalArtifacts}");
            OnArtifactStored?.Invoke(type);

            if (StoredCount >= TotalArtifacts && story != null)
            {
                var stage = story.GetCurrentStage();
                if (stage == GameState.Collecting || stage == GameState.Exploration)
                {
                    // TeleportController услышит Return и сделает fade + телепорт на Spawn 2 → Quiz
                    story.SetStage(GameState.Return);
                }
            }
        }
    }
}
