using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadNeptuneScene()
    {
        PlayerPrefs.SetInt("visited", 1);
        SceneManager.LoadScene("Neptune");
    }

    public void LoadStartScene()
    {
        SceneManager.LoadScene("StartScene");
    }
}