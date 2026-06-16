using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class TransmitterQuest : MonoBehaviour
{
    [Header("Levers")]
    [SerializeField] private RotationLever firstLever;
    [SerializeField] private RotationLever secondLever;

    [Header("Target Values")]
    [SerializeField] private int firstTargetValue = 2004;
    [SerializeField] private int secondTargetValue = 2017;

    [Header("Audio")]
    [SerializeField] private AudioManager audioManager;

    [Header("Hint UI")]
    [SerializeField] private GameObject hintPanel;
    [SerializeField] private TMP_Text hintText;

    [Header("Hint Texts")]
    [TextArea]
    [SerializeField] private string rotateHint;
    [TextArea]
    [SerializeField] private string sendHint;

    public bool IsCompleted { get; private set; }

    public UnityEvent OnQuestCompleted;

    private bool signalPlayed;

    public void StartQuest()
    {
        IsCompleted = false;
        signalPlayed = false;

        firstLever.OnValueChanged -= CheckLevers;
        secondLever.OnValueChanged -= CheckLevers;

        firstLever.OnValueChanged += CheckLevers;
        secondLever.OnValueChanged += CheckLevers;

        ShowHint(rotateHint);
    }

    private void CheckLevers()
    {
        bool isCorrect =
        firstLever.CurrentValue == firstTargetValue &&
        secondLever.CurrentValue == secondTargetValue;

        if (isCorrect)
        {
            ShowHint(sendHint);
        }

        if (isCorrect && !signalPlayed)
        {
            signalPlayed = true;
            audioManager.Play(AudioType.SuccessSignal);
            Debug.Log("Transmitter configured. Data can be sent");
        }

        if (!isCorrect)
        {
            signalPlayed = false;
        }
    }

    public void SendData()
    {
        audioManager.Play(AudioType.ButtonPress);

        bool isCorrect =
        firstLever.CurrentValue == firstTargetValue &&
        secondLever.CurrentValue == secondTargetValue;

        Debug.Log($"Left lever: {firstLever.CurrentValue}, Right lever: {secondLever.CurrentValue}");

        if (!isCorrect)
        {
            Debug.Log("Transmitter isn't configured yet. Data not sent.");
            return;
        }

        HideHint();

        IsCompleted = true;

        audioManager.Play(AudioType.DataSend);

        firstLever.OnValueChanged -= CheckLevers;
        secondLever.OnValueChanged -= CheckLevers;

        OnQuestCompleted?.Invoke();

        Debug.Log("Data sent to Cassini");
    }

    private void ShowHint(string text)
    {
        hintText.text = text;
        hintPanel.SetActive(true);
    }

    private void HideHint()
    {
        hintPanel.SetActive(false);
    }

    private void OnDisable()
    {
        firstLever.OnValueChanged -= CheckLevers;
        secondLever.OnValueChanged -= CheckLevers;
    }
}
