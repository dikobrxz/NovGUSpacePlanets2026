using TMPro;
using UnityEngine;

/// <summary>
/// Shows progress for the element placement quest.
/// Reads completion state directly from pedestal slots.
/// </summary>
public class ElementProgressBoard : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text progressText;

    [Header("Pedestal Slots")]
    [SerializeField] private ElementPedestalSlot fireSlot;
    [SerializeField] private ElementPedestalSlot waterSlot;
    [SerializeField] private ElementPedestalSlot earthSlot;
    [SerializeField] private ElementPedestalSlot airSlot;

    [Header("Marks")]
    [SerializeField] private string emptyMark = "[ ]";
    [SerializeField] private string completedMark = "[+]";

    [Header("Debug")]
    [SerializeField] private bool updateEveryFrame = true;
    [SerializeField] private bool showDebugLogs;

    private void OnEnable()
    {
        UpdateProgress();
    }

    private void Update()
    {
        if (updateEveryFrame)
            UpdateProgress();
    }

    public void UpdateProgress()
    {
        if (progressText == null)
        {
            Debug.LogWarning("ElementProgressBoard: Progress Text is not assigned.");
            return;
        }

        string progress =
            $"{GetMark(fireSlot)} Огонь\n" +
            $"{GetMark(waterSlot)} Вода\n" +
            $"{GetMark(earthSlot)} Земля\n" +
            $"{GetMark(airSlot)} Воздух";

        progressText.text = progress;
        progressText.ForceMeshUpdate();
        Canvas.ForceUpdateCanvases();

        if (showDebugLogs)
            Debug.Log($"Element progress board updated:\n{progress}");
    }

    private string GetMark(ElementPedestalSlot slot)
    {
        if (slot == null)
            return emptyMark;

        return slot.IsCompleted ? completedMark : emptyMark;
    }
}