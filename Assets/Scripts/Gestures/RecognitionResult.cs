using UnityEngine;

public struct RecognitionResult
{
    public int gestureId;
    public string gestureName;
    public Vector3 startPt;
    public Vector3 endPt;

    public RecognitionResult(int gestureId, string gestureName, Vector3 startPt, Vector3 endPt)
    {
        this.gestureId = gestureId;
        this.gestureName = gestureName;
        this.startPt = startPt;
        this.endPt = endPt;
    }
}