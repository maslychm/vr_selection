using UnityEngine;

public class TargetInteractable : Interactable
{
    public cakeslice.Outline targetOutline = null;
    public static Transform TempTransformForCurrentTrial;
    public static bool updateBack = false;

    private void Start()
    {
        TempTransformForCurrentTrial = this.transform;
    }
    private void Update()
    {
        if (updateBack == true)
        {
            this.gameObject.transform.position = TempTransformForCurrentTrial.position;
            this.gameObject.transform.rotation = TempTransformForCurrentTrial.rotation;
            this.gameObject.transform.localScale = TempTransformForCurrentTrial.localScale;
            this.gameObject.transform.SetParent(TempTransformForCurrentTrial.parent);
        }

        updateBack = false;
    }
    // save the target current transform 
    public static void UpdateReferenceTransformOfTarget(Transform currentUpdate)
    {
        TempTransformForCurrentTrial = currentUpdate;
    }

    // reset target object 
    public static void UpdateTransformOfTarget()
    {
        updateBack = true;
    }
}