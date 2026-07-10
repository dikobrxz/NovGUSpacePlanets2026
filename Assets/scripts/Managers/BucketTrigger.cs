using UnityEngine;

public class BucketTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Игнорируем ВСЁ, кроме льда
        if (!other.name.ToLower().Contains("ice") && !other.CompareTag("IceChunk")) 
            return;

        var manager = FindAnyObjectByType<SceneManager>();
        if (manager != null)
        {
            manager.OnIcePlacedInBucket();
            Destroy(other.gameObject);
        }
    }
}