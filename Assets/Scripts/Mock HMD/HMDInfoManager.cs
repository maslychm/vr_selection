using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

// Make sure Mock HMD Loader is enabled in XR Plug-in Management

public class HMDInfoManager : MonoBehaviour
{
    // Start is called before the first frame update
    // 11 minutes into video
    void Start()
    {
        // Testing the input of the program
        if (!XRSettings.isDeviceActive)
        {
            Debug.Log("No Headset plugged");
        }
        else if (XRSettings.isDeviceActive && (XRSettings.loadedDeviceName == "Mock HMD"
            || XRSettings.loadedDeviceName == "MockHMDDisplay"))
        {
            Debug.Log("Using Mock HMD");
        }
        else
        {
            Debug.Log("We Have a headset " + XRSettings.loadedDeviceName);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
