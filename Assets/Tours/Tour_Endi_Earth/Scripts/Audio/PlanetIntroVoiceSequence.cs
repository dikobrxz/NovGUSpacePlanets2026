using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Plays planet intro voice clips one by one,
/// then returns to the main scene on the island.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class PlanetIntroVoiceSequence : MonoBehaviour
{
    [Header("Voice Clips")]
    [SerializeField] private AudioClip mainIntroClip;
    [SerializeField] private AudioClip planetCutsceneClip;

    [Header("Scene Transition")]
    [SerializeField] private string mainSceneName = "Scene_Earth_Main";
    [SerializeField] private float delayAfterVoice = 1f;

    private AudioSource audioSource;

    private IEnumerator Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;

        yield return PlayClip(mainIntroClip);
        yield return PlayClip(planetCutsceneClip);

        yield return new WaitForSeconds(delayAfterVoice);

        TourSceneState.StartMainSceneOnIsland = true;
        SceneManager.LoadScene(mainSceneName);
    }

    private IEnumerator PlayClip(AudioClip clip)
    {
        if (clip == null)
            yield break;

        audioSource.clip = clip;
        audioSource.Play();

        yield return new WaitForSeconds(clip.length);
    }
}