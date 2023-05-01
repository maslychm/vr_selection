using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

public class SearchExperimentManager : MonoBehaviour
{
    public enum ExperimentState
    { Idle, BetweenLevels, RunningLevel }

    [Header("Experiment Settings")]
    [SerializeField] private string subjectId = "-1";

    [SerializeField] private SelectionTechniqueManager.SelectionTechnique selectionTechnique;

    [Range(0, 15)]
    [SerializeField] private float pauseBetweenLevelsDuration = 4f;

    [SerializeField] private int numTrialsPerLevel = 10;

    [SerializeField] private int randomSeed = 1234;

    [Header("Current Experiment Status")]
    [ReadOnly] public static ExperimentState state = ExperimentState.Idle;

    [ReadOnly][SerializeField] private int numRemainingLevels = -1;

    [ReadOnly][SerializeField] private float pauseTimeRemaining = -1f;

    private Queue<SearchExperimentLevel> remainingLevels;
    private List<SearchExperimentLevel> finishedLevels;
    private SearchExperimentLevel currentLevel;

    [SerializeField] private TMP_Text experimentText;
    public HideViewOfSpheresController Mimir;

    /*create some variables to hold the mask RF*/
    public GameObject maskRF;
    public bool maskRFon = false;

    private void Start()
    {
        maskRF = GameObject.Find("BlackMask");
        maskRFon = maskRF.activeInHierarchy;
        if (maskRFon == false)
        {
            SearchExperimentLogger.softwareStartTime = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
            SearchExperimentLogger.CreateLoggingDirectory(Application.streamingAssetsPath, "density_data");
        }
        else
        {
            SearchExperimentLogger.softwareStartTime = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
            SearchExperimentLogger.CreateLoggingDirectory(Application.streamingAssetsPath, "density_data_WITHRF_BlackMask");
        }
    }

    public void ClearExperiment()
    {
        if (Application.IsPlaying(gameObject) && state != ExperimentState.Idle)
        { return; }

        // Remove all Experiment Levels that might have stayed from the editor
        gameObject.GetComponents<SearchExperimentLevel>().ToList().ForEach(x => DestroyImmediate(x));

        // Reset the states and other values that might have been modified through editor
        state = ExperimentState.Idle;
    }

    public void StartExperiment()
    {
        if (state != ExperimentState.Idle) { return; }
        ClearExperiment();

        SearchExperimentLogger.subjectId = subjectId;

        List<SearchExperimentLevel> levels = new List<SearchExperimentLevel>();
        foreach (int densityLevel in LevelManager.densityLevelIntegers)
        {
            SearchExperimentLevel level = gameObject.AddComponent<SearchExperimentLevel>();
            level.levelTechnique = selectionTechnique;
            level.levelDensity = densityLevel;
            levels.Add(level);
        }

        // shuffle the levels using linq
        levels = levels.OrderBy(a => Guid.NewGuid()).ToList();

        remainingLevels = new Queue<SearchExperimentLevel>(levels);
        finishedLevels = new List<SearchExperimentLevel>();

        if (maskRFon) Debug.Log("Mask RF is applied for this participant"); else Debug.Log("Mask RF OFF FOR THIS PARTICIPANT");

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

                if (currentLevel.state == SearchExperimentLevel.ExperimentLevelState.Finished)
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