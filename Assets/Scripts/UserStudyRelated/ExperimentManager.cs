using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExperimentManager : MonoBehaviour
{
    public class Level
    {
    }

    public enum ExperimentState { Idle, RunningLevel, BetweenLevels }

    [SerializeField] private float levelDuration = 1f;
    [SerializeField] private float pauseDuration = 1f;

    private float levelDurationCountdown = -1f;
    private float pauseDurationCountdown = -1f;

    private ExperimentState state = ExperimentState.Idle;

    private Queue<Level> remainingLevels;
    private List<Level> finishedLevels;
    private Level currentLevel;

    public void StartExperiment()
    {
        List<Level> levels = new List<Level>
        {
            new Level(),
            new Level()
        };
        var rnd = new System.Random();
        levels = levels.OrderBy(i => Guid.NewGuid()).ToList();

        remainingLevels = new Queue<Level>(levels);
        finishedLevels = new List<Level>();

        print($"===> Experiment START <===");
        print($"Will run {remainingLevels.Count} levels");

        StartNextLevel();
    }

    private void FinishExperiment()
    {
        print("===> Experiment END <===");
        state = ExperimentState.Idle;
    }

    private void StartNextLevel()
    {
        if (remainingLevels.Count == 0)
        {
            FinishExperiment();
            return;
        }

        print("-> Level START <-");
        currentLevel = remainingLevels.Dequeue();
        state = ExperimentState.RunningLevel;
        levelDurationCountdown = levelDuration;
    }

    private void EndLevel()
    {
        print("-> Level END <-");
        finishedLevels.Add(currentLevel);
        currentLevel = null;
        state = ExperimentState.BetweenLevels;
        pauseDurationCountdown = pauseDuration;
    }

    private void Update()
    {
        switch (state)
        {
            case ExperimentState.Idle:
                break;

            case ExperimentState.RunningLevel:
                //print($"RUNNING: {Time.deltaTime}");
                levelDurationCountdown -= Time.deltaTime;
                if (levelDurationCountdown < 0)
                    EndLevel();

                break;

            case ExperimentState.BetweenLevels:
                //print($"BETWEEN: {Time.deltaTime}");
                pauseDurationCountdown -= Time.deltaTime;
                if (pauseDurationCountdown < 0)
                    StartNextLevel();

                break;
        }
    }
}