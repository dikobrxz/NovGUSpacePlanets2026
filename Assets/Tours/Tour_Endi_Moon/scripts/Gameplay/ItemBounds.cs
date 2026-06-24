using UnityEngine;

namespace MoonGame
{
    public class ItemBounds : MonoBehaviour
    {
        [Tooltip("Максимальное расстояние от точки возврата (м)")]
        [SerializeField] private float maxDistance = 15f;

        [Tooltip("Включить слежение сразу при старте (для лопаты и других инструментов). " +
                 "Для артефактов — выключи, слежение включится после откопки.")]
        [SerializeField] private bool trackFromStart = true;

        private Vector3 returnPosition;
        private Rigidbody rb;
        private bool isTracking;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            if (trackFromStart)
            {
                returnPosition = transform.position;
                isTracking = true;
            }
        }

        private void Update()
        {
            if (!isTracking) return;
            if (Vector3.Distance(transform.position, returnPosition) > maxDistance)
                ReturnToStart();
        }

        /// <summary>Вызывается из Artifact.Uncover() — включает слежение после откопки.</summary>
        public void UpdateReturnPosition()
        {
            returnPosition = transform.position;
            isTracking = true;
        }

        private void ReturnToStart()
        {
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            transform.position = returnPosition;
            Debug.Log($"[ItemBounds] {name} вернулся на место.");
        }
    }
}