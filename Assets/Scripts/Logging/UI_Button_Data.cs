using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Logging_XR))]
[CanEditMultipleObjects]
public class UI_Button_Data : Editor
{
    public override void OnInspectorGUI() 
    {
        base.OnInspectorGUI();
        Logging_XR button = (Logging_XR) target;
 
        if (GUILayout.Button("Export Data to CSV with Username"))
        {
            Debug.Log("Exporting....");
            button.SaveToFile();
            Debug.Log("Exported sucessfully!");
        }
    }
}
