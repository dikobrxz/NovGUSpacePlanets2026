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
    public Button actionButton;
    public CanvasGroup quizCanvasGroup;

    [Header("Audio")]
    public AudioSource quizAudioSource;
    public AudioClip lowScoreClip;
    public AudioClip mediumScoreClip;
    public AudioClip highScoreClip;
    public AudioClip finalMessageClip;

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
        if (quizCanvasGroup != null)
        {
            quizCanvasGroup.alpha = 0f;
            quizCanvasGroup.interactable = false;
            quizCanvasGroup.blocksRaycasts = false;
        }
        if (actionButton != null)
        {
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(OnActionButtonPressed);
        }
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
        }
        if (stageVisuals == null) stageVisuals = FindFirstObjectByType<StageVisuals>();
    }

    private void LoadQuestions()
    {
        questions.Clear();
        questions.Add(new Question
        {
            questionText = "Какой по размерам Уран?",
            answers = new[] { "А) Самый большой в Солнечной системе", "Б) Второй по размерам", "В) Третий по размерам", "" },
            correctAnswerIndex = 2,
            numberOfAnswers = 3
        });
        questions.Add(new Question
        {
            questionText = "Сколько лететь до Урана?",
            answers = new[] { "А) 1 год", "Б) 3 года", "В) 6,5 года", "Г) 8,5 года" },
            correctAnswerIndex = 2,
            numberOfAnswers = 4
        });
        questions.Add(new Question
        {
            questionText = "Из чего состоит Уран?",
            answers = new[] { "А) Лёд", "Б) Земля", "В) Огонь", "Г) Газ" },
            correctAnswerIndex = 0,
            numberOfAnswers = 4
        });
        questions.Add(new Question
        {
            questionText = "Посещали ли люди планету?",
            answers = new[] { "А) Да", "Б) Нет", "", "" },
            correctAnswerIndex = 1,
            numberOfAnswers = 2
        });
        questions.Add(new Question
        {
            questionText = "Есть ли жизнь на Уране?",
            answers = new[] { "А) Да", "Б) Нет", "В) Когда-то была", "Г) Возможно, существует на его спутниках" },
            correctAnswerIndex = 3,
            numberOfAnswers = 4
        });
    }

    public void StartQuiz()
    {
        if (quizCanvasGroup != null)
        {
            quizCanvasGroup.alpha = 1f;
            quizCanvasGroup.interactable = true;
            quizCanvasGroup.blocksRaycasts = true;
        }
        currentQuestionIndex = 0;
        correctAnswers = 0;
        quizCompleted = false;
        isWaitingForAudio = false;
        if (actionButton != null) actionButton.gameObject.SetActive(false);
        foreach (var btn in answerButtons)
        {
            btn.gameObject.SetActive(true);
            btn.interactable = true;
        }
        UpdateScoreUI();
        ShowQuestion();
    }

    private void ShowQuestion()
    {
        if (currentQuestionIndex >= questions.Count)
        {
            EndQuiz();
            return;
        }
        var q = questions[currentQuestionIndex];
        questionText.text = q.questionText;
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < q.numberOfAnswers)
            {
                answerButtons[i].gameObject.SetActive(true);
                var btnText = answerButtons[i].GetComponentInChildren<TMP_Text>();
                if (btnText != null) btnText.text = q.answers[i];
            }
            else answerButtons[i].gameObject.SetActive(false);
        }
    }

    public void OnAnswerSelected(int answerIndex)
    {
        if (quizCompleted || isWaitingForAudio) return;
        if (answerIndex == questions[currentQuestionIndex].correctAnswerIndex) correctAnswers++;
        UpdateScoreUI();
        currentQuestionIndex++;
        if (currentQuestionIndex < questions.Count) ShowQuestion();
        else EndQuiz();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = $"Правильно: {correctAnswers} / {questions.Count}";
    }

    private void EndQuiz()
    {
        quizCompleted = true;
        foreach (var btn in answerButtons) btn.gameObject.SetActive(false);

        bool showRestart = correctAnswers <= 2;
        string buttonText = showRestart ? "Заново" : "Конец";
        AudioClip clip = correctAnswers <= 2 ? lowScoreClip : (correctAnswers >= 5 ? highScoreClip : mediumScoreClip);
        bool playFinal = correctAnswers >= 3;

        if (actionButton != null)
        {
            var tmpText = actionButton.GetComponentInChildren<TMP_Text>();
            if (tmpText != null) tmpText.text = buttonText;
            actionButton.gameObject.SetActive(true);
        }
        PlayResultAudio(clip, playFinal);
    }

    private void PlayResultAudio(AudioClip clip, bool playFinalMessage)
    {
        if (quizAudioSource == null) return;
        isWaitingForAudio = true;
        StartCoroutine(PlayResultRoutine(clip, playFinalMessage));
    }

    private IEnumerator PlayResultRoutine(AudioClip resultClip, bool playFinalMessage)
    {
        if (resultClip != null)
        {
            quizAudioSource.Stop();
            quizAudioSource.clip = resultClip;
            quizAudioSource.Play();
            yield return new WaitForSeconds(resultClip.length);
        }
        if (playFinalMessage && finalMessageClip != null)
        {
            quizAudioSource.Stop();
            quizAudioSource.clip = finalMessageClip;
            quizAudioSource.Play();
            yield return new WaitForSeconds(finalMessageClip.length);
        }
        isWaitingForAudio = false;
    }

    private void OnActionButtonPressed()
    {
        if (actionButton == null) return;
        var buttonText = actionButton.GetComponentInChildren<TMP_Text>();
        string label = buttonText != null ? buttonText.text : "";

        if (label == "Заново")
        {
            stageVisuals?.RestartScenario();
            if (quizCanvasGroup != null)
            {
                quizCanvasGroup.alpha = 0f;
                quizCanvasGroup.interactable = false;
                quizCanvasGroup.blocksRaycasts = false;
            }
        }
        else if (label == "Конец")
        {
            if (quizCanvasGroup != null)
            {
                quizCanvasGroup.alpha = 0f;
                quizCanvasGroup.interactable = false;
                quizCanvasGroup.blocksRaycasts = false;
            }
            stageVisuals?.EndScenario();
        }
    }
}