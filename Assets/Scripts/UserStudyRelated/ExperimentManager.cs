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
    [SerializeField] private float levelDuration = 10f;

    [SerializeField] private float pauseDuration = 10f;

    [Header("Current Level Status")]
    [ReadOnly] [SerializeField] private ExperimentState state = ExperimentState.Idle;

    [ReadOnly] [SerializeField] private string currentLevelName = "NONE";
    [ReadOnly] [SerializeField] private int numRemaininLevels = -1;

    [ReadOnly] [SerializeField] private float pauseTimeRemaining = -1f;

    private Queue<ExperimentLevel> remainingLevels;
    private List<ExperimentLevel> finishedLevels;
    private ExperimentLevel currentLevel;

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

        List<ExperimentLevel> levels = new List<ExperimentLevel>
        {
            gameObject.AddComponent<ExperimentLevel>(),
            //gameObject.AddComponent<ExperimentLevel>(),
        };

        foreach (ExperimentLevel level in levels)
        {
            level.levelTechnique = "tech1";
            level.levelDensity = "dens1";
            level.SetLevelDuration(levelDuration);
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
        currentLevel.StartLevel();
        state = ExperimentState.RunningLevel;
    }

    private void TransitionToPause()
    {
        pauseTimeRemaining = pauseDuration;
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

                break;

            case ExperimentState.BetweenLevels:
                pauseTimeRemaining -= Time.deltaTime;
                if (pauseTimeRemaining < 0)
                    TransitionToNextLevel();

                break;
        }
    }
}