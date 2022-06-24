using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MINIMAPInitial : MonoBehaviour
{

    public static bool isActive = false;
    public GameObject Inventory;
    public GameObject Anchor;


    void Start()
    {
        isActive = false;
        Inventory.SetActive(false);
        
    }

     public void showMiniMap(bool t)
    {
        if(t == true)
        {
            isActive = true;
            Inventory.SetActive(true);
        }

        if(isActive == true)
        {
            Inventory.transform.position = Anchor.transform.position;
        
            Inventory.transform.eulerAngles = new Vector3(Anchor.transform.eulerAngles.x + 15, Anchor.transform.eulerAngles.y, 0);

        }


    }

    public  void closeMiniMap(bool t)
    {
        if (t == true)
        {
            isActive = false;
            Inventory.SetActive(false);
        }

    }
}
