using UnityEngine;

public class VolcanoCrater : MonoBehaviour
{
    [SerializeField] private Cryovolcano volcano;
    
    private void OnTriggerEnter(Collider other)
    {
        Activator activator = other.GetComponent<Activator>();
        
        if (activator != null)
        {
            Debug.Log($"✅ Активатор брошен в жерло: {other.name}");
            Destroy(other.gameObject);
            
            if (volcano != null) volcano.ActivateEruption();
        }
    }
}