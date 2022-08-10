using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enqueue & Dequeue

public class SelectedFlash : MonoBehaviour
{
    public GameObject selectedObject;

    // The colors
    public int redColor;
    public int greenColor;
    public int blueColor;

    // Whether looking at an object or not
    public bool lookingAtObject = false;

    // Are we flashing/highlighting material or not
    public bool flashingIn = true;

    // Has flashing/highlighting been executed
    public bool startedFlashing = false;

    void Update()
    {
        if (lookingAtObject ==  true)
        {
            // 255 is the alpha 
            // Remember that need to have as bytes and not numbers
            selectedObject.GetComponent<Renderer>().material.color = new Color32((byte)redColor, (byte)greenColor, (byte)blueColor, 255);
        }
    }

    // Looking at the object
    // Change later
    void OnMouseOver()
    {
        // Finds name that was discovered from the raycast and placing it into variable
        selectedObject = GameObject.Find(Highlighting.selectedObject);

        lookingAtObject = true;

        if(startedFlashing == false)
        {
            startedFlashing = true;
            StartCoroutine(FlashObject());
        }
    }

    void OneMouseExit()
    {
        startedFlashing = false;
        lookingAtObject = false;

        StopCoroutine(FlashObject());

        // Reseting material
        selectedObject.GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);        
    }

    IEnumerator FlashObject()
    {
        while(lookingAtObject == true)
        {
            // How fast or slow you want it to highlight
            yield return new WaitForSeconds(0.02f);

            // Basically changing color
            if(flashingIn == true)
            {
                // Checking the blue color
                if(blueColor <= 30)
                {
                    flashingIn = false;
                }
                else
                {
                    blueColor -= 25;
                    greenColor -= 1;
                }
            }

            if (flashingIn == false)
            {

                if (blueColor >= 250)
                {
                    flashingIn = true;
                }
                else
                {
                    blueColor += 25;
                    greenColor += 1;
                }
            }
        }
    }
}

