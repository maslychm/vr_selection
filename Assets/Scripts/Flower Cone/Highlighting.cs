using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Casting to Object
// Raycast obtaining the name of the object
// Need to make sure each object is named differently
// Ensure colliders are on

// Attach this script to whatever you are viewing with (Change to flashlight)

public class Highlighting : MonoBehaviour
{
    public static string selectedObject;

    public string internalObject;

    // Raycast
    public RaycastHit theObject;



    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out theObject))
        {
            // Return the name of the object
            selectedObject = theObject.transform.gameObject.name;
            internalObject = theObject.transform.gameObject.name;
        }
    }
}
