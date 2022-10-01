using UnityEngine;

public class HandDistancesTraveled : MonoBehaviour
{
    [SerializeField] private Transform leftHand, rightHand;
    private static float leftHandPathLength = 0, rightHandPathLength = 0;
    private static Vector3 leftLastPos, rightLastPos;
    private static bool recordingNow = false;

    private void Update()
    {
        if (recordingNow)
        {
            leftHandPathLength += Vector3.Distance(leftHand.position, leftLastPos);
            rightHandPathLength += Vector3.Distance(rightHand.position, rightLastPos);
        }

        leftLastPos = leftHand.position;
        rightLastPos = rightHand.position;
    }

    public static void StartRecording()
    {
        ResetHandPathLengths();
        recordingNow = true;
    }

    public static void FinishRecording()
    {
        recordingNow = false;
    }

    public static void ResetHandPathLengths()
    {
        leftHandPathLength = 0;
        rightHandPathLength = 0;
    }

    public static float GetLeftHandPathLength()
    {
        return leftHandPathLength;
    }

    public static float GetRightHandPathLength()
    {
        return rightHandPathLength;
    }
}