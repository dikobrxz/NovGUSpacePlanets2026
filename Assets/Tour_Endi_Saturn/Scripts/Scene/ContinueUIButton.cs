using TMPro;
using UnityEngine;
namespace Tour_Endi_Saturn
{

public class ContinueUIButton : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TMP_Text buttonText;

    [Header("Audio")]
    [SerializeField] private AudioManager audioManager;

    private ContinueUIManager continueUIManager;

    public void SetButton(string text, ContinueUIManager manager)
    {
        buttonText.text = text;
        continueUIManager = manager;
    }

    public void SelectButton()
    {
        audioManager.Play(AudioType.ContinueButton);
        continueUIManager.PressContinue();
    }
}
}
