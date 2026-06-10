using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Marks an interactable object as one of the Earth tour elements.
/// </summary>
public class ElementItem : MonoBehaviour
{
    [SerializeField] private ElementType elementType;
    [SerializeField] private XRGrabInteractable grabInteractable;

    public ElementType ElementType => elementType;

    public bool IsLockedOnPedestal { get; private set; }

    public bool IsGrabbed
    {
        get
        {
            return grabInteractable != null && grabInteractable.isSelected;
        }
    }

    private void Reset()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void Awake()
    {
        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();
    }

    public void LockOnPedestal(Transform snapPoint)
    {
        if (snapPoint == null)
            return;

        IsLockedOnPedestal = true;

        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (grabInteractable != null)
            grabInteractable.enabled = false;

        transform.SetParent(snapPoint);
        transform.SetPositionAndRotation(snapPoint.position, snapPoint.rotation);
    }
}