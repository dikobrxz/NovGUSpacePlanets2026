using UnityEngine;
using TMPro;
namespace Tour_Endi_Saturn
{

public class UIManager : MonoBehaviour
{
    [Header("Subtitle UI")]
    [SerializeField] private GameObject subtitlePanel;
    [SerializeField] private TMP_Text subtitleText;

    [Header("Lever Value UI")]
    [SerializeField] private GameObject firstLeverPanel;
    [SerializeField] private TMP_Text firstLeverText;

    [SerializeField] private GameObject secondLeverPanel;
    [SerializeField] private TMP_Text secondLeverText;

    private void Start()
    {
        HideSubtitles();
    }

    public void ShowSubtitles(string text)
    {
        subtitleText.text = text;
        subtitlePanel.SetActive(true);
    }

    public void ShowHint(string text)
    {
        subtitleText.text = text;
        subtitlePanel.SetActive(true);
    }

    public void HideTextPanel()
    {
        subtitlePanel.SetActive(false);
    }

    public void HideSubtitles()
    {
        subtitlePanel.SetActive(false);
    }

    public void ShowFirstLeverValue(int value)
    {
        firstLeverText.text = value.ToString();
        firstLeverPanel.SetActive(true);
    }

    public void ShowSecondLeverValue(int value)
    {
        secondLeverText.text = value.ToString();
        secondLeverPanel.SetActive(true);
    }
}
}
