using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GrabLogger : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private Renderer objectRenderer;
    private Color originalColor;
    private readonly Color grabColor = new Color(0.8039f, 0.3137f, 0.2627f);

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        objectRenderer = GetComponent<Renderer>();

        originalColor = objectRenderer.material.color;
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        objectRenderer.material.color = grabColor;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        objectRenderer.material.color = originalColor;
    }

    private void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }
}