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
    [SerializeField] private int subjectId = -1;

    [SerializeField] private SelectionTechniqueManager.SelectionTechnique selectionTechnique;

    [Range(1, 100)]
    [SerializeField] private float levelDuration = 10f;

    [Range(1, 60)]
    [SerializeField] private float pauseBetweenLevelsDuration = 10f;

    [SerializeField] private int randomSeed = 1234;

    [Header("Current Level Status")]
    [ReadOnly] [SerializeField] private ExperimentState state = ExperimentState.Idle;

    [ReadOnly] [SerializeField] private int numRemaininLevels = -1;

    [ReadOnly] [SerializeField] private float pauseTimeRemaining = -1f;

    private Queue<ExperimentLevel> remainingLevels;
    private List<ExperimentLevel> finishedLevels;
    private ExperimentLevel currentLevel;

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
        print($"Will run {remainingLevels.Count} levels");

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
            print("===> Experiment END <===");
            currentLevel = null;
            state = ExperimentState.Idle;
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
                pauseTimeRemaining -= Time.deltaTime;
                if (pauseTimeRemaining < 0)
                    TransitionToNextLevel();

                break;
        }
    }
}