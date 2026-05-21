using UnityEngine;
using UnityEngine.Events;

public class Container : MonoBehaviour
{
    public UnityEvent OnSamplePlaced;
    private bool hasSample = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("IceSample") && !hasSample)
        {
            hasSample = true;
            Destroy(other.gameObject);
            OnSamplePlaced?.Invoke();
        }
    }
}