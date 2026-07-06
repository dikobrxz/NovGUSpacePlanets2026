using UnityEngine;
using TMPro;

public class CoordinateDevice : MonoBehaviour
{
    [Header("UI")]
    public Transform needle;
    public TMP_Text displayText;
    public TMP_Text statusText;

    [Header("Звуки")]
    public AudioSource deviceAudio;
    public AudioClip clickSound;
    public AudioClip successSound;

    private int[] targetCode = { 4, 7, 2 };
    private int[] currentCode = { 0, 0, 0 };
    private int activeSlot = 0;
    private bool missionComplete = false;
    private bool interactionEnabled = false;

    void Start()
    {
        UpdateDisplay();
        if (statusText != null)
            statusText.text = "";
    }

    // Вызывается из StoryManager после clip4
    public void EnableInteraction()
    {
        interactionEnabled = true;
        if (statusText != null)
            statusText.text = "Введите код: 4 - 7 - 2";
        UpdateDisplay();
    }

    public void IncreaseValue()
    {
        if (missionComplete || !interactionEnabled) return;
        currentCode[activeSlot] = (currentCode[activeSlot] + 1) % 10;
        PlayClick();
        UpdateNeedle();
        UpdateDisplay();
    }

    public void DecreaseValue()
    {
        if (missionComplete || !interactionEnabled) return;
        currentCode[activeSlot] = (currentCode[activeSlot] + 9) % 10;
        PlayClick();
        UpdateNeedle();
        UpdateDisplay();
    }

    public void ToggleMode()
    {
        if (missionComplete || !interactionEnabled) return;
        activeSlot = (activeSlot + 1) % 3;
        PlayClick();
        UpdateNeedle();
        UpdateDisplay();
    }

    public void SendCoordinates()
    {
        if (missionComplete || !interactionEnabled) return;

        if (currentCode[0] == targetCode[0] &&
            currentCode[1] == targetCode[1] &&
            currentCode[2] == targetCode[2])
        {
            missionComplete = true;
            if (statusText != null)
                statusText.text = "Код верный! Сигнал отправлен!";
            if (displayText != null)
                displayText.text = "4  7  2\n>>> ОТПРАВЛЕНО <<<";
            if (deviceAudio != null && successSound != null)
                deviceAudio.PlayOneShot(successSound);

            StoryManager sm = FindFirstObjectByType<StoryManager>();
            if (sm != null)
                sm.OnMissionComplete();
        }
        else
        {
            if (statusText != null)
                statusText.text = "Неверный код! Попробуй ещё";
            if (deviceAudio != null && clickSound != null)
                deviceAudio.PlayOneShot(clickSound);
        }
    }

    void UpdateDisplay()
    {
        if (displayText != null)
        {
            string line = "";
            for (int i = 0; i < 3; i++)
            {
                if (i == activeSlot)
                    line += "[" + currentCode[i] + "]";
                else
                    line += " " + currentCode[i] + " ";
                if (i < 2) line += "  ";
            }
            displayText.text = line + "\nНажми экран для отправки";
        }
    }

    void UpdateNeedle()
    {
        if (needle != null)
        {
            float angle = currentCode[activeSlot] * 36f;
            needle.localRotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void PlayClick()
    {
        if (deviceAudio != null && clickSound != null)
            deviceAudio.PlayOneShot(clickSound);
    }
}