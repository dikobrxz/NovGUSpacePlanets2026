using UnityEngine;
using UnityEngine.Events;

public class TransmitterQuest : MonoBehaviour
{
    [Header("Levers")]
    [SerializeField] private RotationLever firstLever;
    [SerializeField] private RotationLever secondLever;

    [Header("Target Values")]
    [SerializeField] private int firstTargetValue = 2004;
    [SerializeField] private int secondTargetValue = 2017;

    /*[Header("Objects")]
    [SerializeField] private GameObject sendButton;*/

    [Header("Audio")]
    [SerializeField] private AudioManager audioManager;

    public bool IsCompleted { get; private set; }

    public UnityEvent OnQuestCompleted;

    private bool canSendData;
    private bool signalPlayed;

    public void StartQuest()
    {
        IsCompleted = false;
        canSendData = false;
        signalPlayed = false;

        firstLever.OnValueChanged -= CheckLevers;
        secondLever.OnValueChanged -= CheckLevers;

        firstLever.OnValueChanged += CheckLevers;
        secondLever.OnValueChanged += CheckLevers;

        CheckLevers();
    }

    private void CheckLevers()
    {
        bool canSendData =
            firstLever.CurrentValue == firstTargetValue &&
            secondLever.CurrentValue == secondTargetValue;

        if (canSendData && !signalPlayed)
        {
            signalPlayed = true;
            audioManager.Play(AudioType.SuccessSignal);
        }

        if (!canSendData)
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

        Debug.Log($"Левый: {firstLever.CurrentValue}, правый: {secondLever.CurrentValue}");

        if (!isCorrect)
        {
            Debug.Log("Передатчик ещё не настроен. Данные не отправлены.");
            return;
        }

        IsCompleted = true;

        audioManager.Play(AudioType.DataSend);

        firstLever.OnValueChanged -= CheckLevers;
        secondLever.OnValueChanged -= CheckLevers;

        OnQuestCompleted?.Invoke();

        Debug.Log("Данные отправлены на Кассини");
    }

    private void OnDisable()
    {
        firstLever.OnValueChanged -= CheckLevers;
        secondLever.OnValueChanged -= CheckLevers;
    }
}
