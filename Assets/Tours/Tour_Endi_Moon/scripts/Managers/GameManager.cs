using UnityEngine;

namespace MoonGame
{
    /// <summary>
    /// Главный менеджер приложения. Singleton, точка входа.
    /// Сценарий НЕ запускается автоматически — ждёт нажатия кнопки «Начать»
    /// на StartScreenUI, которая вызывает BeginStory().
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Ссылки на менеджеры")]
        [SerializeField] private StoryManager storyManager;
        [SerializeField] private AudioManager audioManager;
        [SerializeField] private UIManager uiManager;
        [SerializeField] private QuestManager questManager;
        [SerializeField] private QuizManager quizManager;

        [Header("Экран старта")]
        [SerializeField] private StartScreenUI startScreen;

        public StoryManager Story => storyManager;
        public AudioManager Audio => audioManager;
        public UIManager UI => uiManager;
        public QuestManager Quest => questManager;
        public QuizManager Quiz => quizManager;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (storyManager == null) storyManager = GetComponent<StoryManager>();
            if (audioManager == null) audioManager = GetComponent<AudioManager>();
            if (uiManager == null) uiManager = GetComponent<UIManager>();
            if (questManager == null) questManager = GetComponent<QuestManager>();
            if (quizManager == null) quizManager = GetComponent<QuizManager>();
        }

        private void Start()
        {
            Debug.Log("[GameManager] Игра запущена. Ожидание кнопки «Начать».");

            // Показываем экран старта, сценарий ждёт нажатия кнопки
            if (startScreen != null)
                startScreen.Show();
            else
            {
                // Если экран старта не назначен — запускаем сразу (фолбэк)
                Debug.LogWarning("[GameManager] StartScreenUI не назначен — запускаю сценарий сразу.");
                BeginStory();
            }
        }

        /// <summary>Запускает сценарий с этапа Intro. Вызывается кнопкой «Начать».</summary>
        public void BeginStory()
        {
            if (storyManager == null)
            {
                Debug.LogError("[GameManager] StoryManager не найден! Сценарий не будет запущен.");
                return;
            }

            Debug.Log("[GameManager] Запуск сценария.");
            storyManager.SetStageForce(GameState.Intro);
        }
    }
}
