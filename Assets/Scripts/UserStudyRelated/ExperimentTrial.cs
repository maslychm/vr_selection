using UnityEngine;

public class ExperimentTrial
{
    public static ExperimentTrial activeTrial = null;
    public static TargetInteractable targetInteractable = null;

    public int trialIdx = 0;
    public int randObjIdx = 0;

    private bool targetWasClicked = false;
    private float startTime = 0f;
    private float completionTime = 0f;
    private int numberOfAttempts = 0;
    private Interactable replacedInteractable;

    // made them serialized just so in the editor we can debug if the files assigned are not he correct ones
    [SerializeField] private AudioSource incorrectSelectionSound;
    [SerializeField] private AudioSource validSelectionSound;

    public ExperimentTrial(int _trialIdx, int _randObjIdx)
    {
        trialIdx = _trialIdx;
        randObjIdx = _randObjIdx;
    }

    public void StartTrial(Interactable interactableToReplace)
    {

        // added this condition too to not start the trial iunless the user hovers over the circle first
        if (circleManager.wasHoveredOver == false)
            return;
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

        // collect the audio source and then play it 
        incorrectSelectionSound = Resources.Load("incorrectSelectionSound") as AudioSource;
        incorrectSelectionSound.Play();
        Debug.Log("Non-t was hit");
        numberOfAttempts += 1;
    }

    public void RecordTargetHit()
    {
        // collect the audio source and then play it 
        validSelectionSound = Resources.Load("validSelectionSound") as AudioSource;
        validSelectionSound.Play();
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
        targetInteractable.GetComponent<Object_collected>().ResetGameObject();

        // at the end of the trial we simply set back the circle as it was
        circleManager.wasHoveredOver = false;

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