using UnityEngine;

public class SearchExperimentTrial
{
    public enum SearchExperimentTrialType { Search, Repeat }

    public SearchExperimentTrialType type;

    public static SearchExperimentTrial activeTrial = null;
    public static SearchTargetInteractable targetInteractable = null;
    public soundSystemHolder soundSystemHolder = GameObject.FindObjectOfType<soundSystemHolder>();

    public int trialIdx = 0;
    public int randObjIdx = 0;

    private bool targetWasClicked = false;

    private float trialStartTime = 0f;
    private float trialCompleteTime = 0f;

    private int numAttempts = 0;

    private Interactable replacedInteractable;

    public static bool isTrialOngoingNow = false;

    public float distToTarget = 0f;

    public SearchExperimentTrial(in int _trialIdx, SearchExperimentTrialType t)
    {
        this.trialIdx = _trialIdx;
        this.type = t;
    }

    public void StartTrial(in int randObjIdx, in Interactable interactableToReplace)
    {
        this.randObjIdx = randObjIdx;
        Debug.Log("-- Trial START --");
        isTrialOngoingNow = true;
        replacedInteractable = interactableToReplace;

        targetInteractable.OffHighlighting();

        SearchTargetInteractable.SetReferenceTransformForCurrentTrial(replacedInteractable.transform);

        targetInteractable.transform.position = replacedInteractable.transform.position;
        targetInteractable.transform.rotation = replacedInteractable.transform.rotation;
        targetInteractable.transform.localScale = replacedInteractable.transform.localScale;

        replacedInteractable.GetComponent<Object_collected>().MoveOutsideReach();

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
        replacedInteractable.GetComponent<Object_collected>().ResetGameObject();

        isTrialOngoingNow = false;
        HandDistancesTraveled.FinishRecording();
    }

    public bool SuccessAtFirstAttempt() { return numAttempts == 1; }
    
    public bool WasCompleted() { return targetWasClicked; }

    public int GetNumAttempts() { return numAttempts; }

    public float ComputeTrialTime() { return trialCompleteTime - trialStartTime; }
}
