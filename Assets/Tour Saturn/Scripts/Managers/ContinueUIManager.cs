using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ContinueUIManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private ContinueUIButton continueButton;
    [SerializeField] private GameObject titanHint;

    [Header("Placement")]
    [SerializeField] private Transform canvasRoot;
    [SerializeField] private float distance = 0.001f;
    [SerializeField] private float heightOffset = -0.1f;

    public bool WasPressed { get; private set; }

    private void Start()
    {
        Hide();
    }

    public void Show(string text)
    {
        WasPressed = false;
        panel.SetActive(true);

        continueButton.SetButton(text, this);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    public void PressContinue()
    {
        WasPressed = true;
    }

    public void PlaceNearPlayer(Transform playerCamera, float sideOffset = 0f, float  customDistance = -1f)
    {
        Vector3 forward = playerCamera.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = playerCamera.right;
        right.y = 0f;
        right.Normalize();

        float usedDistance = customDistance > 0f ? customDistance : distance;

        Vector3 position = playerCamera.position + forward * usedDistance + right * sideOffset;

        position.y = playerCamera.position.y + heightOffset;

        canvasRoot.position = position;

        Vector3 directionToCamera = playerCamera.position - canvasRoot.position;

        directionToCamera.y = 0f;

        canvasRoot.rotation =
            Quaternion.LookRotation(-directionToCamera);
    }

    public void ShowMessage(string text)
    {
        panel.SetActive(true);
        titanHint.SetActive(true);
        continueButton.gameObject.SetActive(false);
    }

    public void HideMessage()
    {
        panel.SetActive(false);
        titanHint.SetActive(false);
        continueButton.gameObject.SetActive(true);
    }
}
