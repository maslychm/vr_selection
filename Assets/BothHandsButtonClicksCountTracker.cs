using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BothHandsButtonClicksCountTracker : MonoBehaviour
{


    // let's store all the possible clicks 
    public InputActionReference leftTriggerButton, rightTriggerButton, leftGripButton, rightGripButton;
    public static int leftTriggerButton_Count, rightTriggerButton_Count, leftGripButton_Count, rightGripButton_Count;

    // Start is called before the first frame update
    void Start()
    {

        // assign each of the buttons to their physical reference 
        leftTriggerButton_Count = 0;
        rightTriggerButton_Count = 0;
        leftGripButton_Count = 0;
        rightGripButton_Count=0; 
        
    }

    // Update is called once per frame
    void Update()
    {

        if(ExperimentTrial.isTrialOngoingNow == false)
        {
            flushAllCounts();
            return;
        }

        if (leftGripButton.action.WasPressedThisFrame())
            leftGripButton_Count++;
        if (rightGripButton.action.WasPressedThisFrame())
            rightGripButton_Count++;
        if (leftTriggerButton.action.WasPressedThisFrame())
            leftTriggerButton_Count++;
        if (rightTriggerButton.action.WasPressedThisFrame())
            rightTriggerButton_Count++;
        
    }

    private void flushAllCounts()
    {
        leftTriggerButton_Count = 0;
        rightTriggerButton_Count = 0;
        leftGripButton_Count = 0;
        rightGripButton_Count = 0;
    }
}
