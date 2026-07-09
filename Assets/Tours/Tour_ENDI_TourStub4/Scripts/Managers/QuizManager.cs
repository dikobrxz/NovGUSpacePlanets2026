using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tour_ENDI_TourStub4
{
    [Serializable]
    public class QuizQuestion
    {
        [TextArea] public string text;
        public string[] options;
        public int correctIndex;
    }

    /// <summary>
    /// Менеджер квиза. Содержит 5 вопросов из сценария "Луна",
    /// последовательно показывает их через QuizUI и считает правильные ответы.
    /// </summary>
    public class QuizManager : MonoBehaviour
    {
        [Header("UI квиза (World-Space Canvas)")]
        [SerializeField] private QuizUI quizUI;

        [Header("Вопросы (заполнены по умолчанию из сценария)")]
        [SerializeField]
        private List<QuizQuestion> questions = new()
        {
            new QuizQuestion {
                text = "Какой единственный объект Солнечной системы, кроме Земли, на котором побывал человек?",
                options = new[] { "Марс", "Венера", "Луна", "Человек нигде не был, кроме Земли" },
                correctIndex = 2
            },
            new QuizQuestion {
                text = "В каком году человек впервые побывал на Луне?",
                options = new[] { "1969", "2015", "2024", "1986" },
                correctIndex = 0
            },
            new QuizQuestion {
                text = "Чем является луна",
                options = new[] { "Планета", "Звезда", "Луна — спутник Земли" },
                correctIndex = 2
            },
            new QuizQuestion {
                text = "Какое расстояние от Земли до Луны?",
                options = new[] { "Около 384 тыс. км", "Около 100 тыс. км", "Около 500 тыс. км" },
                correctIndex = 0
            },
            new QuizQuestion {
                text = "Есть ли жизнь на Луне?",
                options = new[] { "Да", "Нет" },
                correctIndex = 1
            },
        };

        private int currentIndex = -1;
        private int correctCount;
        private StoryManager story;

        private void Start()
        {
            story = GameManager.Instance != null ? GameManager.Instance.Story : FindFirstObjectByType<StoryManager>();
            if (story != null) story.OnStateChanged += HandleStateChanged;
            if (quizUI != null) quizUI.SetVisible(false);
        }

        private void OnDestroy()
        {
            if (story != null) story.OnStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState state)
        {
            if (state == GameState.Quiz)
                BeginQuiz();
        }

        public int GetCorrectAnswersCount() => correctCount;

        private void BeginQuiz()
        {
            currentIndex = -1;
            correctCount = 0;
            if (quizUI != null) quizUI.SetVisible(true);
            ShowNextQuestion();
        }

        private void ShowNextQuestion()
        {
            currentIndex++;
            if (currentIndex >= questions.Count)
            {
                FinishQuiz();
                return;
            }

            if (quizUI != null)
                quizUI.ShowQuestion(questions[currentIndex], OnAnswerSelected);
        }

        private void OnAnswerSelected(int chosenIndex)
        {
            var q = questions[currentIndex];
            bool correct = chosenIndex == q.correctIndex;
            if (correct) correctCount++;
            Debug.Log($"[QuizManager] Вопрос {currentIndex + 1}: выбран {chosenIndex}, " +
                      $"{(correct ? "верно" : "неверно")}. Счёт: {correctCount}");
            ShowNextQuestion();
        }

        private void FinishQuiz()
        {
            Debug.Log($"[QuizManager] Квиз завершён. Правильных: {correctCount}/{questions.Count}");
            if (quizUI != null) quizUI.ShowResult(correctCount, questions.Count);
            if (story != null) story.SetStage(GameState.End);
        }
    }
}
