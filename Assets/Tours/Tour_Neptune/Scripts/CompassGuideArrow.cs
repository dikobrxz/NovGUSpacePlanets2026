using UnityEngine;
using TMPro;
using System.Collections;

public class CompassGuideArrow : MonoBehaviour
{
    public Transform compass;
    public Transform playerCamera;
    public float fadeDelay = 1f;
    public float fadeDuration = 1f;
    public float lookThreshold = 30f;
    public float distanceFromPlayer = 1.5f;
    public float heightOffset = -0.7f; // ниже глаз, на уровне рук

    private TMP_Text arrowText;
    private bool isDone = false;

    void Start()
    {
        arrowText = GetComponent<TMP_Text>();
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        isDone = false;
        if (arrowText != null)
            arrowText.alpha = 1f;
        StartCoroutine(GuideRoutine());
    }

    IEnumerator GuideRoutine()
    {
        while (!isDone)
        {
            if (compass != null && playerCamera != null)
            {
                Vector3 dirToCompass = (compass.position - playerCamera.position);
                dirToCompass.y = 0;
                dirToCompass.Normalize();

                // Перед игроком, ниже камеры (на уровне контроллеров)
                Vector3 forward = playerCamera.forward;
                forward.y = 0;
                forward.Normalize();

                Vector3 arrowPos = playerCamera.position + forward * distanceFromPlayer;
                arrowPos.y = playerCamera.position.y + heightOffset;
                transform.position = arrowPos;

                // Стрелка указывает на компас, лицом к игроку
                transform.LookAt(playerCamera.position);
                transform.Rotate(0, 180, 0);

                // Проверяем смотрит ли игрок на компас
                float angle = Vector3.Angle(playerCamera.forward, dirToCompass);
                if (angle < lookThreshold)
                    isDone = true;
            }
            yield return null;
        }

        yield return new WaitForSeconds(fadeDelay);

        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            if (arrowText != null)
                arrowText.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }

        gameObject.SetActive(false);
    }
}