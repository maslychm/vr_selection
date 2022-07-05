using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCircular_Selection_initial : MonoBehaviour
{
    public static bool isActive = false;
    public GameObject circularMap;
    public GameObject Anchor;


    void Start()
    {
        isActive = false;
        circularMap.SetActive(false);

    }

    public void showMiniMap(bool t)
    {
        if (t == true)
        {
            isActive = true;
            circularMap.SetActive(true);
        }

        if (isActive == true)
        {
            circularMap.transform.position = Anchor.transform.position;

            circularMap.transform.eulerAngles = new Vector3(Anchor.transform.eulerAngles.x + 15, Anchor.transform.eulerAngles.y, 0);

        }


    }

    public void closeMiniMap(bool t)
    {
        if (t == true)
        {
            isActive = false;
            circularMap.SetActive(false);
        }

    }
}
