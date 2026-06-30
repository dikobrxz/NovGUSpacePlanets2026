using UnityEngine;

namespace Tour_Endi_Earth
{
    public class PlacedObjectRotator : MonoBehaviour
    {
        [SerializeField] private Vector3 rotationAxis = Vector3.up;
        [SerializeField] private float rotationSpeed = 40f;

        private void Update()
        {
            transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
}