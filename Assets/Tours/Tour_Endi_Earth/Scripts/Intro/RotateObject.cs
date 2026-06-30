using UnityEngine;

namespace Tour_Endi_Earth
{
    /// <summary>
    /// Simple object rotation for intro objects, such as the Earth globe.
    /// </summary>
    public class RotateObject : MonoBehaviour
    {
        [SerializeField] private Vector3 rotationSpeed = new Vector3(0f, 15f, 0f);

        private void Update()
        {
            transform.Rotate(rotationSpeed * Time.deltaTime, Space.World);
        }
    }
}