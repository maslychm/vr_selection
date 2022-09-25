using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// For replicable randomness: https://docs.unity3d.com/ScriptReference/Random-state.html
/// </summary>
public class ExperimentLevel : MonoBehaviour
{
    public enum ExperimentLevelState
    { Idle, BeforeNextTrial, RunningTrial, Finished }

    private ExperimentTrial currentTrial;
    private Queue<ExperimentTrial> remainingTrials;
    private List<ExperimentTrial> completedTrials;

    private string levelName;

    private List<Interactable> levelInteractables;

    // Mostly for editor display purposes
    [ReadOnly] public ExperimentLevelState state = ExperimentLevelState.Idle;

    [ReadOnly] public SelectionTechniqueManager.SelectionTechnique levelTechnique;
    [ReadOnly] public int levelDensity = -1;

    // this will be used as the one to determine where the target object swappers will be placed
    // should be assigned from the editor to guarantee a O(1) access (no need for linear approach
    // comparison)
    private GameObject MiddleMarkerEmptyGameObject;

    private BoundaryCircleManager readinessCircleManager;

    private SelectionTechniqueManager selectyionTechniqueDistributer;

    private int priorRandomIndex = -1;

    private void Start()
    {
        MiddleMarkerEmptyGameObject = GameObject.Find("HalfwayMarker");
        if (MiddleMarkerEmptyGameObject == null)
        { Debug.LogError("HalfwayMarker object was not found!"); }

        readinessCircleManager = FindObjectOfType<BoundaryCircleManager>();
        if (readinessCircleManager == null)
        { Debug.LogError("Did not find boundary circle manager"); };

        selectyionTechniqueDistributer = FindObjectOfType<SelectionTechniqueManager>();
    }

    public void StartLevel(in int randomSeed, in int numTrialsPerLevel)
    {
        Scene scene = SceneManager.GetActiveScene();
        print("-> Level START <-");

        //
        // Set level settings
        //

        levelName = $"{levelTechnique}_dens{levelDensity}";

        GetComponent<LevelManager>().DisableAllLevels();

        GetComponent<LevelManager>().EnableDensityLevel(levelDensity);
        GetComponent<SelectionTechniqueManager>().ActivateTechnique(levelTechnique);

        ExperimentLogger.densityLevel = levelDensity;
        ExperimentLogger.selectionTechnique = levelTechnique;

        // only pick the gameObject Spheres that are positioned in the spawn area
        // the second half within the clutter zone across levels.
        levelInteractables = FindObjectsOfType<Interactable>()
            .ToList()
            .Where(x => x.isActiveAndEnabled && !x.GetComponent<TargetInteractable>()
            && (x.gameObject.transform.position.z > MiddleMarkerEmptyGameObject.transform.position.z))
            .ToList();
        ExperimentTrial.targetInteractable = FindObjectOfType<TargetInteractable>();

        // if (!scene.name.Contains("SearchTask"))
        {
            ExperimentTrial.targetInteractable.interactionOutline.enabled = false;
            ExperimentTrial.targetInteractable.targetOutline.enabled = true;
        }
        Random.InitState(randomSeed);

        //
        // Initialize trials
        //

        currentTrial = null;
        completedTrials = new List<ExperimentTrial>();
        remainingTrials = new Queue<ExperimentTrial>();

        for (int trialIdx = 1; trialIdx < numTrialsPerLevel + 1; trialIdx++)
        {
            remainingTrials.Enqueue(new ExperimentTrial(trialIdx));
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

        // clear the current held components in the technique before the next triel

        int randIdx = Random.Range(0, levelInteractables.Count + 1);
        while (priorRandomIndex == randIdx)
        {
            print("Re-randomizing random object index because same as last one was drawn.");
            randIdx = Random.Range(0, levelInteractables.Count + 1);
        }
        priorRandomIndex = randIdx;

        Interactable interactableToReplace = levelInteractables[randIdx];

        currentTrial = remainingTrials.Dequeue();

        currentTrial.StartTrial(randIdx, interactableToReplace);
        selectyionTechniqueDistributer.clearCurrentTechnique(levelTechnique);
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
                if (currentTrial.WasSuccessful())
                    TransitionToBeforeTrial();

                break;
        }
    }

    private void ComputeLevelStats()
    {
        float numTrials = completedTrials.Count;

        float numSuccessfulTrials = 0;
        float totalSuccessfulTime = 0;
        float totalAttempts = 0;

        foreach (ExperimentTrial et in completedTrials)
        {
            if (et.WasSuccessful())
            {
                totalSuccessfulTime += et.ComputeTrialTime();
                numSuccessfulTrials += 1f;
            }
            totalAttempts += et.GetNumAttempts();
        }

        float accuracy = numSuccessfulTrials / totalAttempts;
        float avgTime = totalSuccessfulTime / numTrials;

        print($"> {levelName}: acc: {accuracy}, time: {avgTime}, attempts: {totalAttempts}");
    }
}