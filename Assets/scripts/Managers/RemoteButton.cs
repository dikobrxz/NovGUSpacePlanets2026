using UnityEngine;

public class RemoteButton : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Игнорируем лёд и таз, чтобы не срабатывало случайно
        if (other.CompareTag("IceChunk") || 
            other.name.Contains("ice") || 
            other.name.Contains("bucket"))
            return;

        if (other.name.Contains("Controller") || 
            other.name.Contains("Hand") || 
            other.attachedRigidbody != null)
        {
            FindAnyObjectByType<SceneManager>()?.OnRemoteButtonPressed();
        }
    }
}