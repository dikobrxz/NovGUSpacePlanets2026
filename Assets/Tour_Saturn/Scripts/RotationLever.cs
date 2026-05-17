using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class RotationLever : MonoBehaviour
{
    [SerializeField] private Transform pivot; // объект вращения
    [SerializeField] private Vector3 rotationAxis = Vector3.forward; //ось вращения

    private XRGrabInteractable grab; // компонент захвата
    private IXRSelectInteractor interactor; // объект, которым схватили (левая или правая рука)

    // повороты
    private Quaternion startHandRotation;
    private Quaternion startPivotRotation;

    private void Awake()
    {
        grab = GetComponent<XRGrabInteractable>(); //поиск компонента захвата

        // события захвата и отпуска рычажка
        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    private void Update()
    {
        if (interactor == null)
            return;

        // текущая позиция и поворот руки
        Transform hand = interactor.GetAttachTransform(grab);

        // разница между текущим поворотом и начальным поворотом при захвате (не абсолютный поворот руки, а изменение поворота)
        Quaternion delta =
            hand.rotation * Quaternion.Inverse(startHandRotation);

        // локальная ось объекта и перевод в мировые координаты
        Vector3 worldAxis =
            pivot.TransformDirection(rotationAxis.normalized);

        // перевод вращения в понятный вид (угол и ось)
        delta.ToAngleAxis(out float angle, out Vector3 axis);

        // определение направления
        if (Vector3.Dot(axis, worldAxis) < 0)
            angle = -angle;

        // поворот рычажка
        pivot.localRotation =
            startPivotRotation * Quaternion.AngleAxis(-angle, rotationAxis); // поворот вокруг нужной оси на заданный угол
    }

    // захват
    private void OnGrab(SelectEnterEventArgs args)
    {
        interactor = args.interactorObject; // какая рука схватила рычажок

        Transform hand = interactor.GetAttachTransform(grab); // точка, которая держит объект

        // начальный поворот руки и рычажка
        startHandRotation = hand.rotation;
        startPivotRotation = pivot.localRotation;
    }

    // отпуск и очистка хватальщика
    private void OnRelease(SelectExitEventArgs args)
    {
        interactor = null;
    }
}
