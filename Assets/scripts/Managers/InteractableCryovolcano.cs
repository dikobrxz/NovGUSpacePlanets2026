using UnityEngine;
using TMPro;

public class InteractableCryovolcano : MonoBehaviour
{
    [SerializeField] private Cryovolcano volcano;
    [SerializeField] private TextMeshProUGUI promptText; // Прямая ссылка на текст
    [SerializeField] private string interactionPrompt = "Нажмите E для активации";
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (promptText != null)
            {
                promptText.text = interactionPrompt;
                promptText.gameObject.SetActive(true);
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (promptText != null)
            {
                promptText.gameObject.SetActive(false);
            }
        }
    }
    
    public void OnInteract()
    {
        if (volcano != null)
            volcano.ActivateEruption();
    }
}