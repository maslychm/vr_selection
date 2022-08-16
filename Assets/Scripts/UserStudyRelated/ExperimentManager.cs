using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ExperimentManager : MonoBehaviour
{
    public enum ExperimentState
    { Idle, BetweenLevels, RunningLevel }

    [Header("Experiment Settings")]
    [SerializeField] private string subjectId = "-1";

    [SerializeField] private SelectionTechniqueManager.SelectionTechnique selectionTechnique;

    [Range(1, 100)]
    [SerializeField] private float levelDuration = 10f;

    [Range(1, 60)]
    [SerializeField] private float pauseBetweenLevelsDuration = 10f;

    [SerializeField] private int randomSeed = 1234;

    [Header("Current Level Status")]
    [ReadOnly][SerializeField] private ExperimentState state = ExperimentState.Idle;

    [ReadOnly][SerializeField] private int numRemaininLevels = -1;

    [ReadOnly][SerializeField] private float pauseTimeRemaining = -1f;

    private Queue<ExperimentLevel> remainingLevels;
    private List<ExperimentLevel> finishedLevels;
    private ExperimentLevel currentLevel;


    [SerializeField] private int countOfTrialsPerCurrentLvl = 0;

    private void Start()
    {
        ExperimentLogger.runTime = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
        ExperimentLogger.CreateLoggingDirectory(Application.streamingAssetsPath, "density_data");
    }

    public void ClearExperiment()
    {
        if (Application.IsPlaying(gameObject) && state != ExperimentState.Idle)
        { return; }

        // Remove all Experiment Levels that might have stayed from the editor
        gameObject.GetComponents<ExperimentLevel>().ToList().ForEach(x => DestroyImmediate(x));

        // Reset the states and other values that might have been modified through editor
        state = ExperimentState.Idle;
    }

    public void StartExperiment()
    {
        ClearExperiment();

        ExperimentLogger.subjectId = subjectId;

        List<ExperimentLevel> levels = new List<ExperimentLevel>();

        foreach (int densityLevel in LevelManager.densityLevelIntegers)
        {
            ExperimentLevel level = gameObject.AddComponent<ExperimentLevel>();

            level.levelTechnique = selectionTechnique;
            level.levelDensity = densityLevel;
            level.SetLevelDuration(levelDuration);

            levels.Add(level);
        }

        remainingLevels = new Queue<ExperimentLevel>(levels);
        finishedLevels = new List<ExperimentLevel>();

        print($"===> Experiment START <===");
        // we set the amount of trial to be 0 again 
        countOfTrialsPerCurrentLvl = 0;
        print($"Will run {remainingLevels.Count} levels");

        SetAllowSwitching(false);

        TransitionToPause();
    }

    private void TransitionToNextLevel()
    {
        if (currentLevel)
        {
            finishedLevels.Add(currentLevel);
        }

        if (remainingLevels.Count == 0)
        {
            SetAllowSwitching(true);
            currentLevel = null;
            state = ExperimentState.Idle;
            print("===> Experiment END <===");

            // reset the trial count 
            setCountOfTrialsToZero();
            
            return;
        }

        currentLevel = remainingLevels.Dequeue();
        currentLevel.StartLevel(randomSeed);
        state = ExperimentState.RunningLevel;
    }

    private void TransitionToPause()
    {
        pauseTimeRemaining = pauseBetweenLevelsDuration;
        state = ExperimentState.BetweenLevels;
    }

    private void SetAllowSwitching(bool value)
    {
        LevelManager.allowKeyLevelSwitching = value;
        SelectionTechniqueManager.allowKeySelectionTechniqueSwitching = value;
    }

    private void Update()
    {
        switch (state)
        {
            case ExperimentState.Idle:
                break;

            case ExperimentState.RunningLevel:
                if (currentLevel.state == ExperimentLevel.ExperimentLevelState.Finished)
                    TransitionToPause();

                numRemaininLevels = remainingLevels.Count;

                break;

            case ExperimentState.BetweenLevels:
                //pauseTimeRemaining -= Time.deltaTime;

                // we switch only if the number of trials per the current level has reached 10 trials
                if (countOfTrialsPerCurrentLvl == 10)
                    TransitionToNextLevel();

                break;
        }
    }

    public void incrementNumberfTrialsForCurrentLvl()
    {
        // don't increment if the count of trials is actually 10 or more
        // either level ended or if bigger than 10, then we need a fix 
        if (countOfTrialsPerCurrentLvl >= 10) { return; }
        countOfTrialsPerCurrentLvl += 1;
    }
    
    public int accessCountOfTrialsForCurrentLvL()
    {
        print($"The count of trials for the current level is {countOfTrialsPerCurrentLvl}");
        return countOfTrialsPerCurrentLvl;
    }
    
    public void setCountOfTrialsToZero()
    {
        print($"The count of trials after the current level is {countOfTrialsPerCurrentLvl}");
        countOfTrialsPerCurrentLvl = 0;
    }
}