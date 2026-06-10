using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls Earth intro scene and smoothly loads the main Earth scene.
/// </summary>
public class EarthIntroSceneController : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string mainSceneName = "Scene_Earth_Main";

    [Header("Fade")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeOutDuration = 1.5f;

    [Header("Audio")]
    [SerializeField] private AudioSource narrationSource;
    [SerializeField] private AudioClip introClip;

    [Header("Timing")]
    [SerializeField] private float fallbackDuration = 5f;
    [SerializeField] private float delayBeforeFade = 0.5f;

    private IEnumerator Start()
    {
        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 0f;

        if (narrationSource != null && introClip != null)
        {
            narrationSource.clip = introClip;
            narrationSource.Play();

            yield return new WaitWhile(() => narrationSource != null && narrationSource.isPlaying);
        }
        else
        {
            yield return new WaitForSeconds(fallbackDuration);
        }

        yield return new WaitForSeconds(delayBeforeFade);

        if (fadeCanvasGroup != null)
            yield return FadeToBlack();

        TourSceneState.StartMainSceneOnIsland = true;
        SceneManager.LoadScene(mainSceneName);
    }

    private IEnumerator FadeToBlack()
    {
        float time = 0f;
        float startAlpha = fadeCanvasGroup.alpha;

        while (time < fadeOutDuration)
        {
            time += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, time / fadeOutDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 1f;
    }
}
