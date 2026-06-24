using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MoonGame
{
    /// <summary>
    /// World-Space экран старта с кнопкой «Начать».
    /// Показывается при запуске сцены. По нажатию кнопки запускает сценарий.
    /// Кнопка нажимается лучом XR-контроллера (как в QuizUI).
    /// </summary>
    public class StartScreenUI : MonoBehaviour
    {
        [Header("Корневой Canvas экрана старта")]
        [SerializeField] private GameObject root;

        [Header("Кнопка «Начать»")]
        [SerializeField] private Button startButton;

        [Header("Текст заголовка (необязательно)")]
        [SerializeField] private TMP_Text titleText;

        private void Awake()
        {
            if (startButton != null)
                startButton.onClick.AddListener(OnStartClicked);
        }

        private void OnDestroy()
        {
            if (startButton != null)
                startButton.onClick.RemoveListener(OnStartClicked);
        }

        public void Show()
        {
            if (root != null) root.SetActive(true);
        }

        public void Hide()
        {
            if (root != null) root.SetActive(false);
        }

        private void OnStartClicked()
        {
            Debug.Log("[StartScreenUI] Кнопка «Начать» нажата.");
            Hide();

            // Запускаем сценарий через GameManager
            if (GameManager.Instance != null)
                GameManager.Instance.BeginStory();
        }
    }
}
