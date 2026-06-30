using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Tour_Endi_Earth
{
    /// <summary>
    /// Physical XR hitbox for quiz answer.
    /// It makes quiz buttons stable for VR ray interaction.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(XRSimpleInteractable))]
    public class EarthQuizAnswerInteractable : MonoBehaviour
    {
        [Header("Quiz")]
        [SerializeField] private EarthQuizManager quizManager;
        [SerializeField] private int answerIndex;

        [Header("Visual Target")]
        [SerializeField] private Image targetButtonImage;
        [SerializeField] private Renderer targetButtonRenderer;
        [SerializeField] private Transform visualTransform;

        [Header("Colors")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color hoverColor = new Color(1f, 0.85f, 0.25f);
        [SerializeField] private Color correctColor = new Color(0.25f, 1f, 0.35f);
        [SerializeField] private Color wrongColor = new Color(1f, 0.25f, 0.25f);

        [Header("Animation")]
        [SerializeField] private float hoverScale = 1.05f;
        [SerializeField] private float resultDuration = 0.7f;

        private XRSimpleInteractable interactable;
        private Vector3 defaultScale;
        private bool isPressed;

        private void Awake()
        {
            interactable = GetComponent<XRSimpleInteractable>();

            interactable.hoverEntered.AddListener(OnHoverEntered);
            interactable.hoverExited.AddListener(OnHoverExited);
            interactable.activated.AddListener(OnActivated);
            interactable.selectEntered.AddListener(OnSelected);

            if (visualTransform != null)
                defaultScale = visualTransform.localScale;

            SetVisualColor(normalColor);
        }

        private void OnDestroy()
        {
            if (interactable == null)
                return;

            interactable.hoverEntered.RemoveListener(OnHoverEntered);
            interactable.hoverExited.RemoveListener(OnHoverExited);
            interactable.activated.RemoveListener(OnActivated);
            interactable.selectEntered.RemoveListener(OnSelected);
        }

        private void OnHoverEntered(HoverEnterEventArgs args)
        {
            if (isPressed)
                return;

            SetVisualColor(hoverColor);

            if (visualTransform != null)
                visualTransform.localScale = defaultScale * hoverScale;
        }

        private void OnHoverExited(HoverExitEventArgs args)
        {
            if (isPressed)
                return;

            SetVisualColor(normalColor);

            if (visualTransform != null)
                visualTransform.localScale = defaultScale;
        }

        private void OnActivated(ActivateEventArgs args)
        {
            PressAnswer();
        }

        private void OnSelected(SelectEnterEventArgs args)
        {
            PressAnswer();
        }

        private void PressAnswer()
        {
            if (isPressed)
                return;

            if (quizManager == null)
            {
                Debug.LogWarning($"{name}: QuizManager is not assigned.");
                return;
            }

            if (GameAudioManager.Instance != null)
                GameAudioManager.Instance.PlayButtonClick();

            isPressed = true;

            bool isCorrect = quizManager.IsCurrentAnswerCorrect(answerIndex);

            SetVisualColor(isCorrect ? correctColor : wrongColor);

            StartCoroutine(AnswerRoutine());
        }

        private IEnumerator AnswerRoutine()
        {
            yield return new WaitForSeconds(resultDuration);

            quizManager.SelectAnswer(answerIndex);

            SetVisualColor(normalColor);

            if (visualTransform != null)
                visualTransform.localScale = defaultScale;

            isPressed = false;
        }

        private void SetVisualColor(Color color)
        {
            if (targetButtonImage != null)
                targetButtonImage.color = color;

            if (targetButtonRenderer != null)
                targetButtonRenderer.material.color = color;
        }
    }
}