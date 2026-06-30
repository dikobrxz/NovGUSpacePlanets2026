using System.Collections;
using UnityEngine;

namespace Tour_Endi_Earth
{
    /// <summary>
    /// Tracks all elemental pedestal slots and moves the tour to the quiz stage
    /// when every element is placed correctly.
    /// This version waits for the narrator queue before starting the quiz.
    /// </summary>
    public class ElementQuestManager : MonoBehaviour
    {
        public static ElementQuestManager Instance { get; private set; }

        [Header("Quest")]
        [SerializeField] private ElementPedestalSlot[] slots;
        [SerializeField] private bool autoFindSlotsIfMissing = true;

        [Header("Tour")]
        [SerializeField] private EarthTourManager tourManager;

        [Header("Quiz Transition")]
        [SerializeField] private float minimumDelayBeforeNextStage = 2f;
        [SerializeField] private bool waitForNarratorBeforeQuiz = true;
        [SerializeField] private float extraDelayAfterNarrator = 0.5f;
        [SerializeField] private string completedHintMessage = "Все элементы установлены. Повернитесь к викторине.";

        [Header("Optional Effects")]
        [SerializeField] private ParticleSystem questCompletedEffect;
        [SerializeField] private AudioClip questCompletedClip;

        [Header("Debug")]
        [SerializeField] private bool showDebugLogs = true;

        private bool questCompleted;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Debug.LogWarning("ElementQuestManager: another instance already exists. Check duplicates in the scene.");

            Instance = this;
        }

        private void Start()
        {
            AutoFindSlotsIfNeeded();
        }

        private void Update()
        {
            if (questCompleted)
                return;

            if (tourManager == null)
                return;

            if (tourManager.CurrentStage != EarthTourStage.MatchingQuest)
                return;

            TryCompleteQuest();
        }

        public void NotifySlotCompleted(ElementPedestalSlot completedSlot)
        {
            if (showDebugLogs && completedSlot != null)
                Debug.Log($"ElementQuestManager notified by slot: {completedSlot.name}");

            TryCompleteQuest();
        }

        [ContextMenu("Refresh Slots From Scene")]
        private void RefreshSlotsFromSceneContext()
        {
            slots = FindObjectsByType<ElementPedestalSlot>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            Debug.Log($"ElementQuestManager: found {slots.Length} slots in scene.");
        }

        [ContextMenu("Check Quest Completion")]
        private void CheckQuestCompletionContext()
        {
            TryCompleteQuest();
        }

        private void TryCompleteQuest()
        {
            if (questCompleted)
                return;

            AutoFindSlotsIfNeeded();

            if (!AreAllSlotsCompleted())
                return;

            questCompleted = true;
            StartCoroutine(CompleteQuestRoutine());
        }

        private IEnumerator CompleteQuestRoutine()
        {
            if (showDebugLogs)
                Debug.Log("All elemental slots completed. Quiz will start after narrator finishes.");

            ShowCompletedHint();
            PlayEffect(questCompletedEffect);

            if (TourVoiceManager.Instance != null && questCompletedClip != null)
                TourVoiceManager.Instance.EnqueueVoice(questCompletedClip, "element quest completed");

            yield return new WaitForSeconds(minimumDelayBeforeNextStage);

            if (waitForNarratorBeforeQuiz && TourVoiceManager.Instance != null)
            {
                yield return TourVoiceManager.Instance.WaitUntilIdle();
                yield return new WaitForSeconds(extraDelayAfterNarrator);
            }

            if (tourManager != null)
                tourManager.SetStage(EarthTourStage.Quiz);
            else
                Debug.LogWarning("ElementQuestManager: TourManager is not assigned.");
        }

        private void AutoFindSlotsIfNeeded()
        {
            if (!autoFindSlotsIfMissing)
                return;

            if (slots != null && slots.Length > 0 && !HasEmptySlotReferences())
                return;

            ElementPedestalSlot[] foundSlots = FindObjectsByType<ElementPedestalSlot>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            if (foundSlots == null || foundSlots.Length == 0)
            {
                if (showDebugLogs)
                    Debug.LogWarning("ElementQuestManager: no ElementPedestalSlot objects found in scene.");

                return;
            }

            slots = foundSlots;

            if (showDebugLogs)
                Debug.Log($"ElementQuestManager: auto-found {slots.Length} pedestal slots.");
        }

        private bool HasEmptySlotReferences()
        {
            if (slots == null || slots.Length == 0)
                return true;

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == null)
                    return true;
            }

            return false;
        }

        private bool AreAllSlotsCompleted()
        {
            if (slots == null || slots.Length == 0)
                return false;

            for (int i = 0; i < slots.Length; i++)
            {
                ElementPedestalSlot slot = slots[i];

                if (slot == null)
                {
                    if (showDebugLogs)
                        Debug.LogWarning($"ElementQuestManager: slot {i} is not assigned.");

                    return false;
                }

                if (!slot.IsCompleted)
                    return false;
            }

            return true;
        }

        private void ShowCompletedHint()
        {
            if (string.IsNullOrWhiteSpace(completedHintMessage))
                return;

            HintManager hintManager = HintManager.Instance;

            if (hintManager == null)
                hintManager = FindFirstObjectByType<HintManager>();

            if (hintManager != null)
                hintManager.ShowCustomMessage(completedHintMessage);
        }

        private void PlayEffect(ParticleSystem effect)
        {
            if (effect == null)
                return;

            effect.gameObject.SetActive(true);

            ParticleSystem[] systems = effect.GetComponentsInChildren<ParticleSystem>(true);

            for (int i = 0; i < systems.Length; i++)
            {
                systems[i].gameObject.SetActive(true);
                systems[i].Clear(true);
                systems[i].Play(true);
            }
        }
    }
}