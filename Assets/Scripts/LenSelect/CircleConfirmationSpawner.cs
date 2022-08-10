using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleConfirmationSpawner : MonoBehaviour
{

    static bool wasSelectClicked = false;

    public static GameObject THIS;

    Transform originalTransform;

    public LenSelect RADIUSGETTER;

    public Transform HMD;


    void Start()
    {
        wasSelectClicked = false;

        THIS = this.gameObject;
        originalTransform = THIS.transform;
        //THIS.AddComponent<Renderer>();

        // this should credate a brown circle 
        Color brownEditted = new Color(165, 42, 42);
        //THIS.GetComponent<Renderer>().material.SetColor("_Color", brownEditted);

        THIS.AddComponent<Rigidbody>();
        THIS.GetComponent<Rigidbody>().isKinematic = true;
        THIS.GetComponent<Rigidbody>().useGravity = false;

        THIS.AddComponent<SphereCollider>();
        THIS.GetComponent<SphereCollider>().isTrigger = true;
        THIS.SetActive(false);
    }
    private void Update()
    {
        //wasSelectClicked = RayManager.getStatusOfGrip();

    }

    //call this when the ray hits the object 
    public void Inraycastmodification(GameObject other)
    {
;
        print("PRINT *********************" + other.name);

        if (other.name == "FlashlightConeCenter" || other == null)
            return;
        //wasSelectClicked = RayManager.currentGameObjectHighlighted;
        if (wasSelectClicked == true)
        {
            print("TREASURE************************************RAR***********************");
            float currentRadius =0.2f;
            //float currentRadius = 0.5f;

            THIS.transform.position = new Vector3(other.transform.position.x,
                other.transform.position.y + 1f, other.transform.position.z);

            THIS.transform.localScale = new Vector3(currentRadius, currentRadius, currentRadius);
            THIS.transform.LookAt(HMD);
            THIS.SetActive(true);

        }

    }

    public void ConfirmSelection()
    {

         // maybe add some audiop right here 

        // take care of the position 
        THIS.SetActive(false);
        THIS.transform.position = originalTransform.position;
        THIS.transform.localScale = originalTransform.localScale;
    }
}
