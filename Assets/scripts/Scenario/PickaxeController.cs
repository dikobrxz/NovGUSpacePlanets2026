using UnityEngine;

public class PickaxeController : MonoBehaviour
{
    [SerializeField] private IceSample targetIce;
    [SerializeField] private int hitsRequired = 3;
    private int currentHits = 0;

    public void OnPickup() => gameObject.SetActive(false); 

    public void OnHit()
    {
        currentHits++;
        targetIce?.RegisterHit();
        
        if (currentHits >= hitsRequired)
        {
            targetIce?.BreakOff(); 
            gameObject.SetActive(false);
        }
    }
}