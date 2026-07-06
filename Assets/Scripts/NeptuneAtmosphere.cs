using UnityEngine;
using System.Collections;

public class NeptuneAtmosphere : MonoBehaviour
{
    [Header("Слои газа (Particle Systems)")]
    public ParticleSystem lowFog;
    public ParticleSystem midGas;
    public ParticleSystem highClouds;
    public ParticleSystem crystalParticles;

    public IEnumerator Dissipate(float duration)
    {
        float startLow = GetRate(lowFog);
        float startMid = GetRate(midGas);
        float startHigh = GetRate(highClouds);
        float startCrystal = GetRate(crystalParticles);

        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = t / duration;

            SetRate(lowFog, Mathf.Lerp(startLow, 0, p));
            SetRate(midGas, Mathf.Lerp(startMid, 0, p));
            SetRate(highClouds, Mathf.Lerp(startHigh, 0, p));
            SetRate(crystalParticles, Mathf.Lerp(startCrystal, 0, p));

            yield return null;
        }

        if (lowFog != null) lowFog.Stop();
        if (midGas != null) midGas.Stop();
        if (highClouds != null) highClouds.Stop();
        if (crystalParticles != null) crystalParticles.Stop();
    }

    float GetRate(ParticleSystem ps)
    {
        if (ps == null) return 0;
        return ps.emission.rateOverTime.constant;
    }

    void SetRate(ParticleSystem ps, float rate)
    {
        if (ps == null) return;
        var em = ps.emission;
        em.rateOverTime = rate;
    }
}