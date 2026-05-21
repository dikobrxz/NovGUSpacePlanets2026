using UnityEngine;
using UnityEngine.Events;

public class IceSample : MonoBehaviour
{
    public UnityEvent OnCollected;
    public UnityEvent OnBrokenOff;

    [SerializeField] private GameObject brokenPiecePrefab;
    private bool isBroken = false;

    public void RegisterHit()
    {
    }

    public void BreakOff()
    {
        if (isBroken) return;
        isBroken = true;
        
        Instantiate(brokenPiecePrefab, transform.position, Quaternion.identity);
        OnBrokenOff?.Invoke();
        gameObject.SetActive(false);
    }

    public void Collect()
    {
        OnCollected?.Invoke();
        Destroy(gameObject);
    }
}