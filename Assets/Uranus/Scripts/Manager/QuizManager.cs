using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class QuizManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text questionText;
    public TMP_Text scoreText;
    public Button[] answerButtons;
    public Button restartButton;
    public Button endButton;               // EndButton на QuizCanvas
    public CanvasGroup quizCanvasGroup;    // Canvas Group на QuizCanvas (для скрытия)

    [Header("Audio")]
    public AudioSource quizAudioSource;
    public AudioClip[] resultAudioClips;  // 0=хорошо(3-4), 1=плохо(0-2), 2=отлично(5)

    [Header("Scene Control")]
    public StageVisuals stageVisuals;

    private List<Question> questions = new List<Question>();
    private int currentQuestionIndex = 0;
    private int correctAnswers = 0;
    private bool quizCompleted = false;
    private bool isWaitingForAudio = false;

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

        // Полностью скрываем квиз в начале
        if (quizCanvasGroup != null)
        {
            quizCanvasGroup.alpha = 0f;
            quizCanvasGroup.interactable = false;
            quizCanvasGroup.blocksRaycasts = false;
        }
        else if (quizCanvasGroup == null)
        {
            // Если Canvas Group нет, просто отключаем Canvas
            gameObject.SetActive(false);
        }

        if (restartButton != null) restartButton.gameObject.SetActive(false);
        if (endButton != null) endButton.gameObject.SetActive(false);

        // Назначаем слушателей
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartQuiz);

        if (endButton != null)
            endButton.onClick.AddListener(OnEndPressed);

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
        }

        if (stageVisuals == null)
            stageVisuals = FindFirstObjectByType<StageVisuals>();
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

    public void StartQuiz()
    {
        if (quizCanvasGroup != null)
        {
            quizCanvasGroup.alpha = 1f;
            quizCanvasGroup.interactable = true;
            quizCanvasGroup.blocksRaycasts = true;
        }
        else
        {
            gameObject.SetActive(true);
        }
        currentQuestionIndex = 0;
        correctAnswers = 0;
        quizCompleted = false;
        isWaitingForAudio = false;

        if (restartButton != null) restartButton.gameObject.SetActive(false);
        if (endButton != null) endButton.gameObject.SetActive(false);

        foreach (Button btn in answerButtons)
        {
            btn.gameObject.SetActive(true);
            btn.interactable = true;
        }

        UpdateScoreUI();
        ShowQuestion();
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
        if (quizCompleted || isWaitingForAudio) return;

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
        if (scoreText != null)
            scoreText.text = "Правильно: " + correctAnswers + " / " + questions.Count;
    }

    void EndQuiz()
    {
        quizCompleted = true;

        foreach (Button btn in answerButtons)
        {
            btn.gameObject.SetActive(false);
        }

        int audioClipIndex = 0;
        bool showRestartButton = false;

        if (correctAnswers == 5)
        {
            audioClipIndex = 2;
        }
        else if (correctAnswers >= 3)
        {
            audioClipIndex = 0;
        }
        else
        {
            audioClipIndex = 1;
            showRestartButton = true;
        }

        if (quizAudioSource != null && resultAudioClips != null && resultAudioClips.Length > audioClipIndex && resultAudioClips[audioClipIndex] != null)
        {
            isWaitingForAudio = true;
            quizAudioSource.clip = resultAudioClips[audioClipIndex];
            quizAudioSource.Play();
            StartCoroutine(WaitForAudioAndFinish(quizAudioSource.clip.length, showRestartButton));
        }
        else
        {
            FinishQuiz(showRestartButton);
        }
    }

    IEnumerator WaitForAudioAndFinish(float audioLength, bool showRestartButton)
    {
        yield return new WaitForSeconds(audioLength);
        FinishQuiz(showRestartButton);
    }

    void FinishQuiz(bool showRestartButton)
    {
        isWaitingForAudio = false;

        if (showRestartButton)
        {
            if (restartButton != null) restartButton.gameObject.SetActive(true);
            if (endButton != null) endButton.gameObject.SetActive(false);
        }
        else
        {
            if (endButton != null) endButton.gameObject.SetActive(true);
            if (restartButton != null) restartButton.gameObject.SetActive(false);
        }
    }

    public void RestartQuiz()
    {
        currentQuestionIndex = 0;
        correctAnswers = 0;
        quizCompleted = false;
        isWaitingForAudio = false;

        if (restartButton != null) restartButton.gameObject.SetActive(false);
        if (endButton != null) endButton.gameObject.SetActive(false);

        foreach (Button btn in answerButtons)
        {
            btn.gameObject.SetActive(true);
            btn.interactable = true;
        }

        UpdateScoreUI();
        ShowQuestion();
    }

    private void OnEndPressed()
    {
        Debug.Log("Завершение квиза");

        if (quizCanvasGroup != null)
        {
            quizCanvasGroup.alpha = 0f;
            quizCanvasGroup.interactable = false;
            quizCanvasGroup.blocksRaycasts = false;
        }
        else
        {
            gameObject.SetActive(false);
        }

        if (stageVisuals != null)
        {
            stageVisuals.OnExitButtonPressed();
        }
        else
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}