using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class QuizManager : MonoBehaviour
{
    [SerializeField] private SceneManager _sceneManager;
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private Button[] answerButtons;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private GameObject quizPanel;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI explanationText;
    [SerializeField] private GameObject explanationBack;
    [SerializeField] private Button continueButton;
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color wrongColor = Color.red;
    [SerializeField] private Color defaultColor = Color.white;

    [Header("Audio")]
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip wrongSound;
    [SerializeField] private float volume = 0.7f;
    [SerializeField] private AudioSource finalVoiceOver; 
    
    private AudioSource audioSource;

    [Header("Data")]
    [SerializeField] private Question[] questions;
    [SerializeField] private Transform lockPoint;
    [SerializeField] private Transform playerRoot;

    private int currentIndex = 0;
    private int score = 0;
    private bool isActive = false;
    private bool waitingForContinue = false;
    private Vector3 savedPlayerPos;

    [System.Serializable]
    public class Question
    {
        public string text;
        public string[] answers;
        public int correctIndex;
        public string explanation;
    }

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f; 
        
        if (quizPanel) quizPanel.SetActive(false);
        if (resultPanel) resultPanel.SetActive(false);
        if (explanationText) explanationText.gameObject.SetActive(false);
        if (continueButton) continueButton.gameObject.SetActive(false);
    }

    public void StartQuiz()
    {
        Debug.Log("StartQuiz вызван!");
        
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] == null)
                Debug.LogError($"answerButtons[{i}] НЕ назначена!");
        }
        
        isActive = true;
        currentIndex = 0;
        score = 0;
        
        if (playerRoot != null && lockPoint != null)
        {
            savedPlayerPos = playerRoot.transform.position;
            playerRoot.transform.position = lockPoint.position;
        }
        
        if (quizPanel) 
        {
            quizPanel.SetActive(true);
            resultPanel.SetActive(false);
        }
        
        ShowQuestion();
    }

    private void ShowQuestion()
    {
        if (currentIndex >= questions.Length)
        {
            EndQuiz();
            return;
        }

        Question q = questions[currentIndex];
        if (questionText) questionText.text = $"Вопрос {currentIndex + 1}: {q.text}";

        if (explanationText != null) 
        {
            explanationBack.SetActive(false);
            explanationText.gameObject.SetActive(false);
            explanationText.text = "";
        }
        if (continueButton != null) 
        {
            continueButton.gameObject.SetActive(false);
        }
        
        waitingForContinue = false;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] == null) continue;
            
            if (i >= q.answers.Length)
            {
                answerButtons[i].gameObject.SetActive(false);
                answerButtons[i].interactable = false;
                continue;
            }
            
            answerButtons[i].gameObject.SetActive(true);
            answerButtons[i].interactable = true;
            answerButtons[i].onClick.RemoveAllListeners();
            int buttonIndex = i; 
            answerButtons[i].onClick.AddListener(() => OnAnswer(buttonIndex));

            var btnText = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (btnText) btnText.text = $"{q.answers[i]}";//$"{(char)('А' + i)}) {q.answers[i]}";

            var image = answerButtons[i].GetComponent<Image>();
            if (image != null) image.color = defaultColor;
        }
    }

    private void OnAnswer(int selectedIndex)
    {
        
        if (waitingForContinue) 
        {
            Debug.LogWarning("Ждём нажатия 'Продолжить', игнорирую");
            return;
        }
        if (currentIndex >= questions.Length) 
        {
            Debug.LogWarning("currentIndex >= questions.Length");
            return;
        }
        
        Question q = questions[currentIndex];
        
        // Проверка границ
        if (selectedIndex < 0 || selectedIndex >= q.answers.Length)
        {
            Debug.LogError($"❌ selectedIndex ({selectedIndex}) вне диапазона [0, {q.answers.Length})!");
            return;
        }
        
        if (q.correctIndex < 0 || q.correctIndex >= q.answers.Length)
        {
            Debug.LogError($"❌ correctIndex ({q.correctIndex}) вне диапазона [0, {q.answers.Length})!");
            return;
        }
        
        bool isCorrect = (selectedIndex == q.correctIndex);
        
        if (audioSource != null)
        {
            if (isCorrect && correctSound != null)
            {
                audioSource.PlayOneShot(correctSound, volume);
            }
            else if (!isCorrect && wrongSound != null)
            {
                audioSource.PlayOneShot(wrongSound, volume);
            }
        }
        
        if (isCorrect) score++;
        
        HighlightButtons(selectedIndex, q.correctIndex);
        
        if (explanationText != null)
        {
            string prefix = isCorrect ? "ВЕРНО! " : "НЕВЕРНО. ";
            explanationText.text = prefix + q.explanation;
            explanationText.color = isCorrect ? correctColor : wrongColor;
            explanationText.gameObject.SetActive(true);
            explanationBack.SetActive(true);
        }
        
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(true);
        }
        
        foreach (var btn in answerButtons)
        {
            if (btn != null) btn.interactable = false;
        }
        
        waitingForContinue = true;
    }

    public void OnContinueClicked()
    {
        if (!waitingForContinue) return;
        
        foreach (var btn in answerButtons)
        {
            if (btn == null) continue;
            var image = btn.GetComponent<Image>();
            if (image != null) image.color = defaultColor;
            btn.interactable = true;
        }
        
        if (explanationText != null) explanationText.gameObject.SetActive(false);
        if (continueButton != null) continueButton.gameObject.SetActive(false);
        
        waitingForContinue = false;
        currentIndex++;
        ShowQuestion();
    }

    private void HighlightButtons(int selectedIndex, int correctIndex)
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] == null) continue;
            
            var image = answerButtons[i].GetComponent<Image>();
            if (image == null) continue;
            
            if (i == correctIndex)
                image.color = correctColor;
            else if (i == selectedIndex && i != correctIndex)
                image.color = wrongColor;
            else
                image.color = defaultColor;
        }
    }

    private void EndQuiz()
    {
        isActive = false;
        
        string msg = score == 5 
            ? "Невероятный успех! Твои знания помогут человечеству узнать больше о Солнечной системе." 
            : score >= 3 
            ? "Спасибо, юный исследователь! Твоих знаний уже достаточно для организации новой миссии." 
            : "Спасибо, друг! Давай вернёмся к Плутону, чтобы узнать немного больше.";
        
        Debug.Log($"Финальный счёт: {score}/5");

        if (resultText != null) 
        {
            resultText.text = msg;
        }
        
        if (quizPanel != null) 
        {
            quizPanel.SetActive(false);
        }

        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
        }
        
        // Отключаем кнопки ответов, чтобы XR не пытался с ними работать
        foreach (var btn in answerButtons)
        {
            if (btn != null) 
            {
                btn.gameObject.SetActive(false);
                btn.interactable = false;
            }
        }

        // Возвращаем игрока
        //if (playerRoot != null && lockPoint != null)
        //{
        //    playerRoot.transform.position = savedPlayerPos;
        //}
        if (score >= 3)
        {
            Debug.Log("Квиз завершён!");
            if (finalVoiceOver != null && finalVoiceOver.clip != null)
            {
                Invoke(nameof(PlayFinalVoice), 1f);
            }
        }
        else 
        {
            Invoke(nameof(Restart), 4f);
        }
    }

    private void Restart()
    { 
        _sceneManager.RestartStage();
    }

    private void PlayFinalVoice()
    {
        finalVoiceOver.Play();
    }

    public bool IsRunning() => isActive;
}