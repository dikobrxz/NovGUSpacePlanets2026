using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class QuizManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text questionText;
    public Button[] answerButtons;
    public Button actionButton;
    public CanvasGroup quizCanvasGroup;
    public TMP_Text finalMessageText;

    [Header("Audio")]
    public AudioSource quizAudioSource;
    public AudioClip correctAnswerClip;
    public AudioClip wrongAnswerClip;
    public AudioClip lowScoreClip;
    public AudioClip mediumScoreClip;
    public AudioClip highScoreClip;
    public AudioClip finalMessageClip;

    [Header("Scene Control")]
    [SerializeField] private StageVisuals stageVisuals;
    [SerializeField] private ScenarioManager scenarioManager;

    private List<Question> questions = new List<Question>();
    private int currentIndex;
    private int correctCount;
    private bool completed;
    private bool waitingAudio;
    private bool waitingResult;

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
        quizCanvasGroup.alpha = 0f;
        quizCanvasGroup.interactable = false;
        quizCanvasGroup.blocksRaycasts = false;
        finalMessageText.gameObject.SetActive(false);
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(OnActionButtonPressed);
        actionButton.gameObject.SetActive(false);

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
        }

        if (stageVisuals == null) stageVisuals = FindFirstObjectByType<StageVisuals>();
        if (scenarioManager == null) scenarioManager = FindFirstObjectByType<ScenarioManager>();
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
        quizCanvasGroup.alpha = 1f;
        quizCanvasGroup.interactable = true;
        quizCanvasGroup.blocksRaycasts = true;

        currentIndex = 0;
        correctCount = 0;
        completed = false;
        waitingAudio = false;
        waitingResult = false;

        actionButton.gameObject.SetActive(false);
        finalMessageText.gameObject.SetActive(false);

        foreach (var btn in answerButtons)
        {
            btn.gameObject.SetActive(true);
            btn.interactable = true;
        }

        ShowQuestion();
    }

    private void ShowQuestion()
    {
        if (currentIndex >= questions.Count)
        {
            StartCoroutine(FinishWithDelay());
            return;
        }

        Question q = questions[currentIndex];
        questionText.text = q.questionText;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            bool active = i < q.numberOfAnswers;
            answerButtons[i].gameObject.SetActive(active);
            if (active)
            {
                TMP_Text btnText = answerButtons[i].GetComponentInChildren<TMP_Text>();
                if (btnText != null) btnText.text = q.answers[i];
            }
        }
    }

    public void OnAnswerSelected(int answerIndex)
    {
        if (completed || waitingAudio || waitingResult) return;

        bool correct = answerIndex == questions[currentIndex].correctAnswerIndex;
        if (correct) correctCount++;

        PlaySound(correct ? correctAnswerClip : wrongAnswerClip);

        currentIndex++;

        if (currentIndex < questions.Count)
        {
            ShowQuestion();
        }
        else
        {
            StartCoroutine(FinishWithDelay());
        }
    }

    private IEnumerator FinishWithDelay()
    {
        waitingResult = true;

        questionText.text = "";
        foreach (var btn in answerButtons) btn.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        waitingResult = false;
        EndQuiz();
    }

    private void EndQuiz()
    {
        completed = true;

        bool lowScore = correctCount <= 2;
        bool goodScore = correctCount >= 3;
        bool perfectScore = correctCount >= 5;

        AudioClip resultClip = lowScore ? lowScoreClip : (perfectScore ? highScoreClip : mediumScoreClip);
        bool playFinal = goodScore;

        if (lowScore)
        {
            SetupActionButton("Заново");
            finalMessageText.gameObject.SetActive(false);
        }
        else
        {
            actionButton.gameObject.SetActive(false);
            finalMessageText.text = "Конец";
            finalMessageText.gameObject.SetActive(true);
        }

        StartCoroutine(PlayResultSequence(resultClip, playFinal));
    }

    private void SetupActionButton(string text)
    {
        Button btn = actionButton.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnActionButtonPressed);

        TMP_Text label = actionButton.GetComponentInChildren<TMP_Text>();
        if (label != null) label.text = text;

        actionButton.gameObject.SetActive(true);
    }

    private IEnumerator PlayResultSequence(AudioClip resultClip, bool playFinal)
    {
        waitingAudio = true;

        if (resultClip != null)
        {
            quizAudioSource.Stop();
            quizAudioSource.clip = resultClip;
            quizAudioSource.Play();
            yield return new WaitForSeconds(resultClip.length);
        }

        if (playFinal && finalMessageClip != null)
        {
            yield return new WaitForSeconds(1f);
            quizAudioSource.Stop();
            quizAudioSource.clip = finalMessageClip;
            quizAudioSource.Play();
            yield return new WaitForSeconds(finalMessageClip.length);
        }

        waitingAudio = false;
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null) quizAudioSource.PlayOneShot(clip);
    }

    private void OnActionButtonPressed()
    {
        TMP_Text label = actionButton.GetComponentInChildren<TMP_Text>();
        if (label == null) return;

        if (label.text == "Заново")
        {
            stageVisuals?.RestartScenario();
            quizCanvasGroup.alpha = 0f;
            quizCanvasGroup.interactable = false;
            quizCanvasGroup.blocksRaycasts = false;
            finalMessageText.gameObject.SetActive(false);
        }
    }
}