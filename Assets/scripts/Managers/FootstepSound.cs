using UnityEngine;

public class FootstepSound : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private AudioSource audioSource;
    
    [Header("Settings")]
    [SerializeField] private float stepInterval = 0.5f;
    [SerializeField] private float minSpeed = 0.5f;
    [SerializeField] private float volume = 0.6f;
    
    private float lastStepTime = 0f;
    private int currentSoundIndex = 0;
    private Vector3 lastPosition;
    private float currentSpeed = 0f;

    private void Start()
    {
        lastPosition = transform.position;
        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f; // 3D звук
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.maxDistance = 20f;
            audioSource.volume = volume;
        }
    }

    private void Update()
    {
        currentSpeed = Vector3.Distance(transform.position, lastPosition) / Time.deltaTime;
        lastPosition = transform.position;
        
        if (currentSpeed >= minSpeed)
        {
            if (Time.time - lastStepTime >= stepInterval)
            {
                PlayFootstep();
                lastStepTime = Time.time;
            }
        }
    }

    private void PlayFootstep()
    {
        if (footstepSounds == null || footstepSounds.Length == 0) return;
        
        AudioClip clip = footstepSounds[currentSoundIndex];
        currentSoundIndex = (currentSoundIndex + 1) % footstepSounds.Length;
        
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }

    public void SetSurfaceSounds(AudioClip[] sounds)
    {
        footstepSounds = sounds;
    }
}