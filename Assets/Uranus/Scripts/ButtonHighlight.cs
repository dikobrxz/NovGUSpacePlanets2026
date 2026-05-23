using UnityEngine;

public class ButtonHighlight : MonoBehaviour
{
    [SerializeField] private Renderer buttonRenderer;
    [SerializeField] private Color normalColor = Color.red;
    [SerializeField] private Color hoverColor = Color.green;

    private Material buttonMaterial;

    void Start()
    {
        if (buttonRenderer == null)
            buttonRenderer = GetComponentInChildren<Renderer>();

        if (buttonRenderer != null)
            buttonMaterial = buttonRenderer.material;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor>() != null)
        {
            if (buttonMaterial != null)
                buttonMaterial.color = hoverColor;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor>() != null)
        {
            if (buttonMaterial != null)
                buttonMaterial.color = normalColor;
        }
    }
}