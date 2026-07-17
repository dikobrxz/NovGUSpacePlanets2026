using UnityEngine;

namespace Tour_ENDI_TourStub6
{

    public class VRButton : MonoBehaviour
    {
        public UnityEngine.Events.UnityEvent onPress;
        private bool isHovered = false;

        void Start()
        {
            var interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
            interactable.hoverEntered.AddListener((args) => isHovered = true);
            interactable.hoverExited.AddListener((args) => isHovered = false);
            interactable.selectEntered.AddListener((args) =>
            {
                if (isHovered) onPress.Invoke();
            });
        }
    }

}