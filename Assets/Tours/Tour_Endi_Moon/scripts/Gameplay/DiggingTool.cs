using UnityEngine;

namespace Tour_Endi_Moon
{
    public class DiggingTool : MonoBehaviour
    {
        [Header("Минимальный интервал между касаниями одного объекта (сек)")]
        [SerializeField] private float hitCooldown = 0.25f;

        [Header("Частицы при ударе")]
        [SerializeField] private ParticleSystem digParticles;

        [Header("Звук копания")]
        [SerializeField] private AudioSource digAudioSource;
        [SerializeField] private AudioClip digClip;
        [Range(0f, 1f)]
        [SerializeField] private float digVolume = 0.4f;

        [Header("Вибрация контроллера")]
        [SerializeField] private float hapticAmplitude = 0.3f;
        [SerializeField] private float hapticDuration = 0.1f;

        private readonly System.Collections.Generic.Dictionary<int, float> lastHitTime = new();

        private void OnTriggerEnter(Collider other)
        {
            RegisterPileHit(other);
        }

        private void OnTriggerStay(Collider other)
        {
            RegisterPileHit(other);
        }

        private void RegisterPileHit(Collider other)
        {
            var pile = other.GetComponent<SandPile>();
            if (pile == null || pile.IsDug) return;

            float now = Time.time;
            int id = pile.GetInstanceID();
            if (lastHitTime.TryGetValue(id, out float prev) && now - prev < hitCooldown) return;

            lastHitTime[id] = now;

            PlayDigParticles(pile.transform.position);
            PlayDigSound();
            SendHaptic();

            pile.RegisterHit();
        }

        private void PlayDigParticles(Vector3 position)
        {
            if (digParticles == null) return;
            digParticles.transform.position = position;
            digParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            digParticles.Play();
        }

        private void PlayDigSound()
        {
            if (digAudioSource == null || digClip == null) return;
            digAudioSource.PlayOneShot(digClip, digVolume);
        }

        private void SendHaptic()
        {
            var controller = GetComponentInParent<
                UnityEngine.XR.Interaction.Toolkit.XRBaseController>();
            if (controller == null) return;
            controller.SendHapticImpulse(hapticAmplitude, hapticDuration);
        }
    }
}