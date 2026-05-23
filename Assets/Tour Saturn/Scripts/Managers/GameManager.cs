using UnityEngine;

public class GameManager : MonoBehaviour
{
    private StoryManager _sm;

    private void Awake()
    {
        _sm = GetComponent<StoryManager>();
    }

    private void Start()
    {
        Debug.Log("—ценарий запущен");
        _sm.StoryStart();
    }
}
