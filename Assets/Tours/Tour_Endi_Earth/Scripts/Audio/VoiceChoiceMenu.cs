using UnityEngine;
using TMPro;

/// <summary>
/// Shows a small voice choice menu near the player.
/// Used when narrator is already speaking and a new voice line is requested.
/// </summary>
public class VoiceChoiceMenu : MonoBehaviour
{
    public enum VoiceChoiceAction
    {
        ListenCurrentThenPlayNew,
        SkipCurrentAndPlayNew,
        SpeedUpCurrentThenPlayNew
    }

    [Header("UI")]
    [SerializeField] private GameObject menuRoot;
    [SerializeField] private TMP_Text messageText;

    [Header("Follow")]
    [SerializeField] private Transform followTarget;
    [SerializeField] private Vector3 localOffset = new Vector3(-0.35f, -0.15f, 0.7f);
    [SerializeField] private bool followTargetEveryFrame = true;
    [SerializeField] private bool facePlayerCamera = true;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private TourVoiceManager voiceManager;
    private bool isVisible;

    private void Start()
    {
        voiceManager = TourVoiceManager.Instance;

        if (followTarget == null && Camera.main != null)
            followTarget = Camera.main.transform;

        Hide();
    }

    private void LateUpdate()
    {
        if (!isVisible)
            return;

        UpdateMenuPosition();
    }

    public void Show()
    {
        Show("Диктор ещё говорит.\nЧто сделать?");
    }

    public void Show(string message)
    {
        isVisible = true;

        if (menuRoot != null)
            menuRoot.SetActive(true);

        if (messageText != null)
            messageText.text = message;

        UpdateMenuPosition();

        if (showDebugLogs)
            Debug.Log("VoiceChoiceMenu: shown.");
    }

    public void Hide()
    {
        isVisible = false;

        if (menuRoot != null)
            menuRoot.SetActive(false);
    }

    public void ListenCurrentThenPlayNew()
    {
        if (voiceManager == null)
            voiceManager = TourVoiceManager.Instance;

        if (voiceManager != null)
            voiceManager.ConfirmPendingVoiceQueue();

        Hide();
    }

    public void SkipCurrentAndPlayNew()
    {
        SkipCurrentThenPlayNew();
    }

    public void SkipCurrentThenPlayNew()
    {
        if (voiceManager == null)
            voiceManager = TourVoiceManager.Instance;

        if (voiceManager != null)
            voiceManager.ConfirmPendingVoiceSkip();

        Hide();
    }

    public void SpeedUpCurrentThenPlayNew()
    {
        if (voiceManager == null)
            voiceManager = TourVoiceManager.Instance;

        if (voiceManager != null)
            voiceManager.ConfirmPendingVoiceSpeedUpThenQueue();

        Hide();
    }

    // Backward-compatible method names used by older versions of this menu.
    public void ChooseListenCurrentThenPlayNew()
    {
        ListenCurrentThenPlayNew();
    }

    public void ChooseSkipCurrentAndPlayNew()
    {
        SkipCurrentThenPlayNew();
    }

    public void ChooseSpeedUpCurrentThenPlayNew()
    {
        SpeedUpCurrentThenPlayNew();
    }

    public void HandleChoice(VoiceChoiceAction action)
    {
        switch (action)
        {
            case VoiceChoiceAction.ListenCurrentThenPlayNew:
                ListenCurrentThenPlayNew();
                break;

            case VoiceChoiceAction.SkipCurrentAndPlayNew:
                SkipCurrentThenPlayNew();
                break;

            case VoiceChoiceAction.SpeedUpCurrentThenPlayNew:
                SpeedUpCurrentThenPlayNew();
                break;
        }
    }

    private void UpdateMenuPosition()
    {
        if (followTarget == null)
            return;

        if (followTargetEveryFrame)
            transform.position = followTarget.TransformPoint(localOffset);

        if (facePlayerCamera && Camera.main != null)
        {
            transform.LookAt(Camera.main.transform);
            transform.Rotate(0f, 180f, 0f);
        }
        else
        {
            transform.rotation = followTarget.rotation;
        }
    }
}
