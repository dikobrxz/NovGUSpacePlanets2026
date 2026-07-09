using UnityEngine;

namespace Tour_ENDI_TourStub4
{
    /// <summary>
    /// Управляет доступностью лопаты.
    /// Лопата недоступна до перехода в GameState.Exploration —
    /// чтобы игрок не мог взять её во время вступительной озвучки.
    /// </summary>
    public class ToolsController : MonoBehaviour
    {
        [Header("Лопата")]
        [SerializeField] private GameObject shovel;

        private StoryManager storyManager;

        private void Start()
        {
            storyManager = GameManager.Instance != null
                ? GameManager.Instance.Story
                : FindFirstObjectByType<StoryManager>();

            SetShovelActive(false);

            if (storyManager == null)
            {
                Debug.LogError("[ToolsController] StoryManager не найден.");
                return;
            }

            storyManager.OnStateChanged += HandleStateChanged;
        }

        private void OnDestroy()
        {
            if (storyManager != null)
                storyManager.OnStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState state)
        {
            if (state == GameState.Exploration)
                SetShovelActive(true);
            else if (state == GameState.Return || state == GameState.Quiz || state == GameState.End)
                SetShovelActive(false);
        }

        /// <summary>Показывает или скрывает лопату.</summary>
        private void SetShovelActive(bool active)
        {
            if (shovel != null) shovel.SetActive(active);
        }
    }
}
