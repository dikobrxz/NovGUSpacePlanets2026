using System.Collections;
using UnityEngine;

public class VolcanoParticlesManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;

    private bool _isPlay = true;

    private void OnEnable()
    {
        _isPlay = true;

        StopAllCoroutines();
        StartCoroutine(EmitCoroutine());
    }

    private IEnumerator EmitCoroutine()
    {
        yield return new WaitForSeconds(Random.Range(1f, 7f));
        while (_isPlay) 
        {
            _particleSystem.Play();
            _audioSource.PlayOneShot(_audioClip);

            yield return new WaitForSeconds(Random.Range(12f, 25f));
        }
    }

    private void DisableEmit()
    {
        _isPlay = false;
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        DisableEmit();
    }

    private void OnDestroy()
    {
        DisableEmit();
    }
}
