using UnityEngine;
using System.Collections;

public class ParticleFadeOut : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private float waitAfterFade = 1f;
    [SerializeField] private bool disableAfterFade = true;

    private ParticleSystem[] particleSystems;
    private float[] startRates;

    private void Awake()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>(true);
        startRates = new float[particleSystems.Length];

        for (int i = 0; i < particleSystems.Length; i++)
        {
            var emission = particleSystems[i].emission;
            startRates[i] = emission.rateOverTime.constant;
        }
    }

    public IEnumerator FadeOut()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            for (int i = 0; i < particleSystems.Length; i++)
            {
                var emission = particleSystems[i].emission;
                emission.rateOverTime = Mathf.Lerp(startRates[i], 0f, t);
            }

            yield return null;
        }

        for (int i = 0; i < particleSystems.Length; i++)
        {
            var emission = particleSystems[i].emission;
            emission.rateOverTime = 0f;
        }

        yield return new WaitForSeconds(waitAfterFade);

        for (int i = 0; i < particleSystems.Length; i++)
        {
            particleSystems[i].Stop();
        }

        if (disableAfterFade)
            gameObject.SetActive(false);
    }
}
