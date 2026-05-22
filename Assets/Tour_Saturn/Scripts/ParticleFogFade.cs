using UnityEngine;
using System.Collections;

public class ParticleFogFade : MonoBehaviour
{
    [SerializeField] private ParticleSystem fog;
    [SerializeField] private float fadeDuration = 3f;

    private ParticleSystem.EmissionModule emission;
    private float startRate;

    private void Awake()
    {
        emission = fog.emission;
        startRate = emission.rateOverTime.constant;
    }

    public IEnumerator FadeOut()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            float t = timer / fadeDuration;
            float currentRate = Mathf.Lerp(startRate, 0f, t);

            emission.rateOverTime = currentRate;

            yield return null;
        }

        emission.rateOverTime = 0f;

        yield return new WaitForSeconds(fog.main.startLifetime.constant);

        fog.Stop();
        gameObject.SetActive(false);
    }
}
