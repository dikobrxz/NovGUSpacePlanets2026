using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI hintText;       // Текст подсказки
    [SerializeField] private TextMeshProUGUI notificationText; // Текст уведомлений (сверху)

    [Header("Settings")]
    [SerializeField] private float notificationDuration = 3f;

    private void Awake()
    {
        Instance = this;
        if (hintText != null) hintText.gameObject.SetActive(false);
        if (notificationText != null) notificationText.gameObject.SetActive(false);
    }


    public static void ShowHint(string text)
    {
        if (Instance == null) return;
        if (Instance.hintText == null || Instance.hintText.Equals(null)) return;
        
        Instance.hintText.text = text;
        Instance.hintText.gameObject.SetActive(true);
    }

    public static void HideHint()
    {
        if (Instance == null) return;
        if (Instance.hintText == null || Instance.hintText.Equals(null)) return;
        
        Instance.hintText.gameObject.SetActive(false);
    }

    public static void ShowNotification(string text)
    {
        if (Instance == null) return;
        if (Instance.notificationText == null || Instance.notificationText.Equals(null)) return;
        
        Instance.notificationText.text = text;
        Instance.notificationText.gameObject.SetActive(true);
        Instance.Invoke(nameof(HideNotification), Instance.notificationDuration);
    }

    private void HideNotification()
    {
        if (notificationText == null || notificationText.Equals(null)) return;
        notificationText.gameObject.SetActive(false);
    }
}