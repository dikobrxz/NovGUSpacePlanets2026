using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ToggleLever : MonoBehaviour
{
    [Header("Lever")]
    [SerializeField] private Transform leverPivot;

    [Header("Angles")]
    [SerializeField] private float upAngle = -35f;
    [SerializeField] private float downAngle = 35f;

    [Header("Settings")]
    [SerializeField] private float switchSpeed = 8f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip switchOnClip;
    [SerializeField] private AudioClip switchOffClip;

    private XRSimpleInteractable interactable;
    private bool isUp = true;
    private Quaternion targetRotation;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();

        interactable.selectEntered.AddListener(OnSelect);

        SetTargetRotation();
        leverPivot.localRotation = targetRotation;
    }

    private void Update()
    {
        leverPivot.localRotation = Quaternion.Lerp(
            leverPivot.localRotation,
            targetRotation,
            switchSpeed * Time.deltaTime
        );
    }

    private void OnSelect(SelectEnterEventArgs args)
    {
        isUp = !isUp;

        SetTargetRotation();

        if (audioSource != null)
        {
            if (isUp)
            {
                if (switchOnClip != null)
                    audioSource.PlayOneShot(switchOnClip);
            }
            else
            {
                if (switchOffClip != null)
                    audioSource.PlayOneShot(switchOffClip);
            }
        }
    }

    private void SetTargetRotation()
    {
        float angle = isUp ? upAngle : downAngle;
        targetRotation = Quaternion.Euler(angle, 0f, 0f);
    }

    private void OnDestroy()
    {
        interactable.selectEntered.RemoveListener(OnSelect);
    }
}
