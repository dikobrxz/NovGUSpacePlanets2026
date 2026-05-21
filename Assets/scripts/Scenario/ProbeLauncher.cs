using UnityEngine;

public class ProbeLauncher : MonoBehaviour
{
    public bool WasPressed { get; private set; } = false;
    [SerializeField] private GameObject buttonModel;
    [SerializeField] private Material normalMat, pressedMat;

    public void EnableButton(bool enabled)
    {
        GetComponent<Collider>().enabled = enabled;
        buttonModel?.GetComponent<Renderer>().material = enabled ? normalMat : normalMat;
    }

    private void OnMouseDown()
    {
        if (!WasPressed)
        {
            WasPressed = true;
            buttonModel?.GetComponent<Renderer>().material = pressedMat;
        }
    }
}