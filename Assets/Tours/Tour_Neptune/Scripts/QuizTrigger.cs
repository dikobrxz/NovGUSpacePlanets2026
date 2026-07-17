using UnityEngine;

namespace Tour_ENDI_TourStub6
{

    public class QuizTrigger : MonoBehaviour
    {
        public QuizManager quizManager;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                quizManager.StartQuiz();
        }
    }

}