using UnityEngine;
using System.Collections;

public class FadeScreen : MonoBehaviour
{
    public Renderer fadeRenderer;
    public float fadeDuration = 1f;

    void Start()
    {
        StartCoroutine(FadeIn());
    }

    public IEnumerator FadeIn()
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(0);
        fadeRenderer.gameObject.SetActive(false);
    }

    public IEnumerator FadeOut()
    {
        fadeRenderer.gameObject.SetActive(true);
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(1);
    }

    void SetAlpha(float alpha)
    {
        Color c = fadeRenderer.material.color;
        fadeRenderer.material.color = new Color(c.r, c.g, c.b, alpha);
    }
}