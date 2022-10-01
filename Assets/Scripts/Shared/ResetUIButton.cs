using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ResetXROriginCenter))]
[CanEditMultipleObjects]
public class ResetUIButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ResetXROriginCenter experimentManager = (ResetXROriginCenter)target;

        if (GUILayout.Button("Reset Orientation"))
        {
            experimentManager.ResetPosition();
        }

    }
}