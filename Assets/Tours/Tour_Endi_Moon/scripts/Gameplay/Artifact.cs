using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace MoonGame
{
    [RequireComponent(typeof(Collider))]
    public class Artifact : MonoBehaviour
    {
        [Header("Тип артефакта")]
        public ArtifactType type;

        [Header("Подъём после откопки")]
        [SerializeField] private float riseHeight = 0.15f;
        [SerializeField] private float riseDuration = 0.6f;

        [Header("XR Grab Interactable (будет активирован после откопки)")]
        [SerializeField] private XRGrabInteractable grabInteractable;

        private Rigidbody rb;

        public bool IsUncovered { get; private set; }
        public bool IsStored { get; private set; }

        private Vector3 riseStartPos;
        private Vector3 riseEndPos;
        private float riseT;
        private bool rising;

        private void Awake()
        {
            if (grabInteractable == null) grabInteractable = GetComponent<XRGrabInteractable>();
            if (grabInteractable != null)
            {
                grabInteractable.enabled = false;
                grabInteractable.selectEntered.AddListener(_ => CancelRise());
            }

            rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Гравитация выключена до откопки — чтобы не падал под землю
                // isKinematic НЕ ставим — иначе XR Grab не сможет притянуть к руке
                rb.useGravity = false;
                rb.isKinematic = false;
            }
        }

        private void Update()
        {
            if (!rising) return;

            riseT += Time.deltaTime / riseDuration;
            transform.position = Vector3.Lerp(riseStartPos, riseEndPos, Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(riseT)));

            if (riseT >= 1f)
            {
                rising = false;
                EnablePhysics();
                if (grabInteractable != null) grabInteractable.enabled = true;
                Debug.Log($"[Artifact:{type}] готов к подъёму игроком.");
            }
        }

        private void CancelRise()
        {
            if (!rising) return;
            rising = false;
            EnablePhysics();
        }

        public void Uncover(Vector3 spawnPosition)
        {
            if (IsUncovered || IsStored) return;
            IsUncovered = true;

            gameObject.SetActive(true);
            GetComponent<ItemBounds>()?.UpdateReturnPosition();
            transform.position = spawnPosition;

            riseStartPos = spawnPosition;
            riseEndPos = spawnPosition + Vector3.up * riseHeight;
            riseT = 0f;
            rising = true;

            Debug.Log($"[Artifact:{type}] откопан на позиции {spawnPosition}.");

            if (GameManager.Instance?.Audio != null)
                GameManager.Instance.Audio.PlayArtifactFoundClip(type);

            if (GameManager.Instance?.Quest != null)
                GameManager.Instance.Quest.RegisterUncovered(type);

            
        }

        public void MarkStored()
        {
            IsStored = true;
            IsUncovered = true;
        }

        private void EnablePhysics()
        {
            if (rb == null) return;
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }
}
