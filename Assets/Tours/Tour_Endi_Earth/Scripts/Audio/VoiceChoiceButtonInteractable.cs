using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Physical XR button for VoiceChoiceMenu.
/// Put it on cube hitboxes near the menu buttons.
/// </summary>
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(XRSimpleInteractable))]
public class VoiceChoiceButtonInteractable : MonoBehaviour
{
    public enum ChoiceAction
    {
        ListenCurrentThenPlayNew,
        SkipCurrentAndPlayNew,
        SpeedUpCurrentThenPlayNew
    }

    [SerializeField] private VoiceChoiceMenu voiceChoiceMenu;
    [SerializeField] private ChoiceAction action;

    private XRSimpleInteractable interactable;
    private bool pressed;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        interactable.activated.AddListener(OnActivated);
        interactable.selectEntered.AddListener(OnSelected);
    }

    private void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.activated.RemoveListener(OnActivated);
            interactable.selectEntered.RemoveListener(OnSelected);
        }
    }

    private void OnActivated(ActivateEventArgs args)
    {
        Press();
    }

    private void OnSelected(SelectEnterEventArgs args)
    {
        Press();
    }

    private void Press()
    {
        if (pressed)
            return;

        pressed = true;

        if (GameAudioManager.Instance != null)
            GameAudioManager.Instance.PlayButtonClick();

        if (voiceChoiceMenu == null)
            return;

        switch (action)
        {
            case ChoiceAction.ListenCurrentThenPlayNew:
                voiceChoiceMenu.ListenCurrentThenPlayNew();
                break;

            case ChoiceAction.SkipCurrentAndPlayNew:
                voiceChoiceMenu.SkipCurrentAndPlayNew();
                break;

            case ChoiceAction.SpeedUpCurrentThenPlayNew:
                voiceChoiceMenu.SpeedUpCurrentThenPlayNew();
                break;
        }

        Invoke(nameof(ResetPress), 0.3f);
    }

    private void ResetPress()
    {
        pressed = false;
    }
}
