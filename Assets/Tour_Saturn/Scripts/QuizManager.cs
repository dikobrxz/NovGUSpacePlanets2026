using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class QuizQuestion
{
    public string Question;
    public string[] Answers;
    public int CorrectAnswerIndex;

    public QuizQuestion(string question, string[] answers, int correctAnswerIndex)
    {
        Question = question;
        Answers = answers;
        CorrectAnswerIndex = correctAnswerIndex;
    }
}

public class QuizManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject quizCanvas;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private QuizAnswerButton[] answerButtons;

    [Header("Audio")]
    [SerializeField] private AudioManager audioManager;

    public UnityEvent OnQuizFinished;

    private int currentQuestionIndex;
    private int correctAnswers;

    private QuizQuestion[] questions;

    public int CorrectAnswers => correctAnswers;

    private void Awake()
    {
        questions = new QuizQuestion[]
        {
            new QuizQuestion(
                "Какой по размерам Сатурн?",
                new string[] { "Самый большой в Солнечной системе", "Второй по размерам", "Третий по размерам" },
                1
            ),

            new QuizQuestion(
                "Как называется один из самых известных спутников Сатурна?",
                new string[] { "Меркурий", "Титан", "Луна", "Сатурн" },
                1
            ),

            new QuizQuestion(
                "На какой планете, по нашему путешествию, дуют очень сильные ветра?",
                new string[] { "Юпитер", "Сатурн", "Земля", "Марс" },
                1
            ),

            new QuizQuestion(
                "Посещали ли люди планету?",
                new string[] { "Да", "Нет" },
                1
            ),

            new QuizQuestion(
                "Есть ли жизнь на Сатурне?",
                new string[] { "Да", "Нет", "Когда-то была", "Возможно, существует на его спутниках" },
                1
            )
        };

        quizCanvas.SetActive(false);
    }

    public void StartQuiz()
    {
        currentQuestionIndex = 0;
        correctAnswers = 0;

        quizCanvas.SetActive(true);
        ShowQuestion();
    }

    private void ShowQuestion()
    {
        QuizQuestion question = questions[currentQuestionIndex];

        questionText.text = question.Question;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < question.Answers.Length)
            {
                answerButtons[i].gameObject.SetActive(true);
                answerButtons[i].SetAnswer(question.Answers[i], i, this);
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void ChooseAnswer(int answerIndex)
    {
        QuizQuestion question = questions[currentQuestionIndex];

        if (answerIndex == question.CorrectAnswerIndex)
            correctAnswers++;

        currentQuestionIndex++;

        if (currentQuestionIndex >= questions.Length)
        {
            FinishQuiz();
        }
        else
        {
            ShowQuestion();
        }
    }

    private void FinishQuiz()
    {
        quizCanvas.SetActive(false);

        if (correctAnswers == 5)
            audioManager.PlayFinalResult(AudioType.QuizPerfect);
        else if (correctAnswers >= 3)
            audioManager.PlayFinalResult(AudioType.QuizGood);
        else
            audioManager.PlayFinalResult(AudioType.QuizBad);

        OnQuizFinished?.Invoke();

        Debug.Log($"Квиз завершён. Правильных ответов: {correctAnswers}");
    }
}
