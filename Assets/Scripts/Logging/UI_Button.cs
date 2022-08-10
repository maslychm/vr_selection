using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Table_request))]
[CanEditMultipleObjects]
public class UI_Button : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Table_request button = (Table_request)target;

        if (GUILayout.Button("Reset Experiment for New Trial"))
        {
            Debug.Log("Confirming username inputted.");
            button.StartExperiment();
        }
    }
}