using UnityEngine;

public class ExperimentTrial
{
    public static ExperimentTrial activeTrial = null;

    private bool targetWasClicked = false;
    private float startTime = 0f;
    private float completionTime = 0f;
    private int numberOfAttempts = 0;

    public void StartTrial()
    {
        Debug.Log("-- Trial START --");
        activeTrial = this;

        targetWasClicked = false;
        startTime = Time.unscaledTime;
        numberOfAttempts = 0;
    }

    public void RecordTargetMiss()
    {
        Debug.Log("NON-Target was hit");
        numberOfAttempts += 1;
    }

    public void RecordTargetHit()
    {
        Debug.Log("Target was hit");
        numberOfAttempts += 1;
        targetWasClicked = true;
        completionTime = Time.unscaledTime;

        EndTrial();
    }

    public void EndTrial()
    {
        Debug.Log("-- Trial END --");
        activeTrial = null;
    }

    public bool WasSuccessful()
    {
        return targetWasClicked;
    }

    public int GetNumAttempts()
    {
        return numberOfAttempts;
    }

    public float ComputeTrialTime()
    {
        if (!WasSuccessful())
            throw new System.Exception("Computing accuracy on unfinished Trial");

        return completionTime - startTime;
    }

    public float ComputeTrialAccuracy()
    {
        if (!WasSuccessful())
            throw new System.Exception("Computing accuracy on unfinished Trial");

        return 1f / numberOfAttempts;
    }
}