using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizAnswerButton : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TMP_Text answerText;

    [Header("Image")]
    [SerializeField] private Image buttonImage;
    private Color defaultColor;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound;

    private int answerIndex;
    private QuizManager quizManager;

    private void Awake()
    {
        defaultColor = buttonImage.color;
    }

    public void SetAnswer(string text, int index, QuizManager manager)
    {
        answerText.text = text;
        answerIndex = index;
        quizManager = manager;
    }

    public void SelectAnswer()
    {
        StartCoroutine(SelectRoutine());
    }

    private IEnumerator SelectRoutine()
    {
        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound);

        quizManager.ChooseAnswer(answerIndex, this);

        yield return new WaitForSeconds(0.2f);
    }

    public void SetCorrect()
    {
        buttonImage.color = Color.green;
    }

    public void SetWrong()
    {
        buttonImage.color = Color.red;
    }

    public void ResetColor()
    {
        buttonImage.color = defaultColor;
    }
}
