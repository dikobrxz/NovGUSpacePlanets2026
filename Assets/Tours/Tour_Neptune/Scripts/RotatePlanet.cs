using UnityEngine;

namespace Tour_ENDI_TourStub6
{

    public class RotatePlanet : MonoBehaviour
    {
        public float speed = 15f;

        void Update()
        {
            transform.Rotate(Vector3.up * speed * Time.deltaTime);
        }
    }

}