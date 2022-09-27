using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SearchExperimentLevel : MonoBehaviour
{
    public enum ExperimentLevelState
    { Idle, BeforeNextTrial, RunningTrial, Finished }

    private SearchExperimentTrial currentTrial;
    private Queue<SearchExperimentTrial> remainingTrials;
    private List<SearchExperimentTrial> completedTrials;

    private string levelName;

    private List<Interactable> levelInteractables;

    [ReadOnly] public ExperimentLevelState state = ExperimentLevelState.Idle;
    [ReadOnly] public SelectionTechniqueManager.SelectionTechnique levelTechnique;
    [ReadOnly] public int levelDensity = -1;

    private Transform MiddleMarkerEmptyGameObject;

    private BoundaryCircleManager readinessCircleManager;

    private SelectionTechniqueManager selectyionTechniqueDistributer;

    private int priorRandomIndex = -1;

    private TMP_Text experimentText;

    private void Start()
    {
        MiddleMarkerEmptyGameObject = GameObject.Find("HalfwayMarker").transform;
        if (MiddleMarkerEmptyGameObject == null)
        { Debug.LogError("HalfwayMarker object was not found!"); }

        readinessCircleManager = FindObjectOfType<BoundaryCircleManager>();
        if (readinessCircleManager == null)
        { Debug.LogError("Did not find boundary circle manager"); };

        experimentText = GameObject.Find("ExperimentTextTMP").GetComponent<TMP_Text>();
        if (experimentText == null)
        { Debug.LogError("Did not find experiment text"); }

        selectyionTechniqueDistributer = FindObjectOfType<SelectionTechniqueManager>();
    }

    public void StartLevel(in int randomSeed, in int numTrialsPerlevel)
    {
        print("-> Level START <-");
        levelName = $"{levelTechnique}_dens{levelDensity}";

        GetComponent<LevelManager>().DisableAllLevels();
        GetComponent<LevelManager>().EnableDensityLevel(levelDensity);
        GetComponent<SelectionTechniqueManager>().ActivateTechnique(levelTechnique);

        SearchExperimentLogger.densityLevel = levelDensity;
        SearchExperimentLogger.selectionTechnique = levelTechnique;

        levelInteractables = FindObjectsOfType<Interactable>()
            .ToList()
            .Where(x => x.isActiveAndEnabled && !x.GetComponent<SearchTargetInteractable>()
            && (x.gameObject.transform.position.z > MiddleMarkerEmptyGameObject.position.z))
            .ToList();
        SearchExperimentTrial.targetInteractable = FindObjectOfType<SearchTargetInteractable>();

        Random.InitState(randomSeed);

        currentTrial = null;
        completedTrials = new List<SearchExperimentTrial>();
        remainingTrials = new Queue<SearchExperimentTrial>();

        for (int trialIdx = 1; trialIdx < numTrialsPerlevel + 1; trialIdx++)
        {
            remainingTrials.Enqueue(new SearchExperimentTrial(trialIdx, SearchExperimentTrial.SearchExperimentTrialType.Search));
            remainingTrials.Enqueue(new SearchExperimentTrial(trialIdx, SearchExperimentTrial.SearchExperimentTrialType.Repeat));
        }

        TransitionToBeforeTrial();
    }

    public void EndLevel()
    {
        state = ExperimentLevelState.Finished;
        ComputeLevelStats();

        print("-> Level END <-");

        GetComponent<LevelManager>().DisableAllLevels();
        GetComponent<SelectionTechniqueManager>().DisableAllTechniques();
    }

    private void TransitionToNextTrial()
    {
        FindObjectOfType<GrabbingHand>().ClearGrabbed();

        Vector3 xrOrigin = FindObjectOfType<XROrigin>().transform.position;
        float distToObj;

        currentTrial = remainingTrials.Dequeue();
        // check if it's a repeat trial
        if (currentTrial.type == SearchExperimentTrial.SearchExperimentTrialType.Repeat)
        {
            Interactable interactableToReplace = levelInteractables[priorRandomIndex];
            distToObj = Vector3.Distance(interactableToReplace.transform.position, xrOrigin);
            currentTrial.StartTrial(priorRandomIndex, interactableToReplace);
        }
        else
        {
            int randIdx = Random.Range(0, levelInteractables.Count + 1);
            while (priorRandomIndex == randIdx)
            {
                print("Re-randomizing random object index because same as last one was drawn.");
                randIdx = Random.Range(0, levelInteractables.Count + 1);
            }
            Interactable interactableToReplace = levelInteractables[randIdx];
            distToObj = Vector3.Distance(interactableToReplace.transform.position, xrOrigin);
            currentTrial.StartTrial(randIdx, interactableToReplace);
            priorRandomIndex = randIdx;
        }

        currentTrial.distToTarget = distToObj;

        selectyionTechniqueDistributer.clearCurrentTechnique(levelTechnique);

        if (currentTrial.type == SearchExperimentTrial.SearchExperimentTrialType.Repeat)
        { experimentText.text = "Repeat\nTarget\nSelection"; }
        else
        { experimentText.text = "Search\nfor\nTarget"; }

        state = ExperimentLevelState.RunningTrial;
    }

    private void TransitionToBeforeTrial()
    {
        if (currentTrial != null)
        {
            completedTrials.Add(currentTrial);
        }

        if (remainingTrials.Count == 0)
        {
            EndLevel();
            return;
        }

        readinessCircleManager.SetWaitForUserReady();

        if (remainingTrials.Peek().type == SearchExperimentTrial.SearchExperimentTrialType.Search)
            experimentText.text = "Start\nSearch";
        else
            experimentText.text = "Start\nRepeat";

        state = ExperimentLevelState.BeforeNextTrial;
    }

    private void Update()
    {
        switch (state)
        {
            case ExperimentLevelState.Idle:
            case ExperimentLevelState.Finished:
                break;

            case ExperimentLevelState.BeforeNextTrial:
                if (readinessCircleManager.UserConfirmedReadiness())
                    TransitionToNextTrial();
                break;

            case ExperimentLevelState.RunningTrial:
                if (currentTrial.WasCompleted())
                    TransitionToBeforeTrial();

                break;
        }
    }

    private void ComputeLevelStats()
    {
        float numTrials = completedTrials.Count / 2;

        int numFirstAttemptSearchTrials = 0, numFirstAttemptSelectTrials = 0;
        float totalSearchTime = 0, totalSelectTime = 0;

        foreach (SearchExperimentTrial et in completedTrials)
        {
            if (et.SuccessAtFirstAttempt())
            {
                if (et.type == SearchExperimentTrial.SearchExperimentTrialType.Search)
                    numFirstAttemptSearchTrials += 1;
                else
                    numFirstAttemptSelectTrials += 1;
            }

            if (et.type == SearchExperimentTrial.SearchExperimentTrialType.Search)
                totalSearchTime += et.ComputeTrialTime();
            else
                totalSelectTime += et.ComputeTrialTime();
        }

        float firstAttemptSearchPercentage = (float)numFirstAttemptSearchTrials / (float)numTrials;
        float firstAttemptSelectPercentage = (float)numFirstAttemptSelectTrials / (float)numTrials;

        float avgSearchTime = (float)totalSearchTime / (float)numTrials;
        float avgSelectTime = (float)totalSelectTime / (float)numTrials;

        print($"> {levelName}: search at first attempt: {firstAttemptSearchPercentage}, search time: {totalSearchTime}");
        print($"> {levelName}: select at first attempt: {firstAttemptSelectPercentage}, select time: {totalSelectTime}");
    }

}
