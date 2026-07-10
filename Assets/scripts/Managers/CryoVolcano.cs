using UnityEngine;

public class Cryovolcano : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ParticleSystem eruptionEffect;
    [SerializeField] private GameObject sampleContainer;
    [SerializeField] private Transform probeTargetPoint;
    
    [Header("Settings")]
    [SerializeField] private float eruptionDuration = 8f;
    [SerializeField] private AudioClip eruptionSound;
    [SerializeField] private AudioSource audioSource;
    
    private bool isActivated = false;
    private bool isErupting = false;
    private bool sampleCollected = false;

    public void ActivateEruption()
    {
        if (isActivated) 
        {
            Debug.Log("⚠️ Криовулкан уже активирован!");
            return;
        }
        
        isActivated = true;
        isErupting = true;
        Debug.Log("🌋 Криовулкан активирован! Извержение жидкого азота...");
        
        if (eruptionEffect != null) eruptionEffect.Play();
        
        if (audioSource != null && eruptionSound != null)
            audioSource.PlayOneShot(eruptionSound);
        
        if (sampleContainer != null)
        {
            sampleContainer.SetActive(true);
            Debug.Log("✅ Образец азотного льда появился в жерле!");
        }
        
        Invoke(nameof(StopEruption), eruptionDuration);
        
        var sceneManager = FindAnyObjectByType<SceneManager>();
        if (sceneManager != null) sceneManager.OnCryovolcanoActivated();
        
        HUDManager.ShowHint("Криовулкан извергается! Используйте пульт для запуска зонда");
    }

    private void StopEruption()
    {
        if (eruptionEffect != null) eruptionEffect.Stop();
        isErupting = false;
        Debug.Log("🌋 Извержение завершено");
    }

    public void CollectSample()
    {
        if (sampleCollected) return;
        if (sampleContainer == null || !sampleContainer.activeInHierarchy)
        {
            Debug.LogWarning("⚠️ Образец ещё не появился!");
            return;
        }
        
        sampleCollected = true;
        sampleContainer.SetActive(false);
        Debug.Log("✅ Образец криовулкана собран зондом!");
        
        var sceneManager = FindAnyObjectByType<SceneManager>();
        if (sceneManager != null) sceneManager.OnCryovolcanoSampleCollected();
    }

    public bool IsSampleAvailable() 
        => sampleContainer != null && sampleContainer.activeInHierarchy && !sampleCollected;
    
    public Transform GetProbeTargetPoint() => probeTargetPoint;
    public bool IsActivated() => isActivated;
}