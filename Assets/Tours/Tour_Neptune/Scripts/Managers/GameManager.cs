using UnityEngine;

namespace Tour_ENDI_TourStub6
{

    public class GameManager : MonoBehaviour
    {
        public static bool visitedNeptune = false;

        void Start()
        {
            // StoryManager сам настраивает начальное состояние в своём Start()
            // Кнопка "Начать" вызывает StoryManager.OnStartPressed()
        }
    }

}