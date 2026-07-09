using System.Collections;
using UnityEngine;

namespace Tour_Endi_Moon
{
    /// <summary>
    /// Зона возврата возле корабля. Когда игрок входит в неё после сбора артефактов —
    /// сценарий переходит к квизу.
    /// Поставить пустой объект с большим Collider (Trigger) у стартовой площадки.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ReturnZone : MonoBehaviour
    {
        [Tooltip("Тег игрока (на XR Origin или его коллайдере)")]
        [SerializeField] private string playerTag = "Player";

        [Tooltip("Задержка активации зоны после телепорта (сек)")]
        [SerializeField] private float activationDelay = 2f;

        private bool triggered;
        private bool ready;

        private void Reset()
        {
            var col = GetComponent<Collider>();
            if (col != null) col.isTrigger = true;
        }

        private void OnEnable()
        {
            triggered = false;
            ready = false;
        }

        private void Start()
        {
            var story = GameManager.Instance != null ? GameManager.Instance.Story : null;
            if (story != null)
                story.OnStateChanged += OnStateChanged;
        }

        private void OnDestroy()
        {
            var story = GameManager.Instance != null ? GameManager.Instance.Story : null;
            if (story != null)
                story.OnStateChanged -= OnStateChanged;
        }

        private void OnStateChanged(GameState state)
        {
            if (state == GameState.Return)
                StartCoroutine(ActivateAfterDelay());
        }

        private IEnumerator ActivateAfterDelay()
        {
            ready = false;
            yield return new WaitForSeconds(activationDelay);
            ready = true;
            Debug.Log("[ReturnZone] Зона активна — ждём игрока.");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (triggered || !ready) return;
            if (!other.CompareTag(playerTag)) return;

            var story = GameManager.Instance != null ? GameManager.Instance.Story : null;
            if (story == null) return;

            if (story.GetCurrentStage() == GameState.Return)
            {
                triggered = true;
                story.SetStage(GameState.Quiz);
            }
        }
    }
}