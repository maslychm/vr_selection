using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MINIMAPInitial : MonoBehaviour
{
    //public XRGestureFilterInteractor temp = new XRGestureFilterInteractor();
    public static bool isActive;
    public GameObject Inventory;
    public GameObject Anchor;

    // Start is called before the first frame update
    void Start()
    {
        isActive = false;
        Inventory.SetActive(false);
        
    }

     public void showMiniMap(bool t)
    {
        if(t)
        {
            isActive = true;
            Inventory.SetActive(true);
        }

        if(isActive)
        {
            Inventory.transform.position = Anchor.transform.position;
            Inventory.transform.eulerAngles = new Vector3(Anchor.transform.eulerAngles.x + 15, Anchor.transform.eulerAngles.y, 0);
        }


    }

    public  void closeMiniMap(bool t)
    {
        if (t)
        {
            isActive = false;
            Inventory.SetActive(false);
        }

    }
}
