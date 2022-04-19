using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

public class Logging_XR : MonoBehaviour
{
    private void Start()
    {
        //Occulus has 6 buttons so we need to map this correctly to the amount
        button_presses_L = new List<int>() { 0, 0, 0, 0, 0, 0 };
        button_presses_R = new List<int>() { 0, 0, 0, 0, 0, 0 };
        left_hand = GameObject.Find("LeftHand Controller");
        right_hand = GameObject.Find("RightHand Controller");
    }

    private void Update()
    {
        if (trigger_one.action.WasPressedThisFrame())
        {
            button_presses_L[0]++;
        }
        if (trigger_two.action.WasPressedThisFrame())
        {
            button_presses_R[0]++;
        }

        //While the trigger is being pressed collect this value
        if (trigger_one.action.IsPressed())
        {
            collecting_path_left_hand = true;
        }
        else
        {
            if (collecting_path_left_hand)
            {
                collecting_path_left_hand = false;
                distance_traveled_left_hand.Add(calculate_distance_collected_path(L_Hand_path));
            }
        }

        //Right hand triggering
        if (trigger_two.action.IsPressed())
        {
            collecting_path_right_hand = true;
        }
        else
        {
            if (collecting_path_right_hand)
            {
                collecting_path_right_hand = false;
                distance_traveled_right_hand.Add(calculate_distance_collected_path(R_Hand_path));
            }
        }
    }

    private List<Vector3> L_Hand_path = new List<Vector3>();
    private List<Vector3> R_Hand_path = new List<Vector3>();

    private void FixedUpdate()
    {
        //Ticks checking for the amount of time required for collecting the path of the hand.
        ticks++;
        if (ticks > ticks_timer)
        {
            if (collecting_path_left_hand)
                L_Hand_path.Add(left_hand.transform.position);
            if (collecting_path_right_hand)
                R_Hand_path.Add(right_hand.transform.position);
            ticks = 0;
        }
    }

    ///----------------------------------------------------------------
    //Variables for the tracking
    private int ticks = 0;

    private int ticks_timer = 25; // Around 1/3 of a second?
    public List<int> button_presses_L = new List<int>();
    public List<int> button_presses_R = new List<int>();
    public List<float> task_times = new List<float>();
    public List<float> distance_traveled_left_hand = new List<float>();
    public List<float> distance_traveled_right_hand = new List<float>();

    //Not sure if we should store the entire path every time a user creates a gesture. I would prefer to make this as small as possible.
    //List<List<Vector3>> Path_created_Left_Hand;

    private float start_timer;
    private bool collecting_path_left_hand = false;
    private bool collecting_path_right_hand = false;

    private int object_pickups_successful = 0;
    private int object_pickups_fails = 0;

    public GameObject left_hand;
    public GameObject right_hand;

    public string Username;

    //TriggerBump L
    [SerializeField] private InputActionReference trigger_one;

    //TriggerBump R
    [SerializeField] private InputActionReference trigger_two;

    ///----------------------------------------------------------------
    //Utilities methods
    private float calculate_distance_collected_path(List<Vector3> path)
    {
        float distance = 0;
        for (int v = 0; v < path.Count - 1; v++)
        {
            distance += Vector3.Distance(path[v], path[v + 1]);
        }
        return distance;
    }

    public string ToCSV(string username)
    {
        var sb = new StringBuilder("Username,Time_Registered,Button_presses_L,Button_presses_R,Task_Times," +
        "Distance_L_hand, Distance_R_hand");
        sb.Append('\n').Append(username).Append(',').Append(Time.time.ToString()).Append(',').Append(button_presses_L.ToString()).Append(',').Append(button_presses_R.ToString()).Append(',').
        Append(',').Append(task_times.ToString()).Append(',').Append(distance_traveled_left_hand.ToString()).Append(",").Append(distance_traveled_right_hand.ToString());
        // foreach(var frame in keyFrames)
        // {
        //     sb.Append('\n').Append(frame.Time.ToString()).Append(',').Append(frame.Value.ToString());

        // }
        return sb.ToString();
    }

    public void SaveToFile()
    {
        string fileName = Username + "_" + Time.time.ToString();

        // Use the CSV generation from before
        var content = ToCSV(Username);

        // The target file path e.g.
#if UNITY_EDITOR
        var folder = Application.streamingAssetsPath;

        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
#else
        var folder = Application.persistentDataPath;
#endif

        var filePath = Path.Combine(folder, fileName + ".csv");

        using (var writer = new StreamWriter(filePath, false))
        {
            writer.Write(content);
        }

        // Or just
        //File.WriteAllText(content);

        Debug.Log($"CSV file written to \"{filePath}\"");
    }

    public void ResetStatistics()
    {
        button_presses_R.Clear();
        button_presses_L.Clear();
        task_times.Clear();
        distance_traveled_left_hand.Clear();
        distance_traveled_right_hand.Clear();
    }

    ///----------------------------------------------------------------
    //Functions for tracking the stuff around the world
    public void start_task_timer()
    {
        start_timer = Time.time;
        object_pickups_successful++;
    }

    public void stop_task_timer()
    {
        float end_timer = Time.time;
        float total_task_time = end_timer - start_timer;
        task_times.Add(total_task_time);
    }

    public void failed_task_timer()
    {
        start_timer = 0.0f;
        object_pickups_fails++;
    }

    public List<float> get_task_times()
    {
        return task_times;
    }

    public List<float> get_distances_left_hand()
    {
        return distance_traveled_left_hand;
    }

    public List<float> get_distances_right_hand()
    {
        return distance_traveled_right_hand;
    }

    public (List<int>, List<int>) get_button_presses()
    {
        return (button_presses_L, button_presses_R);
    }
}