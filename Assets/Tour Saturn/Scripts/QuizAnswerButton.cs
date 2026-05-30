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
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip wrongSound;

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
        quizManager.ChooseAnswer(answerIndex, this);
    }

    public void PlayAnswerSound(bool isCorrect)
    {
        if (audioSource == null)
            return;

        AudioClip clip = isCorrect ? correctSound : wrongSound;

        if (clip != null)
            audioSource.PlayOneShot(clip);
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
