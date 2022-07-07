using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrabbingHand : MonoBehaviour
{

    [SerializeField] private InputActionReference grabActionReference;
    [SerializeField] private Transform attachTransform;
    private List<string> interactableTags = new List<string>() { "cube", "sphere", "star", "pyramid", "cylinder", "infinity" };
    public  bool isOnHand;
    public MiniMap temp;
    public MiniMapInteractor temp2;

    public bool addForceOnObjectDetach = true;
    public float objPushForce = 20.0f;

    public GameObject onHand;
    // Start is called before the first frame update
    void Start()
    {

        isOnHand = false;
        onHand = null;
    }

    // Update is called once per frame
    void Update()
    {

        if (grabActionReference.action.WasPressedThisFrame())
        {
            print("clickedTriggerButton*********************");
        }

        if (grabActionReference.action.WasReleasedThisFrame())
        {
            releaseCurrentlyHeldObject();
        }
    }

    private void OnTriggerStay(Collider col)
    {

        print("This was reached >>>>>>>>>>>>>");
        if (onHand == null && grabActionReference.action.WasPressedThisFrame() && isOnHand == false)
        {

            print("Grabbed Object currently*******");
            GameObject original = col.gameObject.GetComponent<shapeItem_2>().original;
            GameObject duplicate = col.gameObject;
            temp.removeFromMinimapUponGrab(duplicate);

            PickupObject(original);

            onHand = original;
            isOnHand = true;

        }
    }
/*
    private void OnTriggerEnter(Collider col)
    {

        if (onHand == null && grabActionReference.action.WasPressedThisFrame() && isOnHand == false)
        {

            print("Grabbed Object currently*******");
            GameObject original = col.gameObject.GetComponent<shapeItem_2>().original;
            GameObject duplicate = col.gameObject;
            temp.removeFromMinimapUponGrab(duplicate);

            PickupObject(original);

            onHand = original;
            isOnHand = true;

        }
    }*/
    private void OnTriggerExit(Collider col)
    {


            print("Exited the pyramid");
    }
    private void PickupObject(GameObject o)
    {
        o.transform.SetPositionAndRotation(attachTransform.position, attachTransform.rotation);
        o.transform.parent = attachTransform;
        o.GetComponent<Rigidbody>().useGravity = false;
        o.GetComponent<Rigidbody>().isKinematic = true;
    }

    private void releaseCurrentlyHeldObject()
    {

        
        if (onHand == null && isOnHand == false)
            return;

        print("Being released now *****");
        onHand.transform.parent = null;
        onHand.GetComponent<Rigidbody>().useGravity = true;
        onHand.GetComponent<Rigidbody>().isKinematic = false;

        if (addForceOnObjectDetach)
        {
            Vector3 applyForce = onHand.transform.forward * objPushForce;
            onHand.GetComponent<Rigidbody>().AddForce(applyForce, ForceMode.Impulse);
        }

        onHand = null;
        isOnHand = false;
    }
}
