using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrabbingHand : MonoBehaviour
{
    [SerializeField] private InputActionReference grabActionReference;
    [SerializeField] private Transform attachTransform;

    public MiniMap temp;
    public MiniMapInteractor temp2;
    public ClutterHandler_circumferenceDisplay helper208;
    public bool addForceOnObjectDetach = true;
    public float objPushForce = 20.0f;

    private bool wasAdded = false;

    public GameObject objectInHand;

    public static bool isHovering = false;

    // add a variable to store the list of objects that are colliding with the hand live
    private List<GameObject> collidingWithHand;

    private void Start()
    {
        objectInHand = null;

        // if we need this later we have it
        collidingWithHand = new List<GameObject>();
    }

    private void Update()
    {
        if (grabActionReference.action.WasPressedThisFrame()) { }

        if (grabActionReference.action.WasReleasedThisFrame())
        {
            ReleaseCurrentlyHeldObject();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<shapeItem_2>())
        {
            Collider[] _collidersWithHand = Physics.OverlapSphere(this.gameObject.transform.position, 0.03f);

            isHovering = true;

            if (helper208.await == true || objectInHand != null)
                return;
            helper208.removeDuplicates();
            helper208.helper(_collidersWithHand);
            wasAdded = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (helper208.await == false && wasAdded == true)
        {
            isHovering = false;
            if (helper208.await == true)
                return;
            helper208.removeDuplicates();
            wasAdded = false;
        }
    }

    private void OnTriggerStay(Collider col)
    {
        if (objectInHand == null && grabActionReference.action.WasPressedThisFrame() && (col.gameObject.GetComponent<shapeItem_3>() || col.gameObject.GetComponent<shapeItem_2>()))
        {
            GameObject duplicate, original;
            // check if the tag of the object  is one from the circumference display
            if (col.gameObject.tag == "unclutterDuplicate")
            {
                duplicate = col.gameObject;
                if (duplicate == null)
                    print("P111");
                original = ClutterHandler_circumferenceDisplay.originaltoduplicatewithgameObject.FirstOrDefault(x => x.Value == col.gameObject).Key;
                if (original == null)
                    print("P222");

                GameObject originalOfFirstDuplicate = duplicate.gameObject.GetComponent<shapeItem_3>().original.gameObject.GetComponent<shapeItem_2>().original;
                if (originalOfFirstDuplicate == null)
                    print("P333");

                //print("HERE ARE ALL THE CHAIN OF PARENTS ; " + duplicate.name + " its parent -> " + original.name + "its original ever -> " + originalOfFirstDuplicate.name);
                temp.RemoveFromMinimapUponGrab(original);

                PickupObject(originalOfFirstDuplicate);

                objectInHand = originalOfFirstDuplicate;

                helper208.removeDuplicates();
                wasAdded = false;
                helper208.await = false;
            }
            else
            {
                original = col.gameObject.GetComponent<shapeItem_2>().original;
                duplicate = col.gameObject;

                temp.RemoveFromMinimapUponGrab(duplicate);

                PickupObject(original);

                objectInHand = original;

                // helper208.removeDuplicates();
                helper208.removeDuplicates();
                wasAdded = false;
                helper208.await = false;
            }
        }
    }

    private void PickupObject(GameObject o)
    {
        o.transform.SetPositionAndRotation(attachTransform.position, attachTransform.rotation);
        o.transform.parent = attachTransform;
        o.GetComponent<Rigidbody>().useGravity = false;
        o.GetComponent<Rigidbody>().isKinematic = true;
    }

    private void ReleaseCurrentlyHeldObject()
    {
        if (objectInHand == null)
            return;

        objectInHand.transform.parent = null;
        //objectInHand.GetComponent<Rigidbody>().useGravity = true;
        //objectInHand.GetComponent<Rigidbody>().isKinematic = false;

        if (addForceOnObjectDetach)
        {
            Vector3 applyForce = objectInHand.transform.forward * objPushForce;
            objectInHand.GetComponent<Rigidbody>().AddForce(applyForce, ForceMode.Impulse);
        }

        objectInHand = null;
    }
}