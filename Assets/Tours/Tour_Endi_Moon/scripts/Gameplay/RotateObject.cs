using UnityEngine;

namespace Tour_Endi_Moon
{
    /// <summary>
    /// Простое вращение объекта вокруг своей оси.
    /// Для вращающейся Луны на стартовом экране.
    /// </summary>
    public class RotateObject : MonoBehaviour
    {
        [Tooltip("Скорость вращения, градусов в секунду")]
        [SerializeField] private Vector3 degreesPerSecond = new(0f, 15f, 0f);

        private void Update()
        {
            transform.Rotate(degreesPerSecond * Time.deltaTime, Space.Self);
        }
    }
}
