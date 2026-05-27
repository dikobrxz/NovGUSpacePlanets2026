using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class QuizManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text questionText;
    public TMP_Text scoreText;
    public Button[] answerButtons;
    public Button restartButton;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] resultAudioClips;

    private List<Question> questions = new List<Question>();
    private int currentQuestionIndex = 0;
    private int correctAnswers = 0;
    private bool quizCompleted = false;

    [System.Serializable]
    public class Question
    {
        public string questionText;
        public string[] answers = new string[4];
        public int correctAnswerIndex;
        public int numberOfAnswers;
    }

    void Start()
    {
        LoadQuestions();

        restartButton.onClick.RemoveAllListeners();
        restartButton.onClick.AddListener(RestartQuiz);

        ShowQuestion();
        UpdateScoreUI();

        restartButton.gameObject.SetActive(false);
    }

    void LoadQuestions()
    {
        questions.Clear();

        Question q1 = new Question();
        q1.questionText = "Какой по размерам Уран?";
        q1.answers[0] = "А) Самый большой в Солнечной системе";
        q1.answers[1] = "Б) Второй по размерам";
        q1.answers[2] = "В) Третий по размерам";
        q1.answers[3] = "";
        q1.correctAnswerIndex = 2;
        q1.numberOfAnswers = 3;
        questions.Add(q1);

        Question q2 = new Question();
        q2.questionText = "Сколько лететь до Урана?";
        q2.answers[0] = "А) 1 год";
        q2.answers[1] = "Б) 3 года";
        q2.answers[2] = "В) 6,5 года";
        q2.answers[3] = "Г) 8,5 года";
        q2.correctAnswerIndex = 2;
        q2.numberOfAnswers = 4;
        questions.Add(q2);

        Question q3 = new Question();
        q3.questionText = "Из чего состоит Уран?";
        q3.answers[0] = "А) Лёд";
        q3.answers[1] = "Б) Земля";
        q3.answers[2] = "В) Огонь";
        q3.answers[3] = "Г) Газ";
        q3.correctAnswerIndex = 0;
        q3.numberOfAnswers = 4;
        questions.Add(q3);

        Question q4 = new Question();
        q4.questionText = "Посещали ли люди планету?";
        q4.answers[0] = "А) Да";
        q4.answers[1] = "Б) Нет";
        q4.answers[2] = "";
        q4.answers[3] = "";
        q4.correctAnswerIndex = 1;
        q4.numberOfAnswers = 2;
        questions.Add(q4);

        Question q5 = new Question();
        q5.questionText = "Есть ли жизнь на Уране?";
        q5.answers[0] = "А) Да";
        q5.answers[1] = "Б) Нет";
        q5.answers[2] = "В) Когда-то была";
        q5.answers[3] = "Г) Возможно, существует на его спутниках";
        q5.correctAnswerIndex = 3;
        q5.numberOfAnswers = 4;
        questions.Add(q5);
    }

    void ShowQuestion()
    {
        if (currentQuestionIndex >= questions.Count)
        {
            EndQuiz();
            return;
        }

        Question q = questions[currentQuestionIndex];
        questionText.text = q.questionText;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < q.numberOfAnswers)
            {
                answerButtons[i].gameObject.SetActive(true);
                TMP_Text buttonText = answerButtons[i].GetComponentInChildren<TMP_Text>();
                if (buttonText != null)
                {
                    buttonText.text = q.answers[i];
                }
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnAnswerSelected(int answerIndex)
    {
        if (quizCompleted) return;

        Question q = questions[currentQuestionIndex];

        if (answerIndex == q.correctAnswerIndex)
        {
            correctAnswers++;
        }

        UpdateScoreUI();
        currentQuestionIndex++;

        if (currentQuestionIndex < questions.Count)
        {
            ShowQuestion();
        }
        else
        {
            EndQuiz();
        }
    }

    void UpdateScoreUI()
    {
        scoreText.text = "Правильно: " + correctAnswers + " / " + questions.Count;
    }

    void EndQuiz()
    {
        quizCompleted = true;

        foreach (Button btn in answerButtons)
        {
            btn.gameObject.SetActive(false);
        }

        restartButton.gameObject.SetActive(true);

        int audioClipIndex = 0;

        if (correctAnswers == 5)
        {
            audioClipIndex = 2;
        }
        else if (correctAnswers >= 4)
        {
            audioClipIndex = 0;
        }
        else if (correctAnswers <= 2)
        {
            audioClipIndex = 1;
        }
        else
        {
            audioClipIndex = 0;
        }

        if (audioSource != null && resultAudioClips != null && resultAudioClips.Length > audioClipIndex && resultAudioClips[audioClipIndex] != null)
        {
            audioSource.clip = resultAudioClips[audioClipIndex];
            audioSource.Play();
        }
    }

    public void RestartQuiz()
    {
        currentQuestionIndex = 0;
        correctAnswers = 0;
        quizCompleted = false;

        restartButton.gameObject.SetActive(false);

        UpdateScoreUI();
        ShowQuestion();
    }
}