using UnityEngine;

public class ExperimentTrial
{
    public static ExperimentTrial activeTrial = null;
    public static TargetInteractable targetInteractable = null;

    private bool targetWasClicked = false;
    private float startTime = 0f;
    private float completionTime = 0f;
    private int numberOfAttempts = 0;
    private Interactable replacedInteractable;

    public void StartTrial(Interactable interactableToReplace)
    {
        Debug.Log("-- Trial START --");

        replacedInteractable = interactableToReplace;

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
        Debug.Log("Non-t was hit");
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
        replacedInteractable.GetComponent<Object_collected>().ResetGameObject();
        targetInteractable.GetComponent<Object_collected>().ResetGameObject();
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