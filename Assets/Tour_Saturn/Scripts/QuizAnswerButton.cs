using TMPro;
using UnityEngine;

public class QuizAnswerButton : MonoBehaviour
{
    [SerializeField] private TMP_Text answerText;

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
        quizManager.ChooseAnswer(answerIndex);
    }
}
