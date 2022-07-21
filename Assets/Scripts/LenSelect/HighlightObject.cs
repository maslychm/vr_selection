using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightObject : MonoBehaviour
{

    public bool isTriggered = false;
    public float count = 2.0f;
    public GameObject current;
    // Start is called before the first frame update
    void Start()
    {
        isTriggered = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTriggered == true)
        {
            while (count > 0)
                count -= 0.4f;



            if(count == 0)
            {
                turnOffShader();
                count = 2.0f;
            }
        }

        //current = RayManager.helperTemp;
    }
    
    void turnOffShader()
    {
        if (current != null)
        {
            current.GetComponent<cakeslice.Outline>().eraseRenderer = true;
        }
    }
}
