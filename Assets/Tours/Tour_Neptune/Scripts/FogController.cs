using UnityEngine;
using System.Collections;

public class FogController : MonoBehaviour
{
    public GameObject fogSphere;
    public GameObject saturn;
    public float dissolveDuration = 5f;

    public IEnumerator DissolveFog()
    {
        Renderer rend = fogSphere.GetComponent<Renderer>();
        Material mat = rend.material;
        Color startColor = mat.color;
        float t = 0;

        while (t < dissolveDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0f, t / dissolveDuration);
            mat.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        fogSphere.SetActive(false);

        if (saturn != null)
            StartCoroutine(AppearSaturn());
    }

    IEnumerator AppearSaturn()
    {
        saturn.SetActive(true);
        Renderer[] renderers = saturn.GetComponentsInChildren<Renderer>();
        
        float t = 0;
        float duration = 3f;

        foreach (var r in renderers)
        foreach (var m in r.materials)
            m.color = new Color(m.color.r, m.color.g, m.color.b, 0);

        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / duration);
            foreach (var r in renderers)
            foreach (var m in r.materials)
                m.color = new Color(m.color.r, m.color.g, m.color.b, alpha);
            yield return null;
        }
    }
}