using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class Logging_XR : MonoBehaviour
{
    public string Username = "empty";
    public string techName = "empty";

    [SerializeField] private InputActionReference leftTriggerAction;
    [SerializeField] private InputActionReference rightTriggerAction;

    private GameObject leftHand;
    private GameObject rightHand;

    // Vars to log to file

    private List<int> leftButtonPresses_Log = new List<int>();
    private List<int> rightButtonPresses_Log = new List<int>();

    private List<float> leftHandDistances_Log = new List<float>();
    private List<float> rightHandDistances_Log = new List<float>();

    private List<float> leftTriggerHeldDownTime_Log = new List<float>();
    private List<float> rightTriggerHeldDownTime_Log = new List<float>();

    private List<float> perObjectTimes_Log = new List<float>();

    // Temp vars

    private List<Vector3> leftHandPath_temp = new List<Vector3>();
    private List<Vector3> rightHandPath_temp = new List<Vector3>();

    private float leftTriggerStart, rightTriggerStart;

    private float startTimer;

    private void Start()
    {
        //Occulus has 6 buttons so we need to map this correctly to the amount
        leftButtonPresses_Log = new List<int>() { 0, 0, 0, 0, 0, 0 };
        rightButtonPresses_Log = new List<int>() { 0, 0, 0, 0, 0, 0 };
        leftHand = GameObject.Find("LeftHand Controller");
        rightHand = GameObject.Find("RightHand Controller");
    }

    private void Update()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        // Left Hand
        if (leftTriggerAction.action.WasPressedThisFrame())
        {
            leftButtonPresses_Log[0]++;
            leftTriggerStart = Time.time;
        }

        if (leftTriggerAction.action.WasReleasedThisFrame())
        {
            leftTriggerHeldDownTime_Log.Add(Time.time - leftTriggerStart);

            var leftHandPathDist = CalculateHandPathDistance(leftHandPath_temp);
            leftHandDistances_Log.Add(leftHandPathDist);
            leftHandPath_temp.Clear();
        }

        if (leftTriggerAction.action.IsPressed())
        {
            RecordLeftHandPosition();
        }

        // Right Hand
        if (rightTriggerAction.action.WasPressedThisFrame())
        {
            rightButtonPresses_Log[0]++;
            rightTriggerStart = Time.time;
        }

        if (rightTriggerAction.action.WasReleasedThisFrame())
        {
            rightTriggerHeldDownTime_Log.Add(Time.time - rightTriggerStart);

            var rightHandPathDist = CalculateHandPathDistance(rightHandPath_temp);
            rightHandDistances_Log.Add(rightHandPathDist);
            rightHandPath_temp.Clear();
        }

        if (rightTriggerAction.action.IsPressed())
        {
            RecordRightHandPosition();
        }
    }

    private void RecordRightHandPosition()
    {
        rightHandPath_temp.Add(rightHand.transform.position);
    }

    private void RecordLeftHandPosition()
    {
        leftHandPath_temp.Add(leftHand.transform.position);
    }

    private float CalculateHandPathDistance(List<Vector3> path)
    {
        float distance = 0;
        for (int v = 0; v < path.Count - 1; v++)
        {
            distance += Vector3.Distance(path[v], path[v + 1]);
        }
        return distance;
    }

    private string ListToString<T>(List<T> array)
    {
        return $"[{string.Join(",", array)}]";
    }

    public string ToCSV(string username)
    {
        var headers = new List<string> {
            "username",
            "technique_name",
            "total_time",
            "left_button_presses",
            "right_button_presses",
            "time_per_object",
            "left_hand_distances",
            "right_hand_distances"
        };

        var values = new List<string> {
            username.ToString(),
            techName.ToString(),
            Time.time.ToString(),
            ListToString(leftButtonPresses_Log),
            ListToString(rightButtonPresses_Log),
            ListToString(perObjectTimes_Log),
            ListToString(leftHandDistances_Log),
            ListToString(rightHandDistances_Log)
        };

        string ret = string.Join(";", headers);
        ret += "\n";
        ret += string.Join(";", values);

        return ret;
    }

    public void SaveToFile()
    {
        var dateStr = DateTime.Now.ToString("d").Replace("/", "-");
        var timeStr = DateTime.Now.ToString("T").Replace(" ", "");
        var fileName = $"{Username}_{techName}_{dateStr}_{timeStr}";

        fileName = fileName.Replace(":", "-");

        var content = ToCSV(Username);

#if UNITY_EDITOR
        var folder = Application.streamingAssetsPath;

        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
#else
        var folder = Application.persistentDataPath;
#endif

        var filePath = Path.Combine(folder, fileName + ".csv");

        using (var writer = new StreamWriter(filePath, true))
        {
            writer.Write(content);
        }

        print($"Wrote CSV to: {filePath}");
    }

    public void ResetLogger()
    {
        rightButtonPresses_Log.Clear();
        leftButtonPresses_Log.Clear();
        perObjectTimes_Log.Clear();
        leftHandDistances_Log.Clear();
        rightHandDistances_Log.Clear();

        print("Resetting logger");
    }

    public void StartObjectTrackingTimer()
    {
        startTimer = Time.time;
    }

    public void StopTaskTimer()
    {
        perObjectTimes_Log.Add(Time.time - startTimer);
    }
}