using UnityEngine;
using UnityEngine.InputSystem;

public class ContinueInput : MonoBehaviour
{
    [SerializeField] private InputActionProperty continueAction;

    public bool WasPressed { get; private set; }

    private void OnEnable()
    {
        continueAction.action.Enable();
    }

    private void OnDisable()
    {
        continueAction.action.Disable();
    }

    private void Update()
    {
        if (continueAction.action.WasPressedThisFrame())
        {
            WasPressed = true;
        }
    }

    public void ResetPress()
    {
        WasPressed = false;
    }
}
