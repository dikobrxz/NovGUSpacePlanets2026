using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tour_ENDI_TourStub6
{

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

}