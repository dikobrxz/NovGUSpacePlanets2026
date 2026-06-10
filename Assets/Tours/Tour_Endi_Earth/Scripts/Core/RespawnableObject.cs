using UnityEngine;

/// <summary>
/// Stores the initial position of an important object and allows returning it back.
/// Useful for quest items that can fall under the map or get lost.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class RespawnableObject : MonoBehaviour
{
    [SerializeField] private Transform customRespawnPoint;

    private Rigidbody objectRigidbody;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody>();

        if (customRespawnPoint != null)
        {
            initialPosition = customRespawnPoint.position;
            initialRotation = customRespawnPoint.rotation;
        }
        else
        {
            initialPosition = transform.position;
            initialRotation = transform.rotation;
        }
    }

    public void Respawn()
    {
        objectRigidbody.isKinematic = true;

#if UNITY_6000_0_OR_NEWER
        objectRigidbody.linearVelocity = Vector3.zero;
#else
        objectRigidbody.velocity = Vector3.zero;
#endif

        objectRigidbody.angularVelocity = Vector3.zero;

        transform.position = initialPosition;
        transform.rotation = initialRotation;

        objectRigidbody.isKinematic = false;
    }
}