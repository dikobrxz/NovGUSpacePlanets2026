using UnityEngine;
using System.Collections;

public class ParticleFadeOut : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private bool disableAfterFade = true;

    private ParticleSystem[] particleSystems;
    private ParticleSystemRenderer[] renderers;

    private Material[][] materials;
    private Color[][] startColors;

    private void Awake()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>(true);
        renderers = GetComponentsInChildren<ParticleSystemRenderer>(true);

        materials = new Material[renderers.Length][];
        startColors = new Color[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
        {
            materials[i] = renderers[i].materials;
            startColors[i] = new Color[materials[i].Length];

            for (int j = 0; j < materials[i].Length; j++)
            {
                startColors[i][j] = materials[i][j].color;
            }
        }
    }

    public IEnumerator FadeOut()
    {
        foreach (ParticleSystem ps in particleSystems)
        {
            var emission = ps.emission;
            emission.enabled = false;
        }

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            for (int i = 0; i < materials.Length; i++)
            {
                for (int j = 0; j < materials[i].Length; j++)
                {
                    Color color = startColors[i][j];
                    color.a = Mathf.Lerp(startColors[i][j].a, 0f, t);
                    materials[i][j].color = color;
                }
            }

            yield return null;
        }

        foreach (ParticleSystem ps in particleSystems)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        if (disableAfterFade)
            gameObject.SetActive(false);
    }
}
