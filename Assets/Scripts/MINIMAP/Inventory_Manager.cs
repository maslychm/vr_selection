using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The sole purpose for this one is to continuously provide an update of the free zones and 
/// the full ones, along with the next availbale zone idnex where we can insert 
/// </summary>

public class Inventory_Manager : MonoBehaviour
{
    public GameObject theInventory;

    private Stack<GameObject> theStack; // think this migh be very good O(1)


    // hold the count for the filled spots 
    private int filled_count, emptycount_Helper;

    private bool isFull, isEmpty;
    
    void Start()
    {
        filled_count = 0;
        emptycount_Helper = 0;
        isFull = false;
        isEmpty = true;
        theStack = new Stack<GameObject>();

        for (int i = 0; i < theInventory.transform.childCount; i++)
        {
            if (theInventory.transform.GetChild(i).GetComponent<Zone>() == null)
                continue;

            theStack.Push(theInventory.transform.GetChild(i).gameObject);


        }

    }

    // update with the function from the zone 

    // Update is called once per frame
    void Update()
    {

        for(int i = 0; i < theInventory.transform.childCount; i++)
        {
            if (theInventory.transform.GetChild(i).GetComponent<Zone>() == null)
                continue;

            if (theInventory.transform.GetChild(i).GetComponent<Zone>().ItemInZone == null)
            {
                emptycount_Helper++;

                // original line 
                theStack.Push(theInventory.transform.GetChild(i).gameObject);
            }
            else
            {
                filled_count++;
            }

        }

        if (filled_count == 6)
            isFull = true;
        if(emptycount_Helper == 6)
            isEmpty = true;

    }

    public int getCount()
    {
        return filled_count;
    }

    public bool isFull_Getter()
    {
        return isFull;
    }

    public bool isEmpty_Getter()
    {
        return isEmpty;
    }


    // get the next available position  
    public GameObject getAvailablePositions()
    {
        if(theStack.Count != 0)
            return this.theStack.Pop();

        return null;
    }

}
