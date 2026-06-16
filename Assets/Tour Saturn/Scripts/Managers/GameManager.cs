using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private StoryManager storyManager;

    [Header("UI Objects")]
    [SerializeField] private GameObject startCanvas;

    [Header("Objects")]
    [SerializeField] private GameObject startPoint;

    private void Awake()
    {
        startCanvas.SetActive(true);

        storyManager.MovePlayer(startPoint.transform);
    }

    public void StoryStart()
    {
        Debug.Log("Scenario Start");
        audioManager.Play(AudioType.ContinueButton);
        storyManager.StoryStart();
        startCanvas.SetActive(false);
    }
}
