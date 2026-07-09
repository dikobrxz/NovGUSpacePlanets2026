using UnityEngine;

namespace Tour_Endi_Moon
{
    /// <summary>
    /// Управляет видимостью объекта в зависимости от этапа сценария.
    /// Отметь галочками этапы, на которых объект должен быть виден.
    /// </summary>
    public class StateVisibility : MonoBehaviour
    {
        [Header("На каких этапах объект виден")]
        [SerializeField] private bool intro       = true;
        [SerializeField] private bool landing     = true;
        [SerializeField] private bool monologue   = false;
        [SerializeField] private bool exploration = false;
        [SerializeField] private bool collecting  = false;
        [SerializeField] private bool returnStage = false;
        [SerializeField] private bool quiz        = true;
        [SerializeField] private bool end         = true;

        private void Start()
        {
            var story = GameManager.Instance != null ? GameManager.Instance.Story : null;
            if (story != null)
            {
                story.OnStateChanged += OnStateChanged;
                // Применяем сразу при старте
                OnStateChanged(story.GetCurrentStage());
            }
        }

        private void OnDestroy()
        {
            var story = GameManager.Instance != null ? GameManager.Instance.Story : null;
            if (story != null)
                story.OnStateChanged -= OnStateChanged;
        }

        private void OnStateChanged(GameState state)
        {
            gameObject.SetActive(state switch
            {
                GameState.Intro       => intro,
                GameState.Landing     => landing,
                GameState.Monologue   => monologue,
                GameState.Exploration => exploration,
                GameState.Collecting  => collecting,
                GameState.Return      => returnStage,
                GameState.Quiz        => quiz,
                GameState.End         => end,
                _                     => false
            });
        }
    }
}
