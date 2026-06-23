using UnityEngine;

namespace TourverseToolkit.Runtime
{
    public sealed class PlayerCamera : MonoBehaviour
    {
        [field: SerializeField] public new Camera camera;

        private void OnValidate()
        {
            if (camera == null)
                camera = GetComponent<Camera>();
        }
    }
}