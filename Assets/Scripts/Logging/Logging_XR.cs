using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Logging_XR : MonoBehaviour
{
    void Start()
    {
        //Occulus has 6 buttons so we need to map this correctly to the amount
        button_presses_L = new List<int>(){0,0,0,0,0,0};
        button_presses_R = new List<int>(){0,0,0,0,0,0};
        left_hand = GameObject.Find("LeftHand Controller");
        right_hand = GameObject.Find("RightHand Controller");
    }

    void Update()
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
        if(trigger_one.action.IsPressed())
        { 
            collecting_path_left_hand = true;

        }
        else
        { 
            collecting_path_left_hand = false;
            distance_traveled_left_hand.Add(calculate_distance_collected_path(L_Hand_path));
        }

        //Right hand triggering 
        if(trigger_two.action.IsPressed())
        { 
            collecting_path_right_hand = true;
        }
        else
        { 
            collecting_path_right_hand = false;
            distance_traveled_right_hand.Add(calculate_distance_collected_path(R_Hand_path));
        }

    }


    List<Vector3> L_Hand_path;
    List<Vector3> R_Hand_path;

    void FixedUpdate()
    {
        //Ticks checking for the amount of time required for collecting the path of the hand. 
        ticks++;
        if (ticks > ticks_timer)
        {
            if(collecting_path_left_hand)
                L_Hand_path.Add(left_hand.transform.position);
            if(collecting_path_right_hand)
                R_Hand_path.Add(right_hand.transform.position);
            ticks = 0;
        }
    }

    ///----------------------------------------------------------------
    //Variables for the tracking
    int ticks = 0;
    int ticks_timer = 25; // Around 1/3 of a second?  
    List<int> button_presses_L;
    List<int> button_presses_R;


    //Not sure if we should store the entire path every time a user creates a gesture. I would prefer to make this as small as possible.
    //List<List<Vector3>> Path_created_Left_Hand; 

    List<float> distance_traveled_left_hand;
    List<float> distance_traveled_right_hand;
    float start_timer;
    List<float> task_times; 
    bool collecting_path_left_hand = false; 
    bool collecting_path_right_hand = false; 

    int object_pickups_successful = 0;
    int object_pickups_fails = 0;

    public GameObject left_hand;
    public GameObject right_hand;

    //TriggerBump L
    [SerializeField] private InputActionReference trigger_one;
    
    //TriggerBump R 
    [SerializeField] private InputActionReference trigger_two;

    ///----------------------------------------------------------------
    //Utilities methods
    float calculate_distance_collected_path(List<Vector3> path)
    {   
        float distance = 0;
        for (int v = 0; v< path.Count - 1; v++)
        { 
            distance += Vector3.Distance(path[v], path[v+1]);
        }
        return distance;
    }

    ///----------------------------------------------------------------
    //Functions for tracking the stuff around the world
    public void start_task_timer()
    { 
        start_timer =Time.time;
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

    public (List<int>,List<int>) get_button_presses()
    { 
        return (button_presses_L, button_presses_R);
    }
}
