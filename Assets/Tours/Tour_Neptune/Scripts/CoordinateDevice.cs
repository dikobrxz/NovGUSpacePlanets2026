using UnityEngine;
using TMPro;
using UnityEngine.Experimental.GlobalIllumination;
using System.Collections;
using UnityEngine.UI;

namespace Tour_ENDI_TourStub6
{

    public class CoordinateDevice : MonoBehaviour
    {
        [Header("UI")]
        public Transform needle;
        public TMP_Text displayText;
        public TMP_Text statusText;

        [Header("Звуки")]
        public AudioSource deviceAudio;
        public AudioClip clickSound;
        public AudioClip successSound;

        [Space, Header("Other")]
        [SerializeField] private GameObject _impactSound;
        [SerializeField] private GameObject _landingSound;
        [SerializeField] private GameObject _fxParticles;
        [SerializeField] private GameObject _landingLight;
        [SerializeField] private Light[] _displayLights;
        [SerializeField] private Animator _animation;
        [SerializeField] private AudioSource _audio;
        [SerializeField] private AudioClip _displaySound;
        [SerializeField] private GameObject _hint;
        [SerializeField] private GameObject _landCollider;
        [SerializeField] private Image[] _displays;

        private int[] targetCode = { 4, 7, 2 };
        private int[] currentCode = { 0, 0, 0 };
        private int activeSlot = 0;
        private bool missionComplete = false;
        private bool interactionEnabled = false;
        private Color _displayColor = new Color32(0, 53, 2, 255);

        private static int _displayKey = Animator.StringToHash("display");

        void Start()
        {
            //UpdateDisplay();
            if (statusText != null)
                statusText.text = "";

            //_displayColor = _displays[0].color;
        }

        // Вызывается из StoryManager после clip4
        public void EnableInteraction()
        {
            interactionEnabled = true;
            if (statusText != null)
                statusText.text = "Введите код: 4 - 7 - 2";
            UpdateDisplay();
            UpdateNeedle();
        }

        public void IncreaseValue()
        {
            if (missionComplete || !interactionEnabled) return;
            currentCode[activeSlot] = (currentCode[activeSlot] + 1) % 10;
            PlayClick();
            UpdateNeedle();
            UpdateDisplay();
        }

        public void DecreaseValue()
        {
            if (missionComplete || !interactionEnabled) return;
            currentCode[activeSlot] = (currentCode[activeSlot] + 9) % 10;
            PlayClick();
            UpdateNeedle();
            UpdateDisplay();
        }

        public void ToggleMode()
        {
            if (missionComplete || !interactionEnabled) return;
            activeSlot = (activeSlot + 1) % 3;
            PlayClick();
            UpdateNeedle();
            UpdateDisplay();
        }

        public void SendCoordinates()
        {
            if (missionComplete || !interactionEnabled) return;

            if (currentCode[0] == targetCode[0] &&
                currentCode[1] == targetCode[1] &&
                currentCode[2] == targetCode[2])
            {
                missionComplete = true;
                if (statusText != null)
                    statusText.text = "Код верный! Сигнал отправлен!";
                if (displayText != null)
                    displayText.text = "4  7  2\n>>> ОТПРАВЛЕНО <<<";
                if (deviceAudio != null && successSound != null)
                    deviceAudio.PlayOneShot(successSound);

                StoryManager sm = FindFirstObjectByType<StoryManager>();
                if (sm != null)
                    sm.OnMissionComplete();
            }
            else
            {
                if (statusText != null)
                    statusText.text = "Неверный код! Попробуй ещё";
                if (deviceAudio != null && clickSound != null)
                    deviceAudio.PlayOneShot(clickSound);
            }
        }

        void UpdateDisplay()
        {
            if (displayText != null)
            {
                string line = "";
                for (int i = 0; i < 3; i++)
                {
                    if (i == activeSlot)
                        line += "[" + currentCode[i] + "]";
                    else
                        line += " " + currentCode[i] + " ";
                    if (i < 2) line += "  ";
                }
                displayText.text = line;
            }
            statusText.text = "Введите код: 4 - 7 - 2";
        }

        void UpdateNeedle()
        {
            if (needle != null)
            {
                float angle = currentCode[activeSlot] * 36f;
                needle.localRotation = Quaternion.Euler(0, 0, angle);
            }
        }

        void PlayClick()
        {
            if (deviceAudio != null && clickSound != null)
                deviceAudio.PlayOneShot(clickSound);
        }

        public void LandingEvent()
        {
            _impactSound.SetActive(true);
            _fxParticles.SetActive(true);
            _landingLight.SetActive(false);
            _landingSound.SetActive(false);
            _landCollider.SetActive(false);

            StartCoroutine(DisplayCoroutine());
        }

        private IEnumerator DisplayCoroutine()
        {
            yield return new WaitForSeconds(4f);

            _animation.SetTrigger(_displayKey);
            _audio.PlayOneShot(_displaySound);
        }

        public void DisableHint()
        {
            _hint.SetActive(false);
        }

        public void Restar()
        {
            _impactSound.SetActive(false);
            _landingSound.SetActive(true);
            missionComplete = false;
            _fxParticles.SetActive(false);
            _landingLight.SetActive(true);
            displayText.text = "";
            statusText.text = "";
            currentCode[0] = currentCode[1] = currentCode[2] = 0;
            foreach (var light in _displayLights)
            {
                light.intensity = 0;
            }
            foreach (var display in _displays)
            {
                display.color = _displayColor;
            }
        }
    }

}