using UnityEngine;

/// <summary>
/// Rotates the moon around the Earth and optionally rotates the moon itself.
/// Attach this script to MoonOrbitPivot. Put MoonOrbitPivot exactly in Earth's center.
/// </summary>
public class MoonOrbit : MonoBehaviour
{
    [Header("Orbit")]
    [SerializeField] private Transform earthCenter;
    [SerializeField] private Vector3 orbitAxis = Vector3.up;
    [SerializeField] private float orbitSpeed = 4f;
    [SerializeField] private bool keepPivotAtEarthCenter = true;

    [Header("Moon Distance")]
    [SerializeField] private Transform moonTransform;
    [SerializeField] private bool forceMoonLocalDistance = true;
    [SerializeField] private float moonLocalDistance = 1.6f;

    [Header("Moon Self Rotation")]
    [SerializeField] private Vector3 moonRotationAxis = Vector3.up;
    [SerializeField] private float moonRotationSpeed = 8f;

    private void LateUpdate()
    {
        if (keepPivotAtEarthCenter && earthCenter != null)
            transform.position = earthCenter.position;

        if (moonTransform != null && forceMoonLocalDistance)
            moonTransform.localPosition = new Vector3(moonLocalDistance, 0f, 0f);

        transform.Rotate(orbitAxis, orbitSpeed * Time.deltaTime, Space.Self);

        if (moonTransform != null)
            moonTransform.Rotate(moonRotationAxis, moonRotationSpeed * Time.deltaTime, Space.Self);
    }
}
