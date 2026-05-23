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
    [SerializeField] private GameObject sun;
    [SerializeField] private GameObject stormClouds;

    [Header("Particle Fade")]
    [SerializeField] private ParticleFadeOut fogFade;
    [SerializeField] private ParticleFadeOut tornadoFade;

    [Header("Movement")]
    [SerializeField] private OrbitMovement titanMovement;
    [SerializeField] private OrbitMovement cassiniMovement;

    [Header("Quest")]
    [SerializeField] private TransmitterQuest transmitterQuest;

    [Header("Hints")]
    //[SerializeField] private HintManager hintManager;
    [SerializeField] private ContinueInput continueInput;

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

    private IEnumerator StoryRoutine()
    {
        HideAllObjects();

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
        sun.SetActive(true);
        rings.SetActive(true);

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

        yield return WaitForContinue("Осмотритесь и нажмите маленькую кнопку на контроллере, чтобы продолжить.");
    }

    private IEnumerator RunResearchHistory()
    {
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
        SetStage(Stage.Return);
        MovePlayer(returnPoint);

        HideAllObjects();
        saturn.SetActive(true);

        yield return audioManager.PlayAndWait(AudioType.Return);

        yield return WaitForContinue("Нажмите маленькую кнопку на контроллере, чтобы перейти к финальному квизу.");
    }

    private IEnumerator RunQuiz()
    {
        SetStage(Stage.Quiz);

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
        rings.SetActive(false);
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

    public void OnQuizFinished()
    {
        quizFinished = true;
    }
}
