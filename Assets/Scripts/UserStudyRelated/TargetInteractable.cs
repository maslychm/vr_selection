using UnityEngine;

public class TargetInteractable : Interactable
{
    public cakeslice.Outline targetOutline = null;

    private static Vector3 originalScale;
    private static Vector3 originalPosition;
    private static Quaternion originalRotation;

    public static bool updateBack = false;

    public void ResetTargetForCurrentTrial()
    {
        print("Resetting target in the current trial");

        transform.position = originalPosition;
        transform.rotation = originalRotation;
        transform.localScale = originalScale;
        transform.SetParent(null);
    }

    public static void SetReferenceTransformForCurrentTrial(Transform _t)
    {
        originalPosition = _t.position;
        originalRotation = _t.rotation;
        originalScale = _t.localScale;
    }
}