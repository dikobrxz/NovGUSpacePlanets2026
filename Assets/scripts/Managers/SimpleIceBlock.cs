using UnityEngine;

public class SimpleIceBlock : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private int hitsNeeded = 3;
    [SerializeField] private ParticleSystem hitParticles;
    
    private int currentHits = 0;
    private bool isDestroyed = false;

    // Срабатывает при физическом столкновении
    private void OnCollisionEnter(Collision collision)
    {
        if (isDestroyed) return;

        if (collision.gameObject.name.Contains("pickaxe") || 
            collision.gameObject.name.Contains("Pickaxe"))
        {
            RegisterHit(collision);
        }
    }

    private void RegisterHit(Collision collision)
    {
        currentHits++;
        Debug.Log($" Удар {currentHits} из {hitsNeeded}");

        if (hitParticles != null && collision.contactCount > 0)
        {
            Vector3 hitPoint = collision.contacts[0].point;
            GameObject particlesObj = Instantiate(hitParticles.gameObject, hitPoint, Quaternion.identity);
            
            ParticleSystem ps = particlesObj.GetComponent<ParticleSystem>();
            if (ps != null) ps.Play();
            
            Destroy(particlesObj, 1f);
        }

        var sceneManager = FindAnyObjectByType<SceneManager>();
        if (sceneManager != null)
        {
            sceneManager.RegisterIceHit(transform.position);
        }
        else
        {
            Debug.LogWarning("SceneManager не найден на сцене!");
        }

        if (currentHits >= hitsNeeded)
        {
            DestroyBlock();
        }
    }

    private void DestroyBlock()
    {
        isDestroyed = true;
        gameObject.SetActive(false);
    }
}