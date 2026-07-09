using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

namespace Tour_ENDI_TourStub3
{

    public class QuizManager : MonoBehaviour
    {
        [Header("UI")]
        public TMP_Text questionText;
        public Button[] answerButtons;
        public Button actionButton;
        public CanvasGroup quizCanvasGroup;
        public TMP_Text finalMessageText;
        public TMP_Text resultText;

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

        [Header("Button Labels")]
        [SerializeField] private string restartButtonLabel;
        [SerializeField] private string endButtonLabel;

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
                questionText = firstQuestionText,
                answers = new[] { firstQuestionFirstAnswerText, firstQuestionSecondAnswerText, firstQuestionThirdAnswerText, "" },
                correctAnswerIndex = 2,
                numberOfAnswers = 3
            });

            questions.Add(new Question
            {
                questionText = secondQuestionText,
                answers = new[] { secondQuestionFirstAnswerText, secondQuestionSecondAnswerText, secondQuestionThirdAnswerText, secondQuestionFourthAnswerText },
                correctAnswerIndex = 2,
                numberOfAnswers = 4
            });

            questions.Add(new Question
            {
                questionText = thirdQuestionText,
                answers = new[] { thirdQuestionFirstAnswerText, thirdQuestionSecondAnswerText, thirdQuestionThirdAnswerText, thirdQuestionFourthAnswerText },
                correctAnswerIndex = 0,
                numberOfAnswers = 4
            });

            questions.Add(new Question
            {
                questionText = fourthQuestionText,
                answers = new[] { fourthQuestionFirstAnswerText, fourthQuestionSecondAnswerText, "", "" },
                correctAnswerIndex = 1,
                numberOfAnswers = 2
            });

            questions.Add(new Question
            {
                questionText = fifthQuestionText,
                answers = new[] { fifthQuestionFirstAnswerText, fifthQuestionSecondAnswerText, fifthQuestionThirdAnswerText, fifthQuestionFourthAnswerText },
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
            resultText.gameObject.SetActive(false);

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

            resultText.text = $"Правильно {correctCount} из 5";
            resultText.gameObject.SetActive(true);

            if (lowScore)
            {
                SetupActionButton(restartButtonLabel);
                finalMessageText.gameObject.SetActive(false);
            }
            else
            {
                actionButton.gameObject.SetActive(false);
                finalMessageText.text = endButtonLabel;
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
            label.text = text;

            actionButton.gameObject.SetActive(true);
        }

        private IEnumerator PlayResultSequence(AudioClip resultClip, bool playFinal)
        {
            waitingAudio = true;

            quizAudioSource.Stop();
            quizAudioSource.clip = resultClip;
            quizAudioSource.Play();
            yield return new WaitForSeconds(resultClip.length);

            if (playFinal)
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
            quizAudioSource.PlayOneShot(clip);
        }

        private void OnActionButtonPressed()
        {
            TMP_Text label = actionButton.GetComponentInChildren<TMP_Text>();
            if (label == null) return;

            if (label.text == restartButtonLabel)
            {
                resultText.gameObject.SetActive(false);
                quizCanvasGroup.alpha = 0f;
                quizCanvasGroup.interactable = false;
                quizCanvasGroup.blocksRaycasts = false;
                finalMessageText.gameObject.SetActive(false);

                stageVisuals?.RestartScenario();
            }
        }
    }

}