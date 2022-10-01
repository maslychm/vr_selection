using UnityEngine;

public class SearchExperimentTrial
{
    public enum SearchExperimentTrialType
    { Search, Repeat }

    public SearchExperimentTrialType type;

    public static SearchExperimentTrial activeTrial = null;
    public static SearchTargetInteractable targetInteractable = null;
    public soundSystemHolder soundSystemHolder = GameObject.FindObjectOfType<soundSystemHolder>();

    public int trialIdx = 0;

    private bool targetWasClicked = false;

    private float trialStartTime = 0f;
    private float trialCompleteTime = 0f;

    private int numAttempts = 0;

    public static bool isTrialOngoingNow = false;

    public float distToTarget = 0f;

    public SearchExperimentTrial(in int _trialIdx, SearchExperimentTrialType t)
    {
        this.trialIdx = _trialIdx;
        this.type = t;
    }

    public void StartTrial(in Vector3 searchTargetPosition)
    {
        Debug.Log("-- Trial START --");
        isTrialOngoingNow = true;

        targetInteractable.OffHighlighting();
        targetInteractable.TeleportToPosition(searchTargetPosition);
        SearchTargetInteractable.SetReferenceTransformForCurrentTrial(targetInteractable.transform);

        activeTrial = this;
        targetWasClicked = false;
        trialStartTime = Time.unscaledTime;
        numAttempts = 0;

        HandDistancesTraveled.StartRecording();
    }

    public void RecordTargetMiss()
    {
        soundSystemHolder.incorrectPlay();
        Debug.Log("Non-t was hit");
        numAttempts += 1;
    }

    public void RecordTargetHit()
    {
        soundSystemHolder.correctPlay();
        Debug.Log("Target was hit");
        numAttempts += 1;
        targetWasClicked = true;
        trialCompleteTime = Time.unscaledTime;
        EndTrial();
    }

    public void EndTrial()
    {
        var fname = SearchExperimentLogger.LogTrial(this);
        Debug.Log($"Wrote results file: {fname}");
        activeTrial = null;
        Debug.Log("-- Trial End --");

        isTrialOngoingNow = false;
        HandDistancesTraveled.FinishRecording();
        //TargetAreaOutline.DisableSearchOutlineAroundPosition();
    }

    public bool SuccessAtFirstAttempt()
    { return numAttempts == 1; }

    public bool WasCompleted()
    { return targetWasClicked; }

    public int GetNumAttempts()
    { return numAttempts; }

    public float ComputeTrialTime()
    { return trialCompleteTime - trialStartTime; }
}