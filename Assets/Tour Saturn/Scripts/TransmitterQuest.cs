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

    [Header("Objects")]
    [SerializeField] private GameObject sendButton;

    [Header("Audio")]
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound;

    public bool IsCompleted { get; private set; }

    public UnityEvent OnQuestCompleted;

    private bool signalPlayed;

    public void StartQuest()
    {
        IsCompleted = false;
        signalPlayed = false;

        sendButton.SetActive(false);

        firstLever.OnValueChanged -= CheckLevers;
        secondLever.OnValueChanged -= CheckLevers;

        firstLever.OnValueChanged += CheckLevers;
        secondLever.OnValueChanged += CheckLevers;

        CheckLevers();
    }

    private void CheckLevers()
    {
        bool isCorrect =
            firstLever.CurrentValue == firstTargetValue &&
            secondLever.CurrentValue == secondTargetValue;

        sendButton.SetActive(isCorrect);

        if (isCorrect && !signalPlayed)
        {
            signalPlayed = true;
            audioManager.Play(AudioType.SuccessSignal);
        }

        if (!isCorrect)
        {
            signalPlayed = false;
        }
    }

    public void SendData()
    {
        bool isCorrect =
            firstLever.CurrentValue == firstTargetValue &&
            secondLever.CurrentValue == secondTargetValue;

        if (!isCorrect)
            return;

        IsCompleted = true;

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
