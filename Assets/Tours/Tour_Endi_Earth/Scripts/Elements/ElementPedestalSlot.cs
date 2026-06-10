using UnityEngine;

/// <summary>
/// Detects the correct element near the pedestal
/// and snaps it to the pedestal when the player releases it.
/// </summary>
public class ElementPedestalSlot : MonoBehaviour
{
    [Header("Slot")]
    [SerializeField] private ElementType requiredElementType;
    [SerializeField] private Transform snapPoint;

    [Header("Effects")]
    [SerializeField] private ParticleSystem idleEffect;
    [SerializeField] private ParticleSystem hoverEffect;
    [SerializeField] private ParticleSystem successEffect;

    [Header("Audio")]
    [SerializeField] private bool playElementVoiceOnComplete = true;

    [Header("Wrong Element Feedback")]
    [SerializeField] private bool playWrongVoiceOnWrongElement = true;
    [SerializeField] private float wrongVoiceCooldown = 1.5f;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    [Header("State")]
    [SerializeField] private bool isCompleted;

    public bool IsCompleted => isCompleted;
    public ElementType RequiredElementType => requiredElementType;

    private float lastWrongVoiceTime = -999f;
    private ElementItem lastWrongItem;
    private bool wrongVoicePlayedForCurrentItem;

    private void Awake()
    {
        AutoFindMissingEffects();
    }

    private void Start()
    {
        PlayLoopingEffect(idleEffect);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryHandleElement(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryHandleElement(other);
    }

    private void OnTriggerExit(Collider other)
    {
        ElementItem item = other.GetComponentInParent<ElementItem>();

        if (item == null)
            return;

        if (!isCompleted)
            StopEffect(hoverEffect);

        if (item == lastWrongItem)
        {
            lastWrongItem = null;
            wrongVoicePlayedForCurrentItem = false;
        }

        if (showDebugLogs)
            Debug.Log($"Element exited trigger: {item.name}");
    }

    private void TryHandleElement(Collider other)
    {
        if (isCompleted)
            return;

        ElementItem item = other.GetComponentInParent<ElementItem>();

        if (item == null)
            return;

        if (showDebugLogs)
            Debug.Log($"Detected element: {item.ElementType}, grabbed = {item.IsGrabbed}");

        if (item.ElementType != requiredElementType)
        {
            TryPlayWrongElementVoice(item);
            return;
        }

        PlayLoopingEffect(hoverEffect);

        if (item.IsGrabbed)
            return;

        CompleteSlot(item);
    }

    private void CompleteSlot(ElementItem item)
    {
        if (snapPoint == null)
        {
            Debug.LogError($"SnapPoint is not assigned on {gameObject.name}");
            return;
        }

        isCompleted = true;

        StopEffect(idleEffect);
        StopEffect(hoverEffect);

        item.LockOnPedestal(snapPoint);

        PlayOneShotEffect(successEffect);

        if (GameAudioManager.Instance != null)
            GameAudioManager.Instance.PlayElementPlaced();

        NotifyHintManager();
        PlayElementVoice();
        NotifyQuestManager();

        Debug.Log($"Element slot completed: {requiredElementType}");
    }

    private void TryPlayWrongElementVoice(ElementItem item)
    {
        if (!playWrongVoiceOnWrongElement)
            return;

        if (item == null)
            return;

        // Пока игрок держит предмет, не ругаемся.
        // Звук сработает только когда он отпустит неправильный элемент рядом с пьедесталом.
        if (item.IsGrabbed)
            return;

        // Чтобы один и тот же неправильный предмет не говорил "Подумай ещё" бесконечно.
        if (item == lastWrongItem && wrongVoicePlayedForCurrentItem)
            return;

        if (Time.time - lastWrongVoiceTime < wrongVoiceCooldown)
            return;

        lastWrongItem = item;
        wrongVoicePlayedForCurrentItem = true;
        lastWrongVoiceTime = Time.time;

        TourVoiceManager voiceManager = TourVoiceManager.Instance;

        if (voiceManager == null)
            voiceManager = FindObjectOfType<TourVoiceManager>();

        if (voiceManager != null)
        {
            voiceManager.PlayWrongAnswerVoice();

            if (showDebugLogs)
            {
                Debug.Log(
                    $"Wrong element voice played. Required: {requiredElementType}, got: {item.ElementType}"
                );
            }
        }
        else
        {
            Debug.LogWarning("TourVoiceManager was not found. Wrong element voice was not played.");
        }
    }

    private void NotifyHintManager()
    {
        HintManager hintManager = HintManager.Instance;

        if (hintManager == null)
            hintManager = FindObjectOfType<HintManager>();

        if (hintManager != null)
        {
            hintManager.MarkElementPlaced(requiredElementType);

            if (showDebugLogs)
                Debug.Log($"HintManager notified: {requiredElementType} placed.");
        }
        else
        {
            Debug.LogWarning($"HintManager was not found. Progress was not updated for {requiredElementType}.");
        }
    }

    private void PlayElementVoice()
    {
        if (!playElementVoiceOnComplete)
            return;

        TourVoiceManager voiceManager = TourVoiceManager.Instance;

        if (voiceManager == null)
            voiceManager = FindObjectOfType<TourVoiceManager>();

        if (voiceManager != null)
        {
            voiceManager.PlayElementVoice(requiredElementType);

            if (showDebugLogs)
                Debug.Log($"TourVoiceManager played voice for: {requiredElementType}");
        }
        else
        {
            Debug.LogWarning($"TourVoiceManager was not found. Voice was not played for {requiredElementType}.");
        }
    }

    private void NotifyQuestManager()
    {
        ElementQuestManager questManager = ElementQuestManager.Instance;

        if (questManager == null)
            questManager = FindObjectOfType<ElementQuestManager>();

        if (questManager != null)
        {
            questManager.NotifySlotCompleted(this);

            if (showDebugLogs)
                Debug.Log($"ElementQuestManager notified: {requiredElementType} completed.");
        }
        else
        {
            Debug.LogWarning($"ElementQuestManager was not found. Quiz transition check was skipped for {requiredElementType}.");
        }
    }

    private void AutoFindMissingEffects()
    {
        if (idleEffect == null)
            idleEffect = FindEffectByKeyword("Idle");

        if (hoverEffect == null)
            hoverEffect = FindEffectByKeyword("Hover");

        if (successEffect == null)
            successEffect = FindEffectByKeyword("Success");
    }

    private ParticleSystem FindEffectByKeyword(string keyword)
    {
        Transform root = transform.parent != null ? transform.parent : transform;
        ParticleSystem[] systems = root.GetComponentsInChildren<ParticleSystem>(true);

        string lowerKeyword = keyword.ToLowerInvariant();

        for (int i = 0; i < systems.Length; i++)
        {
            if (systems[i].name.ToLowerInvariant().Contains(lowerKeyword))
                return systems[i];
        }

        return null;
    }

    private void PlayLoopingEffect(ParticleSystem effect)
    {
        if (effect == null)
            return;

        effect.gameObject.SetActive(true);

        ParticleSystem[] systems = effect.GetComponentsInChildren<ParticleSystem>(true);

        for (int i = 0; i < systems.Length; i++)
        {
            systems[i].gameObject.SetActive(true);

            if (!systems[i].isPlaying)
                systems[i].Play(true);
        }
    }

    private void StopEffect(ParticleSystem effect)
    {
        if (effect == null)
            return;

        ParticleSystem[] systems = effect.GetComponentsInChildren<ParticleSystem>(true);

        for (int i = 0; i < systems.Length; i++)
            systems[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private void PlayOneShotEffect(ParticleSystem effect)
    {
        if (effect == null)
        {
            Debug.LogWarning($"{gameObject.name}: Success Effect is not assigned.");
            return;
        }

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