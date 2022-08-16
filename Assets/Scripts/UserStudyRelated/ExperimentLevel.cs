using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// For replicable randomness: https://docs.unity3d.com/ScriptReference/Random-state.html
///
/// </summary>
public class ExperimentLevel : MonoBehaviour
{
    public enum ExperimentLevelState
    { Idle, Running, Finished }

    private float levelDuration;
    private ExperimentTrial currentTrial = null;
    private List<ExperimentTrial> trialHistory = null;
    private string levelName;
    private Random.State lastUsedRandomState;
    private List<Interactable> levelInteractables;

    // Mostly for editor display purposes
    [ReadOnly] public ExperimentLevelState state = ExperimentLevelState.Idle;

    [ReadOnly] public SelectionTechniqueManager.SelectionTechnique levelTechnique;
    [ReadOnly] public int levelDensity = -1;

    [ReadOnly] [SerializeField] private float levelTimeRemaining = -1f;
    [ReadOnly] [SerializeField] private int numTrials = -1;


    // need to assign an experiemnt manager handler here
    [SerializeField] private ExperimentManager experimentManager;

    public void StartLevel(int randomSeed)
    {
        print("-> Level START <-");

        // first be sure that the count is 0 for the trials at the start
        experimentManager.setCountOfTrialsToZero();
        
        levelName = $"{levelTechnique}_dens{levelDensity}";
        levelTimeRemaining = levelDuration;
        numTrials = 0;
        currentTrial = null;
        trialHistory = new List<ExperimentTrial>();

        GetComponent<LevelManager>().DisableAllLevels();
        GetComponent<LevelManager>().EnableDensityLevel(levelDensity);
        GetComponent<SelectionTechniqueManager>().ActivateTechnique(levelTechnique);

        levelInteractables = FindObjectsOfType<Interactable>()
            .ToList()
            .Where(x => x.isActiveAndEnabled && !x.GetComponent<TargetInteractable>())
            .ToList();

        ExperimentTrial.targetInteractable = FindObjectOfType<TargetInteractable>();

        Random.InitState(randomSeed);

        ExperimentLogger.densityLevel = levelDensity;
        ExperimentLogger.selectionTechnique = levelTechnique;

        TransitionToNextTrial();
    }

    public void EndLevel()
    {
        state = ExperimentLevelState.Finished;
        currentTrial?.EndTrial();
        ComputeLevelStats();

        print("-> Level END <-");

        // after the end of the current level we set the count of trials to 0
        experimentManager.setCountOfTrialsToZero();
    }

    private void TransitionToNextTrial()
    {
        numTrials = trialHistory.Count;

        int randIdx = Random.Range(0, levelInteractables.Count + 1);
        Interactable interactableToReplace = levelInteractables[randIdx];

        currentTrial = new ExperimentTrial(numTrials + 1, randIdx);
        trialHistory.Add(currentTrial);
        currentTrial.StartTrial(interactableToReplace);

        state = ExperimentLevelState.Running;

        // now we increment the trial number 
        experimentManager.incrementNumberfTrialsForCurrentLvl();

    }

    private void Update()
    {
        switch (state)
        {
            case ExperimentLevelState.Idle:
            case ExperimentLevelState.Finished:
                break;

            case ExperimentLevelState.Running:

                if (currentTrial.WasSuccessful() && experimentManager.accessCountOfTrialsForCurrentLvL() < 10)
                    TransitionToNextTrial();

                //levelTimeRemaining -= Time.deltaTime;
                //if (levelTimeRemaining < 0)
                if(experimentManager.accessCountOfTrialsForCurrentLvL() >= 10)
                    EndLevel();

                break;
        }
    }

    private void ComputeLevelStats()
    {
        float numTrials = trialHistory.Count;

        float numSuccessfulTrials = 0;
        float totalSuccessfulTime = 0;
        float totalAttempts = 0;

        foreach (ExperimentTrial et in trialHistory)
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

    public void SetLevelDuration(float _levelDuration)
    {
        levelDuration = _levelDuration;
    }
}