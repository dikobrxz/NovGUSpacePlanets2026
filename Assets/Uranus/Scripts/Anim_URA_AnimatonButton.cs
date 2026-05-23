using UnityEngine;
using System.Collections;

public class AnimatedButton : MonoBehaviour
{
    [Header("Button Settings")]
    [SerializeField] private Transform buttonMesh;
    [SerializeField] private float pressDepth = 50f;
    [SerializeField] private float animationSpeed = 12f;
    [SerializeField] private float holdTime = 0.15f;

    private Vector3 originalPosition;
    private Vector3 pressedPosition;
    private bool isPressed = false;
    private bool canPress = true;

    void Start()
    {
        if (buttonMesh == null)
            buttonMesh = transform.Find("ButtonMesh");

        originalPosition = buttonMesh.localPosition;
        pressedPosition = originalPosition - new Vector3(0, pressDepth, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canPress) return;


        if (other.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor>() != null ||
            other.name.Contains("Hand") ||
            other.name.Contains("Controller") ||
            other.CompareTag("Player"))
        {
            if (!isPressed)
                StartCoroutine(AnimatePress());
        }
    }

    private IEnumerator AnimatePress()
    {
        isPressed = true;
        canPress = false;

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * animationSpeed;
            buttonMesh.localPosition = Vector3.Lerp(originalPosition, pressedPosition, t);
            yield return null;
        }

        buttonMesh.localPosition = pressedPosition;

        OnButtonClick();

        yield return new WaitForSeconds(holdTime);

        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * animationSpeed;
            buttonMesh.localPosition = Vector3.Lerp(pressedPosition, originalPosition, t);
            yield return null;
        }

        buttonMesh.localPosition = originalPosition;
        isPressed = false;

        yield return new WaitForSeconds(0.2f);
        canPress = true;
    }

    private void OnButtonClick()
    {
        Debug.Log("Button clicked!");

    }
}