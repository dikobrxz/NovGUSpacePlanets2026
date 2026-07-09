using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Tour_Endi_Moon
{
    /// <summary>
    /// Ящик для артефактов. Trigger Collider у горловины.
    /// При попадании артефакта внутрь — регистрирует его в QuestManager.
    /// После укладки артефакт нельзя вытащить, взять в руки или выбить.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ArtifactBox : MonoBehaviour
    {
        // Тип → GameObject — чтобы гарантировать уникальность по типу артефакта
        private readonly Dictionary<ArtifactType, Artifact> storedArtifacts = new();

        private void Reset()
        {
            var col = GetComponent<Collider>();
            if (col != null) col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            var artifact = other.GetComponentInParent<Artifact>();
            if (artifact == null || !artifact.IsUncovered) return;

            // Игнорируем уже сохранённые артефакты (попали в триггер повторно)
            if (artifact.IsStored) return;

            // Проверяем, не в ящике ли уже этот тип
            if (storedArtifacts.ContainsKey(artifact.type))
            {
                Debug.LogWarning($"[ArtifactBox] {artifact.type} уже в ящике — игнорируем.");
                return;
            }

            storedArtifacts[artifact.type] = artifact;
            Debug.Log($"[ArtifactBox] Принят: {artifact.type}. В ящике: {storedArtifacts.Count}/{QuestManager.TotalArtifacts}");

            LockArtifact(artifact);

            if (GameManager.Instance != null && GameManager.Instance.Quest != null)
                GameManager.Instance.Quest.RegisterStored(artifact.type);
        }

        /// <summary>
        /// Прикрепляет артефакт к ящику и полностью отключает взаимодействие с ним.
        /// Артефакт нельзя схватить, выбить или вытолкнуть.
        /// </summary>
        private void LockArtifact(Artifact artifact)
        {
            // Помечаем как сохранённый — блокирует повторную откопку и попадание в триггер
            artifact.MarkStored();

            var grab = artifact.GetComponent<XRGrabInteractable>();
            if (grab != null)
            {
                // retainTransformParent=true заставляет XR Toolkit восстанавливать
                // оригинального родителя при отпускании/деактивации. Сбрасываем ДО SetParent.
                grab.retainTransformParent = false;
                grab.enabled = false;
            }

            // Останавливаем физику до смены родителя — иначе Rigidbody может дёрнуться
            var rb = artifact.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            // Прикрепляем к ящику, сохраняя мировую позицию
            artifact.transform.SetParent(transform, worldPositionStays: true);

            // Отключаем коллайдеры артефакта, чтобы ничто не могло его вытолкнуть
            foreach (var col in artifact.GetComponentsInChildren<Collider>())
                col.enabled = false;
        }
    }
}
