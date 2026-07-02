using UnityEngine;

namespace Tour_Endi_Earth
{
    public struct VoiceRequest
    {
        public AudioClip Clip;
        public string Reason;

        public VoiceRequest(AudioClip clip, string reason)
        {
            Clip = clip;
            Reason = reason;
        }
    }
}