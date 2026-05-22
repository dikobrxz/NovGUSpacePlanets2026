using TMPro;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    [Header("Hint UI")]
    [SerializeField] private GameObject hintPanel;
    [SerializeField] private TMP_Text hintText;

    private void Start()
    {
        HideHint();
    }

    public void ShowHint(string text)
    {
        hintText.text = text;
        hintPanel.SetActive(true);
    }

    public void HideHint()
    {
        hintPanel.SetActive(false);
    }
}
