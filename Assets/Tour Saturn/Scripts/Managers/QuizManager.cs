using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

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
    [SerializeField] private GameObject quizPanel;
    [SerializeField] private GameObject resultsPanel;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private QuizAnswerButton[] answerButtons;

    [Header("Score UI")]
    [SerializeField] private TMP_Text correctAnswersText;
    [SerializeField] private TMP_Text wrongAnswersText;

    /*[Header("Audio")]
    [SerializeField] private AudioManager audioManager;*/

    [Header("Settings")]
    [SerializeField] private float resultShowTime = 1f;

    public UnityEvent OnQuizFinished;

    private int currentQuestionIndex;
    private int correctAnswers;
    private int wrongAnswers;
    private bool answerSelected;

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
        wrongAnswers = 0;
        answerSelected = false;

        quizCanvas.SetActive(true);
        quizPanel.SetActive(true);
        resultsPanel.SetActive(false);

        ShowQuestion();
    }

    private void ShowQuestion()
    {
        answerSelected = false;

        QuizQuestion question = questions[currentQuestionIndex];

        questionText.text = question.Question;

        foreach (var button in answerButtons)
        {
            button.ResetColor();
        }

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

    public void ChooseAnswer(int answerIndex, QuizAnswerButton selectedButton)
    {
        if (answerSelected)
            return;

        StartCoroutine(ChooseAnswerRoutine(answerIndex, selectedButton));
    }

    private IEnumerator ChooseAnswerRoutine(int answerIndex, QuizAnswerButton selectedButton)
    {
        answerSelected = true;

        QuizQuestion question = questions[currentQuestionIndex];

        bool isCorrect = answerIndex == question.CorrectAnswerIndex;

        if (isCorrect)
        {
            correctAnswers++;
            selectedButton.SetCorrect();
        }
        else
        {
            wrongAnswers++;
            selectedButton.SetWrong();

            if (question.CorrectAnswerIndex >= 0 &&
                question.CorrectAnswerIndex < answerButtons.Length)
            {
                answerButtons[question.CorrectAnswerIndex].SetCorrect();
            }
        }

        yield return new WaitForSeconds(resultShowTime);

        currentQuestionIndex++;

        if (currentQuestionIndex >= questions.Length)
        {
            FinishQuiz();
        }
        else
        {
            answerSelected = false;
            ShowQuestion();
        }
    }

    private void UpdateScoreUI()
    {
        correctAnswersText.text = $"Правильно: {correctAnswers}";
        wrongAnswersText.text = $"Ошибки: {wrongAnswers}";
    }

    /*public void ChooseAnswer(int answerIndex)
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
    }*/

    private void FinishQuiz()
    {
        //quizCanvas.SetActive(false);
        quizPanel.SetActive(false);
        resultsPanel.SetActive(true);
        UpdateScoreUI();

        OnQuizFinished?.Invoke();

        Debug.Log($"Квиз завершён. Правильных ответов: {correctAnswers}");
    }
}
