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
    Vector3 _originalScale; // store the original scale of the item so when it will be cleared we will use this as reference
    Vector3 _originalPosition; // store hte original position of the shape 
    Vector3 _originalEulerAngles; // store the original euler angles 

    void Start()
    {
        ZoneImage = GetComponentInChildren<Image>();
        originalColor = ZoneImage.color;
        ItemInZone = null;
        
    }

    bool checkItemThere(GameObject temp)
    {
        return temp.GetComponent<shapeItem>();
    }

    void _autoResizerInZone(GameObject temp2)
    {

        // Standard size of the zone:
        //        x:  0.7 
        //        y:  0.7
        //        z:  0.7
        // let's make it slighly smaller with 0.5 
        // and store its original scale and position/eulerAngles
        _originalEulerAngles = temp2.transform.eulerAngles;
        _originalPosition = temp2.transform.position;
        //_originalScale = new Vector3(temp2.transform.localScale.x, temp2.transform.localScale.y, temp2.transform.localScale.z);


    }

    // takes care of clearing the zone 
    public void removeFromZone(GameObject _currentInZone)
    {

       // _currentInZone.GetComponent<Rigidbody>().isKinematic = false;

        // to be false?

        _currentInZone.transform.SetParent(gameObject.transform, true);

        _setToOriginalPosition(_currentInZone);

        _setToOriginalEulerAngles(_currentInZone);

       // _setToOriginalScale(_currentInZone);

        _currentInZone.GetComponent<shapeItem>().inZone = false;

        _currentInZone.GetComponent<shapeItem>().currentZone = null;

        ItemInZone = null;

        ResetColor();

       
    }
    /*
    void _setToOriginalScale(GameObject temp3)
    {
        temp3.transform.localScale = _originalScale;
    }*/

    void _setToOriginalPosition(GameObject temp4)
    {
        temp4.transform.position = _originalPosition;
    }
    
    void _setToOriginalEulerAngles(GameObject temp5)
    {
        temp5.transform.eulerAngles = _originalEulerAngles;
    }

    public void InsertItem(GameObject temp)
    {

        /* NOTE----
         * isKinematic : 
         * 
         * Controls whether physics affects the rigidbody. 
         * If isKinematic is enabled, Forces, collisions or joints will not 
         * affect the rigidbody anymore. The rigidbody will be under full control 
         * of animation or script control by changing transform.
        */

        // first call the resizer 
        _autoResizerInZone(temp);
        if (temp.tag == "star")
        {
            this.transform.localEulerAngles = new Vector3(180.0f, 0.0f, 0.0f);
        }
        if (temp.tag == "pyramid")
        {
            this.transform.localEulerAngles = new Vector3(180.0f, 0.0f, 0.0f);

        }

        //temp.GetComponent<Rigidbody>().isKinematic = true;
        temp.transform.SetParent(gameObject.transform, true);
        temp.transform.localPosition = Vector3.zero;

        temp.transform.rotation = this.transform.rotation;
        temp.GetComponent<shapeItem>().currentZone = this;
        ItemInZone = temp;

        if (temp.tag == "infinity")
        {
            temp.transform.localEulerAngles = new Vector3(90.0f, 0.0f, 0.0f);

        }

        ZoneImage.color = Color.blue;
    }

    void ResetColor()
    {
        ZoneImage.color = originalColor;
    }

}
