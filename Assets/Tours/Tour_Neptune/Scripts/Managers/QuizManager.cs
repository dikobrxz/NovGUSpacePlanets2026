using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class QuizManager : MonoBehaviour
{
    [SerializeField] private StoryManager _storyManager;
    [Header("UI")]
    public GameObject quizPanel;
    public GameObject resultsPanel;
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI[] answerTexts;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI correctAnswerText;
    public TextMeshProUGUI wrongAnswerText;

    [Header("Звуки")]
    public AudioSource quizAudio;
    public AudioClip correctSound;
    public AudioClip wrongSound;

    [Header("Цвета")]
    public Color normalColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    public Color correctColor = new Color(0.2f, 0.8f, 0.2f, 1f);
    public Color wrongColor = new Color(0.8f, 0.2f, 0.2f, 1f);

    private int currentQuestion = 0;
    private int correctAnswers = 0;
    private bool waitingForNext = false;

    private string[] questions = {
        "Какой по размерам Нептун?",
        "Сколько лететь до Нептуна?",
        "Из чего состоит Нептун?",
        "Посещали ли люди планету?",
        "Есть ли жизнь на Нептуне?"
    };

    private string[][] answers = {
        new string[] { "Самый большой", "Второй по размерам", "Третий по размерам", "Четвертый по размерам" },
        new string[] { "1 год", "3 года", "9,5 лет", "8,5 лет" },
        new string[] { "Лед", "Земля", "Огонь", "Газ" },
        new string[] { "Да", "Нет", "", "" },
        new string[] { "Да", "Нет", "Когда-то была", "Возможно на спутниках" }
    };

    private int[] correctIndices = { 3, 2, 0, 1, 1 };

    void Start()
    {
        if (quizPanel != null)
            quizPanel.SetActive(false);
        if (resultsPanel != null)
            resultsPanel.SetActive(false);
    }

    public void StartQuiz()
    {
        currentQuestion = 0;
        correctAnswers = 0;
        if (quizPanel != null)
            quizPanel.SetActive(true);
        if (resultsPanel != null)
            resultsPanel.SetActive(false);
        ShowQuestion();
    }

    void ShowQuestion()
    {
        waitingForNext = false;
        questionText.text = questions[currentQuestion];
        for (int i = 0; i < answerTexts.Length; i++)
        {
            if (i < answers[currentQuestion].Length && answers[currentQuestion][i] != "")
            {
                answerTexts[i].transform.parent.gameObject.SetActive(true);
                SetButtonColor(i, normalColor);
            }
            else
            {
                answerTexts[i].transform.parent.gameObject.SetActive(false);
            }
            answerTexts[i].text = answers[currentQuestion][i];
        }
    }

    public void OnAnswerSelected(int index)
    {
        if (waitingForNext) return;
        waitingForNext = true;

        bool isCorrect = index == correctIndices[currentQuestion];

        if (isCorrect)
        {
            correctAnswers++;
            if (quizAudio != null && correctSound != null)
                quizAudio.PlayOneShot(correctSound);
        }
        else
        {
            if (quizAudio != null && wrongSound != null)
                quizAudio.PlayOneShot(wrongSound);
        }

        // Подсвечиваем все кнопки
        for (int i = 0; i < answerTexts.Length; i++)
        {
            if (!answerTexts[i].transform.parent.gameObject.activeSelf) continue;

            if (i == correctIndices[currentQuestion])
                SetButtonColor(i, correctColor);
            else
                SetButtonColor(i, wrongColor);
        }

        StartCoroutine(NextQuestionDelay());
    }

    IEnumerator NextQuestionDelay()
    {
        yield return new WaitForSeconds(3f);

        currentQuestion++;

        if (currentQuestion < questions.Length)
            ShowQuestion();
        else
            ShowResult();
    }

    void SetButtonColor(int index, Color color)
    {
        Image img = answerTexts[index].transform.parent.GetComponent<Image>();
        if (img != null)
            img.color = color;
    }

    void ShowResult()
    {
        int wrongCount = questions.Length - correctAnswers;

        if (quizPanel != null)
            quizPanel.SetActive(false);
        if (resultsPanel != null)
            resultsPanel.SetActive(true);

        if (resultText != null)
        {
            if (correctAnswers == 5)
                resultText.text = "Невероятный успех! Твои знания помогут человечеству!";
            else if (correctAnswers >= 3)
            {
                resultText.text = "Молодец! Твоих знаний хватит для новой миссии!";
                StartCoroutine(RestartSceneCoroutine());
            }
            else
                resultText.text = "Попробуй ещё раз! Стоит снова посетить планету.";
        }

        if (correctAnswerText != null)
            correctAnswerText.text = "Правильно: " + correctAnswers;

        if (wrongAnswerText != null)
            wrongAnswerText.text = "Не правильно: " + wrongCount;
    }

    private IEnumerator RestartSceneCoroutine()
    {
        yield return new WaitForSeconds(5f);
        _storyManager.OnStartPressed();
    }
}