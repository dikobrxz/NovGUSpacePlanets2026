using System.Collections;
using UnityEngine;

/// <summary>
/// Black fade controller for VR scenes.
/// Uses CanvasGroup alpha.
/// </summary>
public class ScreenFade : MonoBehaviour
{
    public static ScreenFade Instance { get; private set; }

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float defaultDuration = 1f;
    [SerializeField] private bool fadeInOnStart = true;

    private Coroutine currentFadeRoutine;

    private void Awake()
    {
        Instance = this;

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        if (canvasGroup == null)
            return;

        if (fadeInOnStart)
        {
            canvasGroup.alpha = 1f;
            FadeIn(defaultDuration);
        }
        else
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public Coroutine FadeIn(float duration)
    {
        return StartFade(1f, 0f, duration);
    }

    public Coroutine FadeOut(float duration)
    {
        return StartFade(0f, 1f, duration);
    }

    private Coroutine StartFade(float from, float to, float duration)
    {
        if (canvasGroup == null)
            return null;

        if (currentFadeRoutine != null)
            StopCoroutine(currentFadeRoutine);

        currentFadeRoutine = StartCoroutine(FadeRoutine(from, to, duration));
        return currentFadeRoutine;
    }

    private IEnumerator FadeRoutine(float from, float to, float duration)
    {
        float timer = 0f;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = from;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);

            canvasGroup.alpha = Mathf.Lerp(from, to, t);

            yield return null;
        }

        canvasGroup.alpha = to;
        canvasGroup.blocksRaycasts = to > 0.01f;
    }
}