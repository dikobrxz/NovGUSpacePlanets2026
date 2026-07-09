using UnityEngine;

namespace Tour_Endi_Moon
{
    /// <summary>
    /// Вешается на объект-платформу (стартовая площадка).
    /// Когда все артефакты в ящике (GameState == Return) и камера игрока
    /// оказывается в радиусе платформы — инициирует fade-телепорт на Spawn 2.
    ///
    /// Использует расстояние до камеры вместо OnTriggerEnter, так как XR Origin
    /// не имеет Collider, который Unity мог бы зарегистрировать в триггере.
    /// </summary>
    public class ReturnPlatform : MonoBehaviour
    {
        [Tooltip("Радиус активации платформы (метры)")]
        [SerializeField] private float activationRadius = 2.5f;

        private bool triggered;
        private StoryManager story;
        private TeleportController teleport;
        private Transform playerCamera;

        private void Start()
        {
            story = GameManager.Instance != null
                ? GameManager.Instance.Story
                : FindFirstObjectByType<StoryManager>();

            teleport = FindFirstObjectByType<TeleportController>();

            // Ищем камеру игрока через XROrigin
            var origin = FindFirstObjectByType<Unity.XR.CoreUtils.XROrigin>();
            if (origin != null && origin.Camera != null)
                playerCamera = origin.Camera.transform;

            // Подписываемся на смену стадии для сброса флага
            if (story != null)
                story.OnStateChanged += OnStateChanged;
        }

        private void OnDestroy()
        {
            if (story != null)
                story.OnStateChanged -= OnStateChanged;
        }

        private void OnStateChanged(GameState state)
        {
            // При переходе в Return сбрасываем флаг — платформа снова активна
            if (state == GameState.Return)
            {
                triggered = false;
                Debug.Log("[ReturnPlatform] Готова к срабатыванию — ждём игрока.");
            }
        }

        private void Update()
        {
            if (triggered) return;
            if (story == null || story.GetCurrentStage() != GameState.Return) return;
            if (teleport == null || playerCamera == null) return;

            // Проверяем только горизонтальное расстояние — высота не важна
            float dist = Vector3.Distance(
                new Vector3(playerCamera.position.x, transform.position.y, playerCamera.position.z),
                transform.position
            );

            if (dist <= activationRadius)
            {
                triggered = true;
                Debug.Log($"[ReturnPlatform] Игрок в радиусе {dist:F2}м — запускаем телепорт на Spawn 2.");
                teleport.TriggerReturnTeleport();
            }
        }

        /// <summary>Ручной сброс флага.</summary>
        public void ResetTrigger() => triggered = false;
    }
}
