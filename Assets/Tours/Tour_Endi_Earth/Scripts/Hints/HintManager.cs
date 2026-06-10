using TMPro;
using UnityEngine;

/// <summary>
/// Controls visual hints for the Earth tour.
/// Updates the current task text and the element progress board.
/// </summary>
public class HintManager : MonoBehaviour
{
    public static HintManager Instance { get; private set; }

    [Header("Tour")]
    [SerializeField] private EarthTourManager tourManager;

    [Header("Current Task UI")]
    [SerializeField] private TMP_Text currentTaskText;

    [Header("Progress UI")]
    [SerializeField] private GameObject progressBoardObject;
    [SerializeField] private TMP_Text progressText;

    [Header("Stage Messages")]
    [SerializeField] private string surfaceIntroMessage = "Осмотрите колонны";
    [SerializeField] private string elementColumnsMessage = "Найдите элементы на столе";
    [SerializeField] private string matchingQuestMessage = "Возьмите элемент и перенесите его на подходящий пьедестал";
    [SerializeField] private string quizMessage = "Повернитесь к викторине и выберите ответы";
    [SerializeField] private string endMessage = "Экскурсия завершена";

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private EarthTourStage lastStage;

    private bool firePlaced;
    private bool waterPlaced;
    private bool earthPlaced;
    private bool airPlaced;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("HintManager: another instance already exists. Check duplicates in the scene.");
        }

        Instance = this;
    }

    private void Start()
    {
        UpdateProgressText();

        if (tourManager == null)
        {
            Debug.LogWarning("HintManager: TourManager is not assigned.");
            return;
        }

        lastStage = tourManager.CurrentStage;
        UpdateHintForStage(lastStage);
    }

    private void Update()
    {
        if (tourManager == null)
            return;

        if (tourManager.CurrentStage == lastStage)
            return;

        lastStage = tourManager.CurrentStage;
        UpdateHintForStage(lastStage);
    }

    public void MarkElementPlaced(ElementType elementType)
    {
        switch (elementType)
        {
            case ElementType.Fire:
                firePlaced = true;
                break;

            case ElementType.Water:
                waterPlaced = true;
                break;

            case ElementType.Earth:
                earthPlaced = true;
                break;

            case ElementType.Air:
                airPlaced = true;
                break;
        }

        UpdateProgressText();
        SetCurrentTask($"Установлен элемент: {GetElementName(elementType)}");

        if (showDebugLogs)
            Debug.Log($"Hint progress updated: {elementType} placed.");
    }

    private void UpdateHintForStage(EarthTourStage stage)
    {
        switch (stage)
        {
            case EarthTourStage.SurfaceIntro:
                SetCurrentTask(surfaceIntroMessage);
                SetProgressBoardVisible(false);
                break;

            case EarthTourStage.ElementColumns:
                SetCurrentTask(elementColumnsMessage);
                SetProgressBoardVisible(false);
                break;

            case EarthTourStage.MatchingQuest:
                SetCurrentTask(matchingQuestMessage);
                SetProgressBoardVisible(true);
                UpdateProgressText();
                break;

            case EarthTourStage.Quiz:
                SetCurrentTask(quizMessage);
                SetProgressBoardVisible(false);
                break;

            case EarthTourStage.End:
                SetCurrentTask(endMessage);
                SetProgressBoardVisible(false);
                break;
        }

        if (showDebugLogs)
            Debug.Log($"Hint changed for stage: {stage}");
    }

    private void SetCurrentTask(string message)
    {
        if (currentTaskText == null)
        {
            Debug.LogWarning("HintManager: Current Task Text is not assigned.");
            return;
        }

        currentTaskText.text = message;
        currentTaskText.ForceMeshUpdate();
    }

    private void SetProgressBoardVisible(bool isVisible)
    {
        if (progressBoardObject != null)
            progressBoardObject.SetActive(isVisible);
    }

    private void UpdateProgressText()
    {
        if (progressText == null)
        {
            Debug.LogWarning("HintManager: Progress Text is not assigned.");
            return;
        }

        string progress =
            $"{GetMark(firePlaced)} Огонь\n" +
            $"{GetMark(waterPlaced)} Вода\n" +
            $"{GetMark(earthPlaced)} Земля\n" +
            $"{GetMark(airPlaced)} Воздух";

        progressText.text = progress;
        progressText.ForceMeshUpdate();
        Canvas.ForceUpdateCanvases();

        if (showDebugLogs)
            Debug.Log($"Progress text set to:\n{progress}");
    }

    private string GetMark(bool isPlaced)
    {
        return isPlaced ? "[+]" : "[ ]";
    }

    private string GetElementName(ElementType elementType)
    {
        switch (elementType)
        {
            case ElementType.Fire:
                return "Огонь";

            case ElementType.Water:
                return "Вода";

            case ElementType.Earth:
                return "Земля";

            case ElementType.Air:
                return "Воздух";

            default:
                return elementType.ToString();
        }
    }

    public void ShowCustomMessage(string message)
    {
        SetCurrentTask(message);
    }

    [ContextMenu("Test Mark Fire")]
    private void TestMarkFire()
    {
        MarkElementPlaced(ElementType.Fire);
    }

    [ContextMenu("Test Mark Water")]
    private void TestMarkWater()
    {
        MarkElementPlaced(ElementType.Water);
    }

    [ContextMenu("Test Mark Earth")]
    private void TestMarkEarth()
    {
        MarkElementPlaced(ElementType.Earth);
    }

    [ContextMenu("Test Mark Air")]
    private void TestMarkAir()
    {
        MarkElementPlaced(ElementType.Air);
    }


}