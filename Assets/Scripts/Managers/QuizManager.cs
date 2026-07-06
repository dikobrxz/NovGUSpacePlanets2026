using UnityEngine;
using TMPro;

public class QuizManager : MonoBehaviour
{
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

    private int currentQuestion = 0;
    private int correctAnswers = 0;

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

    private int[] correctIndices = { 2, 2, 0, 1, 1 };

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
        questionText.text = questions[currentQuestion];
        for (int i = 0; i < answerTexts.Length; i++)
        {
            if (i < answers[currentQuestion].Length && answers[currentQuestion][i] != "")
                answerTexts[i].transform.parent.gameObject.SetActive(true);
            else
                answerTexts[i].transform.parent.gameObject.SetActive(false);

            answerTexts[i].text = answers[currentQuestion][i];
        }
    }

    public void OnAnswerSelected(int index)
    {
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

        currentQuestion++;

        if (currentQuestion < questions.Length)
            ShowQuestion();
        else
            ShowResult();
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
                resultText.text = "Молодец! Твоих знаний хватит для новой миссии!";
            else
                resultText.text = "Попробуй ещё раз! Стоит снова посетить планету.";
        }

        if (correctAnswerText != null)
            correctAnswerText.text = "Правильно: " + correctAnswers;

        if (wrongAnswerText != null)
            wrongAnswerText.text = "Не правильно: " + wrongCount;
    }
}