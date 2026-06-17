using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Controls the Earth quiz stage.
/// Shows visual instructions, questions, answer buttons,
/// checks answers, counts score, and starts the final return to the ship.
/// </summary>
public class EarthQuizManager : MonoBehaviour
{
    [Header("Current Stage")]
    [SerializeField] private EarthTourStage currentStage = EarthTourStage.SurfaceIntro;

    public EarthTourStage CurrentStage => currentStage;

    [System.Serializable]
    public class QuizQuestion
    {
        [TextArea(2, 4)]
        public string question;

        public string[] answers = new string[4];

        [Range(0, 3)]
        public int correctAnswerIndex;

        [TextArea(1, 3)]
        public string explanation;
    }

    [Header("Questions")]
    [SerializeField] private QuizQuestion[] questions;

    [Header("UI Text")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text instructionText;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private TMP_Text feedbackText;

    [Header("UI Buttons")]
    [SerializeField] private Button[] answerButtons;

    [Header("Instruction")]
    [SerializeField] private string quizTitle = "Викторина";
    [SerializeField] private string instructionMessage = "Наведи луч контроллера на вариант ответа и нажми Trigger";

    [Header("Tour")]
    [SerializeField] private EarthTourManager tourManager;
    [SerializeField] private float delayAfterAnswer = 1.5f;
    [SerializeField] private float delayBeforeEndStage = 2f;

    [Header("Final Ship Return")]
    [SerializeField] private MainSceneEntryController entryController;
    [SerializeField] private LoadPlanetIntroButton startButton;
    [SerializeField] private bool returnToShipBeforeFinalVoice = true;
    [SerializeField] private bool hideStartButtonDuringFinalVoice = true;
    [SerializeField] private bool reloadSceneAfterFinalVoice = true;
    [SerializeField] private float delayAfterQuizResultVoice = 0.5f;
    [SerializeField] private float delayAfterReturnToShip = 0.5f;
    [SerializeField] private float delayBeforeSceneReload = 1f;

    [Header("Debug")]
    [SerializeField] private bool allowKeyboardDebug = true;

    private int currentQuestionIndex;
    private int score;
    private bool answerLocked;

    private void OnEnable()
    {
        SetupStaticTexts();
        SetupButtons();
        RestartQuiz();
    }

    private void Update()
    {
        if (!allowKeyboardDebug || answerLocked)
            return;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            SelectAnswer(0);

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            SelectAnswer(1);

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
            SelectAnswer(2);

        if (Keyboard.current.digit4Key.wasPressedThisFrame)
            SelectAnswer(3);
    }

    private void SetupStaticTexts()
    {
        if (titleText != null)
            titleText.text = quizTitle;

        if (instructionText != null)
            instructionText.text = instructionMessage;
    }

    private void SetupButtons()
    {
        if (answerButtons == null)
            return;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int answerIndex = i;

            if (answerButtons[i] == null)
                continue;

            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => SelectAnswer(answerIndex));
        }
    }

    private void RestartQuiz()
    {
        currentQuestionIndex = 0;
        score = 0;
        answerLocked = false;

        ShowCurrentQuestion();
    }

    private void ShowCurrentQuestion()
    {
        if (questions == null || questions.Length == 0)
        {
            if (questionText != null)
                questionText.text = "Вопросы не настроены";

            if (instructionText != null)
                instructionText.text = "Добавьте вопросы в EarthQuizManager";

            return;
        }

        QuizQuestion question = questions[currentQuestionIndex];

        if (questionText != null)
            questionText.text = question.question;

        if (progressText != null)
            progressText.text = $"Вопрос {currentQuestionIndex + 1}/{questions.Length}";

        if (feedbackText != null)
            feedbackText.text = "";

        if (answerButtons != null)
        {
            for (int i = 0; i < answerButtons.Length; i++)
            {
                Button button = answerButtons[i];

                if (button == null)
                    continue;

                bool hasAnswer =
                    question.answers != null &&
                    i < question.answers.Length &&
                    !string.IsNullOrWhiteSpace(question.answers[i]);

                button.gameObject.SetActive(hasAnswer);
                button.interactable = true;

                TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();

                if (buttonText != null && hasAnswer)
                    buttonText.text = $"{i + 1}. {question.answers[i]}";
            }
        }

        answerLocked = false;
    }

    public void SelectAnswer(int answerIndex)
    {
        if (answerLocked)
            return;

        if (questions == null || questions.Length == 0)
            return;

        answerLocked = true;

        QuizQuestion question = questions[currentQuestionIndex];

        bool isCorrect = answerIndex == question.correctAnswerIndex;

        if (isCorrect)
        {
            score++;

            if (feedbackText != null)
                feedbackText.text = "Верно!";
        }
        else
        {
            if (feedbackText != null)
                feedbackText.text = $"Неверно. {question.explanation}";
        }

        if (answerButtons != null)
        {
            foreach (Button button in answerButtons)
            {
                if (button != null)
                    button.interactable = false;
            }
        }

        StartCoroutine(NextQuestionRoutine());
    }

    private IEnumerator NextQuestionRoutine()
    {
        yield return new WaitForSeconds(delayAfterAnswer);

        currentQuestionIndex++;

        if (currentQuestionIndex >= questions.Length)
        {
            StartCoroutine(FinishQuizRoutine());
            yield break;
        }

        ShowCurrentQuestion();
    }

    public bool IsCurrentAnswerCorrect(int answerIndex)
    {
        if (questions == null || questions.Length == 0)
            return false;

        if (currentQuestionIndex < 0 || currentQuestionIndex >= questions.Length)
            return false;

        return answerIndex == questions[currentQuestionIndex].correctAnswerIndex;
    }

    private IEnumerator FinishQuizRoutine()
    {
        if (questionText != null)
            questionText.text = "Викторина завершена";

        if (progressText != null)
            progressText.text = "";

        if (instructionText != null)
            instructionText.text = "Спасибо за прохождение экскурсии";

        if (feedbackText != null)
            feedbackText.text = $"Результат: {score}/{questions.Length}";

        if (answerButtons != null)
        {
            foreach (Button button in answerButtons)
            {
                if (button != null)
                    button.gameObject.SetActive(false);
            }
        }

        Debug.Log($"Earth quiz completed. Score: {score}/{questions.Length}");

        if (TourVoiceManager.Instance != null)
        {
            TourVoiceManager.Instance.PlayQuizResultVoice(score, questions.Length);
            yield return TourVoiceManager.Instance.WaitUntilIdle();
        }
        else
        {
            Debug.LogWarning("EarthQuizManager: TourVoiceManager was not found. Quiz result voice was not played.");
            yield return new WaitForSeconds(delayBeforeEndStage);
        }

        if (delayAfterQuizResultVoice > 0f)
            yield return new WaitForSeconds(delayAfterQuizResultVoice);

        if (!returnToShipBeforeFinalVoice)
        {
            if (tourManager != null)
                tourManager.SetStage(EarthTourStage.End);

            yield break;
        }

        if (entryController == null)
            entryController = MainSceneEntryController.Instance;

        if (entryController == null)
        {
            Debug.LogWarning("EarthQuizManager: EntryController is not assigned. Cannot return player to ship.");

            if (tourManager != null)
                tourManager.SetStage(EarthTourStage.End);

            yield break;
        }

        entryController.StartFinalReturnToShip(
            tourManager,
            startButton,
            hideStartButtonDuringFinalVoice,
            reloadSceneAfterFinalVoice,
            delayAfterReturnToShip,
            delayBeforeSceneReload);
    }
}