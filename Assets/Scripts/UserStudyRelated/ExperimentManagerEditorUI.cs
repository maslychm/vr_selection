﻿using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ExperimentManager))]
[CanEditMultipleObjects]
public class UI_Button : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ExperimentManager experimentManager = (ExperimentManager)target;

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