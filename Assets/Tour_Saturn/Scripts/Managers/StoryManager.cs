using Newtonsoft.Json.Bson;
using System.Drawing;
using UnityEditor.TerrainTools;
using UnityEngine;
using System.Collections;

public class StoryManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private UIManager uiManager;

    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("Checkpoints")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform atmospherePoint;
    [SerializeField] private Transform observationPoint;
    [SerializeField] private Transform questPoint;
    [SerializeField] private Transform returnPoint;

    [Header("Story Objects")]
    [SerializeField] private GameObject saturn;
    [SerializeField] private GameObject terrain;
    [SerializeField] private GameObject titan;
    [SerializeField] private GameObject cassini;
    [SerializeField] private GameObject transmitter;
    [SerializeField] private GameObject sun;
    [SerializeField] private GameObject stormClouds;

    [Header("Fog")]
    [SerializeField] private ParticleFogFade fogFade;

    [Header("Movement")]
    [SerializeField] private OrbitMovement titanMovement;
    [SerializeField] private OrbitMovement cassiniMovement;

    [Header("Quest")]
    [SerializeField] private TransmitterQuest transmitterQuest;

    [Header("Hints")]
    [SerializeField] private HintManager hintManager;
    [SerializeField] private ContinueInput continueInput;

    public Stage CurrentStage { get; private set; }

    public enum Stage
    {
        Start,
        Atmosphere,
        Observation,
        ResearchHistory,
        Quest,
        Return,
        End
    }

    public void StoryStart()
    {
        StartCoroutine(StoryRoutine());
    }

    private IEnumerator StoryRoutine()
    {
        HideAllObjects();

        yield return RunStart();
        yield return RunAtmosphere();
        yield return RunObservation();
        yield return RunResearchHistory();
        yield return RunQuest();
        yield return RunReturn();
        yield return RunEnd();

        Debug.Log("Сценарий завершён");
    }

    private IEnumerator RunStart()
    {
        SetStage(Stage.Start);
        MovePlayer(startPoint);

        saturn.SetActive(true);

        yield return audioManager.PlayAndWait(AudioType.Start);

        yield return WaitForContinue("Нажмите маленькую кнопку на контроллере, чтобы продолжить.");
    }

    private IEnumerator RunAtmosphere()
    {
        SetStage(Stage.Atmosphere);
        MovePlayer(atmospherePoint);

        saturn.SetActive(false);
        terrain.SetActive(true);
        stormClouds.SetActive(true);

        yield return audioManager.PlayAndWait(AudioType.Atmosphere);

        yield return WaitForContinue("Осмотритесь и нажмите маленькую кнопку на контроллере, чтобы продолжить.");
    }

    private IEnumerator RunObservation()
    {
        SetStage(Stage.Observation);

        MovePlayer(observationPoint);

        if (fogFade != null)
            yield return StartCoroutine(fogFade.FadeOut());

        yield return audioManager.PlayAndWait(AudioType.Observation);

        stormClouds.SetActive(false);
        sun.SetActive(true);
        titan.SetActive(true);
        saturn.SetActive(true);

        titanMovement.StartMovement();

        yield return WaitForContinue("Осмотритесь и нажмите маленькую кнопку на контроллере, чтобы продолжить.");
    }

    private IEnumerator RunResearchHistory()
    {
        SetStage(Stage.ResearchHistory);

        cassini.SetActive(true);

        yield return audioManager.PlayAndWait(AudioType.Research);

        yield return WaitForContinue("Осмотритесь и нажмите маленькую кнопку на контроллере, чтобы продолжить.");
    }

    private IEnumerator RunQuest()
    {
        SetStage(Stage.Quest);
        MovePlayer(questPoint);

        transmitter.SetActive(true);
        transmitterQuest.StartQuest();

        yield return audioManager.PlayAndWait(AudioType.Quest);

        yield return new WaitUntil(() => transmitterQuest.IsCompleted);
    }

    private IEnumerator RunReturn()
    {
        SetStage(Stage.Return);
        MovePlayer(returnPoint);

        HideAllObjects();
        saturn.SetActive(true);

        yield return audioManager.PlayAndWait(AudioType.Return);
    }

    private IEnumerator RunEnd()
    {
        SetStage(Stage.End);

        yield return audioManager.PlayAndWait(AudioType.End);
    }

    private void SetStage(Stage stage)
    {
        CurrentStage = stage;
        Debug.Log($"Текущая стадия: {CurrentStage}");
    }

    private void MovePlayer(Transform point)
    {
        player.SetPositionAndRotation(point.position, point.rotation);
    }

    private void HideAllObjects()
    {
        saturn.SetActive(false);
        terrain.SetActive(false);
        titan.SetActive(false);
        cassini.SetActive(false);
        transmitter.SetActive(false);
        sun.SetActive(false);
        stormClouds.SetActive(false);
    }

    private IEnumerator WaitForContinue(string hint)
    {
        yield return new WaitForSeconds(3f);

        continueInput.ResetPress();
        uiManager.ShowHint(hint);

        yield return new WaitUntil(() => continueInput.WasPressed);

        uiManager.HideTextPanel();
    }

    /*[Header("Managers")]
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private UIManager uiManager;

    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("Checkpoints")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform atmospherePoint;
    [SerializeField] private Transform observationPoint;
    [SerializeField] private Transform questPoint;
    [SerializeField] private Transform returnPoint;

    [Header("Story Objects")]
    [SerializeField] private GameObject saturn;
    [SerializeField] private GameObject terrain;
    [SerializeField] private GameObject titan;
    [SerializeField] private GameObject cassini;
    [SerializeField] private GameObject transmitter;
    [SerializeField] private GameObject sun;

    private int _index;
    public Stage CurrentStage;

    public enum Stage
    {
        Start,
        Atmosphere,
        Observation,
        ResearchHistory,
        Quest,
        Return,
        Quiz,
        End
    }

    public void StoryStart()
    {
        _index = 0;
        SetStage();

        GetCurrentStage();
    }

    public void NextStage()
    {
        _index++;

        if (_index >= System.Enum.GetValues(typeof(Stage)).Length)
        {
            Debug.Log("Конец сценария");
            return;
        }

        SetStage();
    }

    public void SetStage()
    {
        CurrentStage = (Stage)_index;

        GetCurrentStage();

        //HideObjects();

        switch (CurrentStage)
        {
            case Stage.Start:
                MovePlayer(startPoint);

                saturn.SetActive(true);

                //audioManager.PlayStartAudio();
                //uiManager.ShowStartScreen();

                Invoke(nameof(NextStage), 5f);

                break;

            case Stage.Atmosphere:
                MovePlayer(atmospherePoint);

                saturn.SetActive(false);
                terrain.SetActive(true);

                //audioManager.PlayAtmosphereAudio();

                Invoke(nameof(NextStage), 5f);

                break;

            case Stage.Observation:
                MovePlayer(observationPoint);

                sun.SetActive(true);
                titan.SetActive(true);

                //audioManager.PlayObservationAudio();

                Invoke(nameof(NextStage), 5f);

                break;

            case Stage.ResearchHistory:
                cassini.SetActive(true);

                //audioManager.PlayResearchAudio();

                Invoke(nameof(NextStage), 5f);

                break;

            case Stage.Quest:
                MovePlayer(questPoint);

                transmitter.SetActive(true);

                //audioManager.PlayQuestAudio();
                //uiManager.ShowQuestUI();

                Invoke(nameof(NextStage), 5f);

                break;

            case Stage.Return:
                MovePlayer(returnPoint);

                HideObjects();
                saturn.SetActive(true);
                
                //audioManager.PlayReturnAudio();

                Invoke(nameof(NextStage), 5f);

                break;

            case Stage.Quiz:
                //uiManager.ShowQuiz();

                Invoke(nameof(NextStage), 5f);

                break;

            case Stage.End:
                //uiManager.ShowFinalScreen();
                //audioManager.PlayEndAudio();

                Invoke(nameof(NextStage), 5f);

                break;
        }
    }

    public void GetCurrentStage()
    {
        Debug.Log($"Текущая стадия: {CurrentStage}");
    }

    private void MovePlayer(Transform point)
    {
        player.position = point.position;
        player.rotation = point.rotation;
    }

    private void HideObjects()
    {
        terrain.SetActive(false);
        titan.SetActive(false);
        cassini.SetActive(false);
        transmitter.SetActive(false);
    }*/
}
