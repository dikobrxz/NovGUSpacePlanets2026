using Newtonsoft.Json.Bson;
using System.Drawing;
using UnityEditor.TerrainTools;
using UnityEngine;
using System.Collections;

public class StoryManager : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool useDebugStartStage = false;
    [SerializeField] private Stage debugStartStage = Stage.Start;

    [Header("Managers")]
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private QuizManager quizManager;

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
    [SerializeField] private GameObject rings;
    [SerializeField] private GameObject terrain;
    [SerializeField] private GameObject titan;
    [SerializeField] private GameObject cassini;
    [SerializeField] private GameObject transmitter;
    [SerializeField] private GameObject stormClouds;
    [SerializeField] private GameObject ship;

    [Header("Particle Fade")]
    [SerializeField] private ParticleFadeOut fogFade;
    [SerializeField] private ParticleFadeOut tornadoFade;

    [Header("Screen Fade")]
    [SerializeField] private ScreenFadeManager screenFadeManager;

    [Header("Movement")]
    [SerializeField] private OrbitMovement titanMovement;
    [SerializeField] private OrbitMovement cassiniMovement;

    [Header("Quest")]
    [SerializeField] private TransmitterQuest transmitterQuest;

    [Header("Hints")]
    [SerializeField] private ContinueInput continueInput;

    [Header("Look Hints")]
    [SerializeField] private LookHintTrigger titanLookHint;

    [Header("Quiz Ray")]
    [SerializeField] private GameObject rightRayInteractor;
    [SerializeField] private GameObject rightPokeInteractor;

    private bool quizFinished;

    public Stage CurrentStage { get; private set; }

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
        StartCoroutine(StoryRoutine());
    }

    private void PrepareStageForDebug(Stage stage)
    {
        HideAllObjects();

        switch (stage)
        {
            case Stage.Start:
                MovePlayer(startPoint);
                saturn.SetActive(true);
                ship.SetActive(true);
                break;

            case Stage.Atmosphere:
                MovePlayer(atmospherePoint);
                terrain.SetActive(true);
                stormClouds.SetActive(true);
                rings.SetActive(true);
                break;

            case Stage.Observation:
                MovePlayer(observationPoint);
                terrain.SetActive(true);
                break;

            case Stage.ResearchHistory:
                MovePlayer(observationPoint);
                terrain.SetActive(true);
                titan.SetActive(true);
                cassini.SetActive(true);
                break;

            case Stage.Quest:
                MovePlayer(questPoint);
                terrain.SetActive(true);
                transmitter.SetActive(true);
                break;

            case Stage.Return:
                MovePlayer(returnPoint);
                saturn.SetActive(true);
                break;

            case Stage.Quiz:
                MovePlayer(returnPoint);
                saturn.SetActive(true);
                ship.SetActive(true);
                break;

            case Stage.End:
                MovePlayer(returnPoint);
                saturn.SetActive(true);
                break;
        }
    }

    private IEnumerator StoryRoutine()
    {
        rightRayInteractor.SetActive(false);
        rightPokeInteractor.SetActive(false);

        if (useDebugStartStage)
        {
            PrepareStageForDebug(debugStartStage);

            switch (debugStartStage)
            {
                case Stage.Start:
                    yield return RunStart();
                    break;

                case Stage.Atmosphere:
                    yield return RunAtmosphere();
                    break;

                case Stage.Observation:
                    yield return RunObservation();
                    break;

                case Stage.ResearchHistory:
                    yield return RunResearchHistory();
                    break;

                case Stage.Quest:
                    yield return RunQuest();
                    break;

                case Stage.Return:
                    yield return RunReturn();
                    break;

                case Stage.Quiz:
                    yield return RunQuiz();
                    break;

                case Stage.End:
                    yield return RunEnd();
                    break;
            }

            yield break;
        }

        yield return RunStart();
        yield return RunAtmosphere();
        yield return RunObservation();
        yield return RunResearchHistory();
        yield return RunQuest();
        yield return RunReturn();
        yield return RunQuiz();
        yield return RunEnd();

        Debug.Log("Сценарий завершён");
    }

    private IEnumerator RunStart()
    {
        SetStage(Stage.Start);
        MovePlayer(startPoint);

        ship.SetActive(true);
        saturn.SetActive(true);

        yield return audioManager.PlayAndWait(AudioType.Start);

        yield return WaitForContinue("Нажмите маленькую кнопку на контроллере, чтобы продолжить.");

        yield return screenFadeManager.FadeOut();
    }

    private IEnumerator RunAtmosphere()
    {
        SetStage(Stage.Atmosphere);
        MovePlayer(atmospherePoint);

        saturn.SetActive(false);
        ship.SetActive(false);
        terrain.SetActive(true);
        stormClouds.SetActive(true);
        //sun.SetActive(true);
        rings.SetActive(true);

        yield return screenFadeManager.FadeIn();

        yield return audioManager.PlayAndWait(AudioType.Atmosphere);

        yield return WaitForContinue("Осмотритесь и нажмите маленькую кнопку на контроллере, чтобы продолжить.");
    }

    private IEnumerator RunObservation()
    {
        SetStage(Stage.Observation);

        MovePlayer(observationPoint);

        if (fogFade != null)
            StartCoroutine(fogFade.FadeOut());

        if (tornadoFade != null)
            StartCoroutine(tornadoFade.FadeOut());

        yield return audioManager.PlayAndWait(AudioType.Observation);

        titan.SetActive(true);

        titanMovement.StartMovement();

        yield return StartCoroutine(titanLookHint.CheckLookDirection());

        yield return WaitForContinue("Осмотритесь и нажмите маленькую кнопку на контроллере, чтобы продолжить.");
    }

    private IEnumerator RunResearchHistory()
    {
        /*titan.SetActive(true);

        titanMovement.StartMovement();*/

        SetStage(Stage.ResearchHistory);

        cassini.SetActive(true);

        if (cassiniMovement != null)
            cassiniMovement.StartMovement();

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
        yield return screenFadeManager.FadeOut();

        SetStage(Stage.Return);
        MovePlayer(returnPoint);

        HideAllObjects();
        ship.SetActive(true);
        saturn.SetActive(true);

        yield return screenFadeManager.FadeIn();

        yield return audioManager.PlayAndWait(AudioType.Return);

        yield return WaitForContinue("Нажмите маленькую кнопку на контроллере, чтобы перейти к финальному квизу.");
    }

    private IEnumerator RunQuiz()
    {
        SetStage(Stage.Quiz);

        rightRayInteractor.SetActive(true);
        rightPokeInteractor.SetActive(true);

        quizFinished = false;

        quizManager.StartQuiz();

        yield return new WaitUntil(() => quizFinished);

        if (quizManager.CorrectAnswers == 5)
        {
            yield return audioManager.PlayAndWait(AudioType.QuizPerfect);
        }
        else if (quizManager.CorrectAnswers >= 3)
        {
            yield return audioManager.PlayAndWait(AudioType.QuizGood);
        }
        else
        {
            yield return audioManager.PlayAndWait(AudioType.QuizBad);
        }
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
        
        Physics.SyncTransforms();
    }

    private void HideAllObjects()
    {
        saturn.SetActive(false);
        ship.SetActive(false);
        rings.SetActive(false);
        terrain.SetActive(false);
        titan.SetActive(false);
        cassini.SetActive(false);
        transmitter.SetActive(false);
        //sun.SetActive(false);
        stormClouds.SetActive(false);
    }

    private IEnumerator WaitForContinue(string hint)
    {
        yield return new WaitForSeconds(1.5f);

        continueInput.ResetPress();
        uiManager.ShowHint(hint);

        yield return new WaitUntil(() => continueInput.WasPressed);

        uiManager.HideTextPanel();
    }

    public void OnQuizFinished()
    {
        quizFinished = true;
    }
}
