using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// Controls the main stages of the Earth VR tour.
/// Enables and disables scene groups depending on the current tour stage.
/// </summary>
public class EarthTourManager : MonoBehaviour
{
    [Header("Current Stage")]
    [SerializeField] private EarthTourStage currentStage = EarthTourStage.ShipIntro;

    public EarthTourStage CurrentStage => currentStage;

    [Header("Stage Groups")]
    [SerializeField] private GameObject shipIntroGroup;
    [SerializeField] private GameObject surfaceIntroGroup;
    [SerializeField] private GameObject elementColumnsGroup;
    [SerializeField] private GameObject matchingQuestGroup;
    [SerializeField] private GameObject quizGroup;
    [SerializeField] private GameObject endGroup;

    [Header("Debug")]
    [SerializeField] private bool allowKeyboardDebug = true;
    [SerializeField] private Key nextStageKey = Key.N;

    [Header("Events")]
    public UnityEvent<EarthTourStage> onStageChanged;

    private void Start()
    {
        SetStage(currentStage);
    }

    private void Update()
    {
        if (!allowKeyboardDebug)
            return;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current[nextStageKey].wasPressedThisFrame)
        {
            Debug.Log("Debug next stage key pressed.");
            GoToNextStage();
        }
    }

    public void SetStage(EarthTourStage stage)
    {
        currentStage = stage;

        DisableAllStageGroups();

        switch (currentStage)
        {
            case EarthTourStage.ShipIntro:
                SetActive(shipIntroGroup, true);
                break;

            case EarthTourStage.SurfaceIntro:
                SetActive(surfaceIntroGroup, true);
                break;

            case EarthTourStage.ElementColumns:
                SetActive(elementColumnsGroup, true);
                break;

            case EarthTourStage.MatchingQuest:
                SetActive(elementColumnsGroup, true);
                SetActive(matchingQuestGroup, true);
                break;

            case EarthTourStage.Quiz:
                SetActive(quizGroup, true);
                break;

            case EarthTourStage.End:
                SetActive(endGroup, true);
                break;
        }

        onStageChanged?.Invoke(currentStage);
        Debug.Log($"Earth tour stage changed to: {currentStage}");
    }

    [ContextMenu("Go To Next Stage")]
    public void GoToNextStage()
    {
        switch (currentStage)
        {
            case EarthTourStage.ShipIntro:
                SetStage(EarthTourStage.SurfaceIntro);
                break;

            case EarthTourStage.SurfaceIntro:
                SetStage(EarthTourStage.ElementColumns);
                break;

            case EarthTourStage.ElementColumns:
                SetStage(EarthTourStage.MatchingQuest);
                break;

            case EarthTourStage.MatchingQuest:
                SetStage(EarthTourStage.Quiz);
                break;

            case EarthTourStage.Quiz:
                SetStage(EarthTourStage.End);
                break;

            case EarthTourStage.End:
                Debug.Log("Earth tour is already completed.");
                break;
        }
    }

    [ContextMenu("Set Stage Ship Intro")]
    private void DebugSetShipIntro()
    {
        SetStage(EarthTourStage.ShipIntro);
    }

    [ContextMenu("Set Stage Surface Intro")]
    private void DebugSetSurfaceIntro()
    {
        SetStage(EarthTourStage.SurfaceIntro);
    }

    [ContextMenu("Set Stage Element Columns")]
    private void DebugSetElementColumns()
    {
        SetStage(EarthTourStage.ElementColumns);
    }

    [ContextMenu("Set Stage Matching Quest")]
    private void DebugSetMatchingQuest()
    {
        SetStage(EarthTourStage.MatchingQuest);
    }

    [ContextMenu("Set Stage Quiz")]
    private void DebugSetQuiz()
    {
        SetStage(EarthTourStage.Quiz);
    }

    [ContextMenu("Set Stage End")]
    private void DebugSetEnd()
    {
        SetStage(EarthTourStage.End);
    }

    private void DisableAllStageGroups()
    {
        SetActive(shipIntroGroup, false);
        SetActive(surfaceIntroGroup, false);
        SetActive(elementColumnsGroup, false);
        SetActive(matchingQuestGroup, false);
        SetActive(quizGroup, false);
        SetActive(endGroup, false);
    }

    private void SetActive(GameObject targetObject, bool isActive)
    {
        if (targetObject != null)
            targetObject.SetActive(isActive);
    }
}