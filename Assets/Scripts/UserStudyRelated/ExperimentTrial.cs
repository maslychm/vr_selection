using UnityEngine;

public class ExperimentTrial
{
    public static ExperimentTrial activeTrial = null;
    public static TargetInteractable targetInteractable = null;
    public soundSystemHolder soundSystemHolder = GameObject.FindObjectOfType<soundSystemHolder>();

    public int trialIdx = 0;
    public int randObjIdx = 0;

    private bool targetWasClicked = false;
    private float startTime = 0f;
    private float completionTime = 0f;
    private int numberOfAttempts = 0;
    private Interactable replacedInteractable;

    public ExperimentTrial(in int trialIdx)
    {
        this.trialIdx = trialIdx;
    }

    public void StartTrial(in int randObjIdx, in Interactable interactableToReplace)
    {
        this.randObjIdx = randObjIdx;

        Debug.Log("-- Trial START --");

        replacedInteractable = interactableToReplace;

        // this will store the updated transform for every time we assign a new target
        TargetInteractable.SetReferenceTransformForCurrentTrial(replacedInteractable.transform);

        targetInteractable.transform.position = replacedInteractable.transform.position;
        targetInteractable.transform.rotation = replacedInteractable.transform.rotation;
        targetInteractable.transform.localScale = replacedInteractable.transform.localScale;

        replacedInteractable.GetComponent<Object_collected>().MoveOutsideReach();

        activeTrial = this;

        targetWasClicked = false;
        startTime = Time.unscaledTime;
        numberOfAttempts = 0;
    }

    public void RecordTargetMiss()
    {
        soundSystemHolder.incorrectPlay();
        Debug.Log("Non-t was hit");
        numberOfAttempts += 1;
    }

    public void RecordTargetHit()
    {
        soundSystemHolder.correctPlay();
        Debug.Log("Target was hit");
        numberOfAttempts += 1;
        targetWasClicked = true;
        completionTime = Time.unscaledTime;

        EndTrial();
    }

    public void EndTrial()
    {
        var fname = ExperimentLogger.LogTrial(this);
        Debug.Log($"Wrote results file: {fname}");
        activeTrial = null;
        Debug.Log("-- Trial END --");
        replacedInteractable.GetComponent<Object_collected>().ResetGameObject();
        //targetInteractable.GetComponent<Object_collected>().ResetGameObject();
        //targetInteractable.transform.position = new Vector3(targetInteractable.transform.position.x + 20.0f, targetInteractable.transform.position.y, targetInteractable.transform.position.z);
    }

    public bool WasSuccessful()
    {
        return targetWasClicked;
    }

    public int GetNumAttempts()
    {
        return numberOfAttempts;
    }

    public int GenNumSuccessful()
    {
        if (WasSuccessful()) return 1; return 0;
    }

    public float ComputeTrialTime()
    {
        if (!WasSuccessful())
            return 0;

        return completionTime - startTime;
    }

    public float ComputeTrialAccuracy()
    {
        if (!WasSuccessful())
            throw new System.Exception("Computing accuracy on unfinished Trial");

        return 1f / numberOfAttempts;
    }
}