using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    public static string selectionTechniqueName;

    [Range(1, 15)]
    [SerializeField] private float pauseBetweenLevelsDuration = 4f;

    [SerializeField] private int numTrialsPerLevel = 5;

    [SerializeField] private int randomSeed = 1234;

    [Header("Current Level Status")]
    [ReadOnly] public static ExperimentState state = ExperimentState.Idle;

    [ReadOnly][SerializeField] private int numRemainingLevels = -1;

    [ReadOnly][SerializeField] private float pauseTimeRemaining = -1f;

    private Queue<ExperimentLevel> remainingLevels;
    private List<ExperimentLevel> finishedLevels;
    private ExperimentLevel currentLevel;

    [SerializeField] private TMP_Text experimentText;
    public HideViewOfSpheresController Mimir;

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
        if (state != ExperimentState.Idle) { return; }
        ClearExperiment();

        ExperimentLogger.subjectId = subjectId;

        List<ExperimentLevel> levels = new List<ExperimentLevel>();
        //List<int> densityLevelTwo = new List<int> { 2 };
        foreach (int densityLevel in LevelManager.densityLevelIntegers)

        // iterate over two levels only 256 spheres
        //foreach (int densityLevel in densityLevelTwo)
        {
            ExperimentLevel level = gameObject.AddComponent<ExperimentLevel>();

            level.levelTechnique = selectionTechnique;

            level.levelDensity = densityLevel;

            levels.Add(level);
        }

        remainingLevels = new Queue<ExperimentLevel>(levels);
        finishedLevels = new List<ExperimentLevel>();

        print($"===> Experiment START <===");
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
            return;
        }

        currentLevel = remainingLevels.Dequeue();
        currentLevel.StartLevel(randomSeed, numTrialsPerLevel);
        state = ExperimentState.RunningLevel;
    }

    private void TransitionToPause()
    {
        if (remainingLevels.Count != 0)
            Mimir.ShowTheBarrier();
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

                numRemainingLevels = remainingLevels.Count;

                break;

            case ExperimentState.BetweenLevels:

                experimentText.text = $"Level ready in:\n{pauseTimeRemaining:0.#} s.";

                pauseTimeRemaining -= Time.deltaTime;
                if (pauseTimeRemaining < 0f)
                    TransitionToNextLevel();

                break;
        }
    }
}