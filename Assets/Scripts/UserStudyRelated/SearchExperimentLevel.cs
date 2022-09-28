using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using Random = UnityEngine.Random;

public class SearchExperimentLevel : MonoBehaviour
{
    public enum ExperimentLevelState
    { Idle, BeforeNextTrial, RunningTrial, Finished }

    private SearchExperimentTrial currentTrial;
    private Queue<SearchExperimentTrial> remainingTrials;
    private List<SearchExperimentTrial> completedTrials;

    private string levelName;

    [ReadOnly] public ExperimentLevelState state = ExperimentLevelState.Idle;
    [ReadOnly] public SelectionTechniqueManager.SelectionTechnique levelTechnique;
    [ReadOnly] public int levelDensity = -1;

    private Transform MiddleMarkerEmptyGameObject;

    private BoundaryCircleManager readinessCircleManager;

    private SelectionTechniqueManager slectionTechniqueDistributer;

    private TMP_Text experimentText;

    private List<Vector3> targetPositions;
    private int currentTargetPositionIdx = 0;

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

        slectionTechniqueDistributer = FindObjectOfType<SelectionTechniqueManager>();

        targetPositions = GetSearchPositions();
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

        currentTrial = remainingTrials.Dequeue();
        // check if it's a repeat trial
        if (currentTrial.type == SearchExperimentTrial.SearchExperimentTrialType.Repeat)
        {
            experimentText.text = "Repeat\nTarget\nSelection";
        }
        else
        {
            experimentText.text = "Search\nfor\nTarget";
            currentTargetPositionIdx++;
        }

        Vector3 xrOrigin = FindObjectOfType<XROrigin>().transform.position;
        currentTrial.distToTarget = Vector3.Distance(xrOrigin, targetPositions[currentTargetPositionIdx]);

        SearchExperimentTrial.targetInteractable = FindObjectOfType<SearchTargetInteractable>();
        currentTrial.StartTrial(targetPositions[currentTargetPositionIdx]);
        slectionTechniqueDistributer.clearCurrentTechnique(levelTechnique);

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

        print($"> {levelName}: search at first attempt: {firstAttemptSearchPercentage}, search time: {avgSearchTime}");
        print($"> {levelName}: select at first attempt: {firstAttemptSelectPercentage}, select time: {avgSelectTime}");
    }


    private List<Vector3> GetSearchPositions()
    {
        List<Vector3> searchPositions = new List<Vector3>
        {
            new Vector3(0.199000001f,1.36899996f,-6.91900015f),
            new Vector3(0.365999997f,2.09500003f,-5.13100004f),
            new Vector3(-0.23199999f,1.08500004f,-4.42799997f),
            new Vector3(-0.29499998f,3.1070001f,-4.03499985f),
            new Vector3(-0.569999993f,2.5999999f,-2.5f),
            new Vector3(0.720000029f,1.89999998f,-2.5f),
            new Vector3(-1.02499998f,1f,-2.5f),
            new Vector3(1.1799999f,1.13f,-0.109999999f),
            new Vector3(-1.91999996f,2.21000004f,-0.109999999f),
            new Vector3(-0.280000001f,2.21000004f,-0.109999999f),
            new Vector3(-0.280000001f,0.370000005f,-0.109999999f),
            new Vector3(1.02999997f,0.879999995f,1.48000002f),
            new Vector3(-2.99000001f,0.930000007f,1.48000002f),
            new Vector3(-2.01999998f,0.930000007f,0.709999979f),
            new Vector3(-0.347000003f,2.20600009f,1.4800000f),
            new Vector3(0.298999995f,1.63900006f,2.49399996f),
            new Vector3(1.12399995f,1.70500004f,1.48000002f),
            new Vector3(-0.152999997f,0.270000011f,1.53999996f),
            new Vector3(-1.65999997f,0.379999995f,1.53999996f),
            new Vector3(-0.629999995f,1.33000004f,3.31999993f),
            new Vector3(1.11699998f,2.11800003f,3.31999993f),
            new Vector3(-1.48000002f,2.1400001f,3.31999993f),
            new Vector3(-2.97000003f,2.01999998f,3.31999993f),
            new Vector3(-1.40400004f,2.13000011f,-5.92000008f),
            new Vector3(1.04700005f,1.90100002f,-5.92000008f),
            new Vector3(1.20799994f,2.046f,-3.51999998f),
            new Vector3(-0.633000016f,1.99100006f,-3.74000001f),
            new Vector3(0.375f,1.44299996f,-0.0430000015f),
            new Vector3(-2.1099999f,1.78199995f,-1.80999994f),
            new Vector3(-1.93900001f,1.11500001f,-5.2329998f)
        };

        searchPositions = searchPositions.OrderBy(a => Guid.NewGuid()).ToList();

        return searchPositions;
    }
}
