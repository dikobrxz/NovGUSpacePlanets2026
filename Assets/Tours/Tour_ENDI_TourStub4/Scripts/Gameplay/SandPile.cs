using System.Collections;
using UnityEngine;

namespace Tour_ENDI_TourStub4
{
    /// <summary>
    /// Кучка земли, которая погружается в грунт при каждом ударе лопатой.
    /// После достаточного числа ударов открывает вложенный артефакт.
    /// Если артефакт не задан — кучка просто уходит вниз (пустая яма).
    /// </summary>
    public class SandPile : MonoBehaviour
    {
        [Header("Артефакт внутри кучки (null = пустая яма)")]
        [SerializeField] private Artifact hiddenArtifact;

        [Header("Сколько ударов лопатой нужно")]
        [SerializeField] private int requiredHits = 6;

        [Header("На сколько кучка опускается за удар (м)")]
        [SerializeField] private float sinkPerHit = 0.04f;

        [Header("Скорость погружения (м/с)")]
        [SerializeField] private float sinkSpeed = 0.3f;

        [Header("Задержка после последнего удара до раскрытия артефакта (сек)")]
        [SerializeField] private float revealDelay = 0.4f;

        private int currentHits;
        private bool isDug;
        private Vector3 targetPosition;
        private bool isSinking;

        public bool IsDug => isDug;

        private void Awake()
        {
            targetPosition = transform.position;

            // Прячем артефакт под землёй до момента откопки
            if (hiddenArtifact != null)
                hiddenArtifact.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!isSinking) return;

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                sinkSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
            {
                transform.position = targetPosition;
                isSinking = false;
            }
        }

        /// <summary>Вызывается из DiggingTool при попадании лопатой.</summary>
        public void RegisterHit()
        {
            if (isDug) return;

            currentHits++;
            targetPosition -= Vector3.up * sinkPerHit;
            isSinking = true;

            Debug.Log($"[SandPile] {name}: удар {currentHits}/{requiredHits}");

            if (currentHits >= requiredHits)
                StartCoroutine(RevealArtifact());
        }

        private IEnumerator RevealArtifact()
        {
            isDug = true;
            yield return new WaitForSeconds(revealDelay);

            if (hiddenArtifact != null)
            {
                // Артефакт появляется чуть выше поверхности кучки
                Vector3 spawnPos = transform.position + Vector3.up * 0.05f;
                hiddenArtifact.Uncover(spawnPos);
            }

            // Кучка исчезает
            gameObject.SetActive(false);
        }
    }
}
