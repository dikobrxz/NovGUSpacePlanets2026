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

    [Header("First Question Texts")]
    [TextArea]
    [SerializeField] private string firstQuestionText;
    [SerializeField] private string firstQuestionFirstAnswerText;
    [SerializeField] private string firstQuestionSecondAnswerText;
    [SerializeField] private string firstQuestionThirdAnswerText;

    [Header("Second Question Texts")]
    [TextArea]
    [SerializeField] private string secondQuestionText;
    [SerializeField] private string secondQuestionFirstAnswerText;
    [SerializeField] private string secondQuestionSecondAnswerText;
    [SerializeField] private string secondQuestionThirdAnswerText;
    [SerializeField] private string secondQuestionFourthAnswerText;

    [Header("Third Question Texts")]
    [TextArea]
    [SerializeField] private string thirdQuestionText;
    [SerializeField] private string thirdQuestionFirstAnswerText;
    [SerializeField] private string thirdQuestionSecondAnswerText;
    [SerializeField] private string thirdQuestionThirdAnswerText;
    [SerializeField] private string thirdQuestionFourthAnswerText;

    [Header("Fourth Question Texts")]
    [TextArea]
    [SerializeField] private string fourthQuestionText;
    [SerializeField] private string fourthQuestionFirstAnswerText;
    [SerializeField] private string fourthQuestionSecondAnswerText;

    [Header("Fifth Question Texts")]
    [TextArea]
    [SerializeField] private string fifthQuestionText;
    [SerializeField] private string fifthQuestionFirstAnswerText;
    [SerializeField] private string fifthQuestionSecondAnswerText;
    [SerializeField] private string fifthQuestionThirdAnswerText;
    [SerializeField] private string fifthQuestionFourthAnswerText;

    [Header("Result Score Texts")]
    [SerializeField] private string correctText;
    [SerializeField] private string wrongText;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip resultsClip;

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
                firstQuestionText,
                new string[] { firstQuestionFirstAnswerText, firstQuestionSecondAnswerText, firstQuestionThirdAnswerText },
                1
            ),

            new QuizQuestion(
                secondQuestionText,
                new string[] { secondQuestionFirstAnswerText, secondQuestionSecondAnswerText, secondQuestionThirdAnswerText, secondQuestionFourthAnswerText },
                1
            ),

            new QuizQuestion(
                thirdQuestionText,
                new string[] { thirdQuestionFirstAnswerText, thirdQuestionSecondAnswerText, thirdQuestionThirdAnswerText, thirdQuestionFourthAnswerText },
                1
            ),

            new QuizQuestion(
                fourthQuestionText,
                new string[] { fourthQuestionFirstAnswerText, fourthQuestionSecondAnswerText },
                1
            ),

            new QuizQuestion(
                fifthQuestionText,
                new string[] { fifthQuestionFirstAnswerText, fifthQuestionSecondAnswerText, fifthQuestionThirdAnswerText, fifthQuestionFourthAnswerText },
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

        selectedButton.PlayAnswerSound(isCorrect);

        if (isCorrect)
        {
            correctAnswers++;
            selectedButton.SetCorrect();
        }
        else
        {
            wrongAnswers++;
            selectedButton.SetWrong();

            if (question.CorrectAnswerIndex >= 0 && question.CorrectAnswerIndex < answerButtons.Length)
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
        correctAnswersText.text = correctText + correctAnswers;
        wrongAnswersText.text = wrongText + wrongAnswers;
    }

    private void FinishQuiz()
    {
        quizPanel.SetActive(false);
        resultsPanel.SetActive(true);
        UpdateScoreUI();

        audioSource.PlayOneShot(resultsClip);

        OnQuizFinished?.Invoke();

        Debug.Log($"Quiz Finished. Correct answers count: {correctAnswers}");
    }
}
