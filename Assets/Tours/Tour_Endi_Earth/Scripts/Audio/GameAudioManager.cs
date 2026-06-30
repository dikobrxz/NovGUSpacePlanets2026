using UnityEngine;

namespace Tour_Endi_Earth
{
    /// <summary>
    /// Controls non-voice game sounds:
    /// button clicks, element placement, and beach ambient sounds.
    /// Beach ambience starts only after the player reaches the island stage.
    /// </summary>
    public class GameAudioManager : MonoBehaviour
    {
        public static GameAudioManager Instance { get; private set; }

        [Header("Tour")]
        [SerializeField] private EarthTourManager tourManager;

        [Header("One Shot SFX")]
        [SerializeField] private AudioClip buttonClickClip;
        [SerializeField] private AudioClip elementPlacedClip;

        [Header("Ambient Loops")]
        [SerializeField] private AudioClip seagullsClip;
        [SerializeField] private AudioClip seaBreezeClip;

        [Header("Volumes")]
        [Range(0f, 1f)]
        [SerializeField] private float buttonClickVolume = 0.7f;

        [Range(0f, 1f)]
        [SerializeField] private float elementPlacedVolume = 0.8f;

        [Range(0f, 1f)]
        [SerializeField] private float seagullsVolume = 0.12f;

        [Range(0f, 1f)]
        [SerializeField] private float seaBreezeVolume = 0.22f;

        [Header("Debug")]
        [SerializeField] private bool showDebugLogs = true;

        private AudioSource oneShotSource;
        private AudioSource seagullsSource;
        private AudioSource seaBreezeSource;

        private EarthTourStage lastStage;
        private bool ambientPlaying;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("GameAudioManager: another instance already exists.");
            }

            Instance = this;

            SetupAudioSources();
        }

        private void Start()
        {
            if (tourManager != null)
            {
                lastStage = tourManager.CurrentStage;
                UpdateAmbientForStage(lastStage);
            }
            else
            {
                StopAmbient();

                if (showDebugLogs)
                    Debug.LogWarning("GameAudioManager: TourManager is not assigned.");
            }
        }

        private void Update()
        {
            if (tourManager == null)
                return;

            if (tourManager.CurrentStage == lastStage)
                return;

            lastStage = tourManager.CurrentStage;
            UpdateAmbientForStage(lastStage);
        }

        private void SetupAudioSources()
        {
            oneShotSource = gameObject.AddComponent<AudioSource>();
            oneShotSource.playOnAwake = false;
            oneShotSource.loop = false;
            oneShotSource.spatialBlend = 0f;

            seagullsSource = gameObject.AddComponent<AudioSource>();
            seagullsSource.playOnAwake = false;
            seagullsSource.loop = true;
            seagullsSource.spatialBlend = 0f;
            seagullsSource.volume = seagullsVolume;

            seaBreezeSource = gameObject.AddComponent<AudioSource>();
            seaBreezeSource.playOnAwake = false;
            seaBreezeSource.loop = true;
            seaBreezeSource.spatialBlend = 0f;
            seaBreezeSource.volume = seaBreezeVolume;
        }

        private void UpdateAmbientForStage(EarthTourStage stage)
        {
            if (ShouldPlayBeachAmbient(stage))
                PlayAmbient();
            else
                StopAmbient();
        }

        private bool ShouldPlayBeachAmbient(EarthTourStage stage)
        {
            switch (stage)
            {
                case EarthTourStage.ElementColumns:
                case EarthTourStage.MatchingQuest:
                case EarthTourStage.Quiz:
                case EarthTourStage.End:
                    return true;

                case EarthTourStage.ShipIntro:
                case EarthTourStage.SurfaceIntro:
                default:
                    return false;
            }
        }

        public void PlayButtonClick()
        {
            PlayOneShot(buttonClickClip, buttonClickVolume, "button click");
        }

        public void PlayElementPlaced()
        {
            PlayOneShot(elementPlacedClip, elementPlacedVolume, "element placed");
        }

        public void PlayAmbient()
        {
            if (ambientPlaying)
                return;

            if (seagullsClip != null)
            {
                seagullsSource.clip = seagullsClip;
                seagullsSource.volume = seagullsVolume;
                seagullsSource.Play();
            }

            if (seaBreezeClip != null)
            {
                seaBreezeSource.clip = seaBreezeClip;
                seaBreezeSource.volume = seaBreezeVolume;
                seaBreezeSource.Play();
            }

            ambientPlaying = true;

            if (showDebugLogs)
                Debug.Log("GameAudioManager: beach ambient started.");
        }

        public void StopAmbient()
        {
            if (seagullsSource != null)
                seagullsSource.Stop();

            if (seaBreezeSource != null)
                seaBreezeSource.Stop();

            ambientPlaying = false;

            if (showDebugLogs)
                Debug.Log("GameAudioManager: beach ambient stopped.");
        }

        private void PlayOneShot(AudioClip clip, float volume, string soundName)
        {
            if (clip == null)
            {
                if (showDebugLogs)
                    Debug.Log("GameAudioManager: no clip for " + soundName);

                return;
            }

            if (oneShotSource == null)
                return;

            oneShotSource.PlayOneShot(clip, volume);

            if (showDebugLogs)
                Debug.Log("GameAudioManager: playing " + soundName);
        }
    }
}