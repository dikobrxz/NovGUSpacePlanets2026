using TMPro;
using UnityEngine;

namespace Tour_ENDI_TourStub3
{

    public class CanvasWorldHint : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text hintText;
        [SerializeField] private GameObject hintPanel;

        void Start()
        {
            HideHint();
        }

        public void ShowHint(string text)
        {
            if (hintPanel != null)
            {
                hintPanel.SetActive(true);
                hintText.text = text;
            }
        }

        public void HideHint()
        {
            if (hintPanel != null)
            {
                hintPanel.SetActive(false);
            }
        }

        public void DestroyHint()
        {
            Destroy(gameObject);
        }
    }

}