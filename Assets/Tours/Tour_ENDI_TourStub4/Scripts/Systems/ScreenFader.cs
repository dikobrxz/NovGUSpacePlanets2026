using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Tour_ENDI_TourStub4
{
    /// <summary>
    /// Градиентный фейд экрана в чёрный цвет и обратно.
    /// Canvas с Image вешается на Main Camera как дочерний объект.
    /// Используется TeleportController для переходов между локациями.
    /// </summary>
    public class ScreenFader : MonoBehaviour
    {
        [Header("Длительность фейда в каждую сторону (сек)")]
        [SerializeField] private float fadeDuration = 0.6f;

        private Image fadeImage;
        private Coroutine currentFade;

        private void Awake()
        {
            // Ищем Image — должна быть на этом объекте или дочернем
            fadeImage = GetComponentInChildren<Image>(true);
            if (fadeImage == null)
            {
                Debug.LogError("[ScreenFader] Image не найден. Убедись что на Canvas есть дочерний Image.");
                return;
            }

            SetAlpha(0f);
        }

        /// <summary>Затемнение → вызов action → осветление.</summary>
        public void FadeOutIn(Action onBlack, float fadeOut = -1f, float fadeIn = -1f)
        {
            float fo = fadeOut > 0 ? fadeOut : fadeDuration;
            float fi = fadeIn > 0 ? fadeIn : fadeDuration;
            if (currentFade != null) StopCoroutine(currentFade);
            currentFade = StartCoroutine(DoFadeOutIn(onBlack, fo, fi));
        }

        /// <summary>Только затемнение.</summary>
        public IEnumerator FadeOut(float duration = -1f)
        {
            float d = duration > 0 ? duration : fadeDuration;
            yield return StartCoroutine(DoFade(0f, 1f, d));
        }

        /// <summary>Только осветление.</summary>
        public IEnumerator FadeIn(float duration = -1f)
        {
            float d = duration > 0 ? duration : fadeDuration;
            yield return StartCoroutine(DoFade(1f, 0f, d));
        }

        private IEnumerator DoFadeOutIn(Action onBlack, float fadeOutDur, float fadeInDur)
        {
            yield return StartCoroutine(DoFade(0f, 1f, fadeOutDur));
            onBlack?.Invoke();
            // Небольшая пауза на чёрном экране
            yield return new WaitForSeconds(0.1f);
            yield return StartCoroutine(DoFade(1f, 0f, fadeInDur));
        }

        private IEnumerator DoFade(float from, float to, float duration)
        {
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                SetAlpha(Mathf.Lerp(from, to, t / duration));
                yield return null;
            }
            SetAlpha(to);
        }

        private void SetAlpha(float alpha)
        {
            if (fadeImage == null) return;
            Color c = fadeImage.color;
            c.a = alpha;
            fadeImage.color = c;
        }
    }
}
