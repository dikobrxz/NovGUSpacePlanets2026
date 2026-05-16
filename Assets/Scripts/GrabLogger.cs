using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabLogger : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        Debug.Log("��������� ����! (Marker grabbed)");

        GetComponent<Renderer>().material.color = Color.red;
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
    }
}
