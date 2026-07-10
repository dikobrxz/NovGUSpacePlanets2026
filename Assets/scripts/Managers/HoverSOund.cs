using UnityEngine;
using UnityEngine.EventSystems;

public class HoverSound : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private AudioClip sound;
    private AudioSource audioSource;

    private void Awake() => audioSource = gameObject.AddComponent<AudioSource>();
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (sound != null) audioSource.PlayOneShot(sound);
    }
}