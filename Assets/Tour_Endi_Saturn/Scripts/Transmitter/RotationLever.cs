using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
namespace Tour_Endi_Saturn
{

public class RotationLever : MonoBehaviour
{
    [SerializeField] private Transform pivot;
    [SerializeField] private Vector3 rotationAxis = Vector3.forward;

    [Header("Value Settings")]
    [SerializeField] private int minValue = 2000;
    [SerializeField] private int maxValue = 2020;

    [Header("Angle Limits")]
    [SerializeField] private float minAngle = -90f;
    [SerializeField] private float maxAngle = 90f;

    [Header("UI")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private bool isFirstLever = true;

    private int lastPlayedValue;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip rotateTickClip;

    private XRGrabInteractable grab;
    private IXRSelectInteractor interactor;

    private Quaternion startHandRotation;
    private Quaternion startPivotRotation;

    private float currentAngle;
    private float startAngle;

    public int CurrentValue { get; private set; }

    public event Action OnValueChanged;

    private void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();

        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    private void Start()
    {
        UpdateValueFromCurrentRotation();

        lastPlayedValue = CurrentValue;
    }

    private void Update()
    {
        if (interactor == null)
            return;

        Transform hand = interactor.GetAttachTransform(grab);

        Quaternion delta = hand.rotation * Quaternion.Inverse(startHandRotation);

        Vector3 worldAxis = pivot.TransformDirection(rotationAxis.normalized);

        delta.ToAngleAxis(out float angle, out Vector3 axis);

        if (Vector3.Dot(axis, worldAxis) < 0)
            angle = -angle;

        float deltaAngle = angle;

        currentAngle = Mathf.Clamp(
            startAngle + deltaAngle,
            minAngle,
            maxAngle
        );

        pivot.localRotation =
            startPivotRotation * Quaternion.AngleAxis(currentAngle - startAngle, rotationAxis);

        UpdateValue(currentAngle);
    }

    private void UpdateValue(float angle)
    {
        float t = Mathf.InverseLerp(minAngle, maxAngle, angle);
        int newValue = Mathf.RoundToInt(Mathf.Lerp(minValue, maxValue, t));

        if (newValue == CurrentValue)
            return;

        CurrentValue = newValue;

        if (CurrentValue != lastPlayedValue)
        {
            lastPlayedValue = CurrentValue;

            if (audioSource != null && rotateTickClip != null)
                audioSource.PlayOneShot(rotateTickClip);
        }

        Debug.Log($"{gameObject.name}: {CurrentValue}");

        OnValueChanged?.Invoke();

        if (isFirstLever)
            uiManager.ShowFirstLeverValue(CurrentValue);
        else
            uiManager.ShowSecondLeverValue(CurrentValue);
    }

    private void UpdateValueFromCurrentRotation()
    {
        float angle = Vector3.SignedAngle(
            Vector3.forward,
            pivot.forward,
            rotationAxis
        );

        UpdateValue(angle);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        interactor = args.interactorObject;

        Transform hand = interactor.GetAttachTransform(grab);

        startHandRotation = hand.rotation;
        startPivotRotation = pivot.localRotation;

        startAngle = currentAngle;

        if (isFirstLever)
            uiManager.ShowFirstLeverValue(CurrentValue);
        else
            uiManager.ShowSecondLeverValue(CurrentValue);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        interactor = null;
    }

    private void OnDestroy()
    {
        grab.selectEntered.RemoveListener(OnGrab);
        grab.selectExited.RemoveListener(OnRelease);
    }
}
}
