using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tour_Endi_Moon
{
    /// <summary>
    /// Контроллер сцены StartScreen.
    /// Вращающаяся Луна + озвучка интро + переход на сцену moon.
    /// Переход происходит:
    ///   - по окончании интро-аудио, ИЛИ
    ///   - по нажатию кнопки контроллера (если игрок хочет пропустить).
    /// </summary>
    public class StartScreenManager : MonoBehaviour
    {
        [Header("Имя сцены с поверхностью Луны")]
        [SerializeField] private string moonSceneName = "moon";

        [Header("AudioSource с интро-озвучкой")]
        [SerializeField] private AudioSource introSource;
        [SerializeField] private AudioClip introClip;

        [Header("Задержка после окончания озвучки перед переходом, сек")]
        [SerializeField] private float postClipDelay = 1.0f;

        private bool transitioning;

        private void Start()
        {
            if (introSource != null && introClip != null)
            {
                introSource.clip = introClip;
                introSource.spatialBlend = 0f;
                introSource.Play();
                StartCoroutine(WaitForClipThenLoad());
            }
            else
            {
                // Если озвучки нет, ждём 5 секунд и переходим
                StartCoroutine(LoadAfterDelay(5f));
            }
        }

        private void Update()
        {
            // Пропуск интро: любая кнопка действия. Простой вариант — Input.anyKeyDown,
            // в XR — лучше навесить ваше InputAction (заменить здесь).
            if (!transitioning && Input.anyKeyDown)
                LoadMoonScene();
        }

        private IEnumerator WaitForClipThenLoad()
        {
            yield return new WaitWhile(() => introSource != null && introSource.isPlaying);
            yield return new WaitForSeconds(postClipDelay);
            LoadMoonScene();
        }

        private IEnumerator LoadAfterDelay(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            LoadMoonScene();
        }

        private void LoadMoonScene()
        {
            if (transitioning) return;
            transitioning = true;
            Debug.Log($"[StartScreenManager] Загружаю сцену '{moonSceneName}'");
            SceneManager.LoadScene(moonSceneName);
        }
    }
}
