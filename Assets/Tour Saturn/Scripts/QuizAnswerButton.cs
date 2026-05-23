using TMPro;
using UnityEngine;
using System.Collections;

public class QuizAnswerButton : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TMP_Text answerText;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound;

    private int answerIndex;
    private QuizManager quizManager;

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

        yield return new WaitForSeconds(0.2f);

        quizManager.ChooseAnswer(answerIndex);
    }
}
