using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tour_Endi_Moon
{
    public class QuizUI : MonoBehaviour
    {
        [Header("Корневой Canvas")]
        [SerializeField] private GameObject root;

        [Header("Текстовое поле вопроса")]
        [SerializeField] private TMP_Text questionText;

        [Header("Кнопки вариантов (до 4)")]
        [SerializeField] private Button[] optionButtons;
        [SerializeField] private TMP_Text[] optionLabels;

        [Header("Финальный экран")]
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private TMP_Text resultText;

        [Header("Цвета ответов")]
        [SerializeField] private Color correctColor   = new Color(0.2f, 0.8f, 0.2f);
        [SerializeField] private Color wrongColor     = new Color(0.9f, 0.2f, 0.2f);
        [SerializeField] private Color defaultColor   = Color.white;

        [Header("Задержка перед следующим вопросом (сек)")]
        [SerializeField] private float feedbackDelay = 1.5f;

        [Header("Звуки")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip correctClip;
        [SerializeField] private AudioClip wrongClip;

        private Action<int> currentCallback;

        public void SetVisible(bool visible)
        {
            if (root != null) root.SetActive(visible);
        }

        public void ShowQuestion(QuizQuestion q, Action<int> onAnswer)
        {
            if (resultPanel != null) resultPanel.SetActive(false);

            currentCallback = onAnswer;

            if (questionText != null)
                questionText.text = q.text;

            for (int i = 0; i < optionButtons.Length; i++)
            {
                if (optionButtons[i] == null) continue;

                bool active = i < q.options.Length;
                optionButtons[i].gameObject.SetActive(active);
                if (!active) continue;

                // Сброс цвета
                optionButtons[i].image.color = defaultColor;
                optionButtons[i].interactable = true;

                if (optionLabels != null && i < optionLabels.Length && optionLabels[i] != null)
                    optionLabels[i].text = q.options[i];

                int captured = i;
                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].onClick.AddListener(() => OnButtonClicked(captured, q.correctIndex));
            }
        }

        private void OnButtonClicked(int chosenIndex, int correctIndex)
        {
            // Блокируем все кнопки чтобы не нажали дважды
            foreach (var b in optionButtons)
                if (b != null) b.interactable = false;

            StartCoroutine(ShowFeedback(chosenIndex, correctIndex));
        }

        private IEnumerator ShowFeedback(int chosenIndex, int correctIndex)
        {
            bool isCorrect = chosenIndex == correctIndex;

            // Подсветка выбранной кнопки
            if (chosenIndex < optionButtons.Length && optionButtons[chosenIndex] != null)
                optionButtons[chosenIndex].image.color = isCorrect ? correctColor : wrongColor;

            // Если ответ неверный — ещё подсветить правильный зелёным
            if (!isCorrect && correctIndex < optionButtons.Length && optionButtons[correctIndex] != null)
                optionButtons[correctIndex].image.color = correctColor;

            // Звук
            if (audioSource != null)
            {
                var clip = isCorrect ? correctClip : wrongClip;
                if (clip != null) audioSource.PlayOneShot(clip);
            }

            yield return new WaitForSeconds(feedbackDelay);

            // Сброс цветов
            foreach (var b in optionButtons)
                if (b != null) b.image.color = defaultColor;

            currentCallback?.Invoke(chosenIndex);
        }

        public void ShowResult(int correct, int total)
        {
            if (resultPanel != null) resultPanel.SetActive(true);
            if (resultText != null) resultText.text = BuildResultText(correct, total);

            if (questionText != null) questionText.text = string.Empty;
            foreach (var b in optionButtons) if (b != null) b.gameObject.SetActive(false);
        }

        private static string BuildResultText(int correct, int total)
        {
            string phrase = correct == total
                ? "Это невероятный успех нашего с тобой исследования Луны!\nТвои знания помогут человечеству узнать больше о нашей Солнечной системе."
                : correct >= 3
                    ? "Твоих знаний уже достаточно, чтобы помочь учёным организовать новую миссию по исследованию Луны."
                    : "Думаю, нам стоит ещё раз посетить Луну, чтобы узнать немного больше.";

            return $"Правильных ответов: {correct} из {total}\n\n{phrase}";
        }
    }
}
