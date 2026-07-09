using TMPro;
using UnityEngine;

namespace Tour_ENDI_TourStub4
{
    /// <summary>
    /// Менеджер UI. Показывает подсказки и счётчик артефактов.
    /// Подписан на смену этапов сценария.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("World-Space Canvas с подсказками")]
        [SerializeField] private GameObject hintCanvas;
        [SerializeField] private TMP_Text hintText;

        [Header("Счётчик собранных артефактов")]
        [SerializeField] private TMP_Text artifactCounterText;

        private StoryManager story;
        private QuestManager quest;

        private void Start()
        {
            story = GameManager.Instance != null ? GameManager.Instance.Story : FindFirstObjectByType<StoryManager>();
            quest = GameManager.Instance != null ? GameManager.Instance.Quest : FindFirstObjectByType<QuestManager>();

            if (story != null) story.OnStateChanged += HandleStateChanged;
            if (quest != null)
            {
                quest.OnArtifactUncovered += _ => RefreshCounter();
                quest.OnArtifactStored += _ => RefreshCounter();
            }

            HideHint();
            RefreshCounter();
        }

        private void OnDestroy()
        {
            if (story != null) story.OnStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.Intro:
                    HideHint();
                    break;
                case GameState.Landing:
                    ShowHint("Ты на Луне! Послушай рассказ об этом небесном теле.");
                    break;
                case GameState.Exploration:
                    ShowHint("Возьми лопату и откопай 3 артефакта в кучках грунта.");
                    break;
                case GameState.Collecting:
                    ShowHint("Отлично! Сложи все артефакты в ящик у корабля.");
                    break;
                case GameState.Return:
                    ShowHint("Все артефакты в ящике. Возвращайся на платформу к кораблю.");
                    break;
                case GameState.Quiz:
                    ShowHint("Ответь на 5 вопросов о Луне.");
                    break;
                case GameState.End:
                    ShowHint("Спасибо за путешествие!");
                    break;
            }
        }

        /// <summary>Показывает подсказку.</summary>
        public void ShowHint(string text)
        {
            if (hintCanvas != null) hintCanvas.SetActive(true);
            if (hintText != null) hintText.text = text;
            Debug.Log($"[UIManager] Подсказка: {text}");
        }

        /// <summary>Скрывает подсказку.</summary>
        public void HideHint()
        {
            if (hintCanvas != null) hintCanvas.SetActive(false);
        }

        private void RefreshCounter()
        {
            if (artifactCounterText == null || quest == null) return;
            artifactCounterText.text = $"В ящике: {quest.StoredCount}/{QuestManager.TotalArtifacts}";
        }
    }
}
