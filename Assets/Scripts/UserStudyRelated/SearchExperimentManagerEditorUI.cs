using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SearchExperimentManager))]
[CanEditMultipleObjects]
public class SearchExperimentManagerEditorUI : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SearchExperimentManager experimentManager = (SearchExperimentManager)target;

        if (GUILayout.Button("Start Experiment"))
        {
            experimentManager.StartExperiment();
        }

        if (GUILayout.Button("Clear Experiment"))
        {
            experimentManager.ClearExperiment();
        }
    }
}