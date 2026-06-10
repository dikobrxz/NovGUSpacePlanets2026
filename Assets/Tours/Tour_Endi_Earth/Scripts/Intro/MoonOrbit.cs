using UnityEngine;

/// <summary>
/// Rotates the moon around the Earth and optionally rotates the moon itself.
/// Attach this script to MoonOrbitPivot.
/// </summary>
public class MoonOrbit : MonoBehaviour
{
    [Header("Orbit")]
    [SerializeField] private Vector3 orbitAxis = Vector3.up;
    [SerializeField] private float orbitSpeed = 10f;

    [Header("Moon Self Rotation")]
    [SerializeField] private Transform moonTransform;
    [SerializeField] private Vector3 moonRotationAxis = Vector3.up;
    [SerializeField] private float moonRotationSpeed = 15f;

    private void Update()
    {
        transform.Rotate(orbitAxis, orbitSpeed * Time.deltaTime, Space.Self);

        if (moonTransform != null)
            moonTransform.Rotate(moonRotationAxis, moonRotationSpeed * Time.deltaTime, Space.Self);
    }
}