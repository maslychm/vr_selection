// in the works..................
using UnityEngine;
using UnityEngine.UI;

public class Zone : MonoBehaviour
{
    /*
     *
     *  STILL CONTAINS LOGIC THAT NEEDS TO BE REVIEWED (Grabbing Items Logic)!!
     * 
     */

    // this file will mainly take care of the zone/slot where the shapes 
    // will be represented 

    public GameObject ItemInZone;
    public Image ZoneImage;
    Color originalColor;

    void Start()
    {
        ZoneImage = GetComponentInChildren<Image>();
        originalColor = ZoneImage.color;
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (!ItemInZone)
            return;

        GameObject temp = other.gameObject;
        if (!checkItemThere(temp))
            return;

    }

    bool checkItemThere(GameObject temp)
    {
        return temp.GetComponent<shapeItem>();
    }

    void InsertItem(GameObject temp)
    {

        /* NOTE----
         * isKinematic : 
         * 
         * Controls whether physics affects the rigidbody. 
         * If isKinematic is enabled, Forces, collisions or joints will not 
         * affect the rigidbody anymore. The rigidbody will be under full control 
         * of animation or script control by changing transform.
        */
        temp.GetComponent<Rigidbody>().isKinematic = true;
        temp.transform.SetParent(gameObject.transform, true);
        temp.transform.localPosition = Vector3.zero;
        temp.transform.localEulerAngles = temp.GetComponent<shapeItem>().zoneRotation;
        temp.GetComponent<shapeItem>().inZone = true;
        temp.GetComponent<shapeItem>().currentZone= this;
        ItemInZone = temp;
        ZoneImage.color = Color.blue;
    }

    void ResetColor()
    {
        ZoneImage.color = originalColor;
    }

}
