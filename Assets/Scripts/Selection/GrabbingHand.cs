using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrabbingHand : MonoBehaviour
{
    [SerializeField] private InputActionReference grabActionReference;
    [SerializeField] private Transform attachTransform;

    public MiniMap miniMap;
    public MiniMapInteractor miniMapIntreractor;
    public ClutterHandler_circumferenceDisplay ClutterHander_circumferenceDisplay;
    public bool addForceOnObjectDetach = true;
    public float objPushForce = 20.0f;

    public GameObject objectInHand;

    public static bool isHovering = false;

    // add a variable to store the list of objects that are colliding with the hand live
    public HashSet<shapeItem_2> collidingWithHand;

    private void Start()
    {
        objectInHand = null;
        collidingWithHand = new HashSet<shapeItem_2>();
    }

    private void Update()
    {
        if (grabActionReference.action.WasReleasedThisFrame())
        {
            ReleaseCurrentlyHeldObject();
        }

        isHovering = collidingWithHand.Count != 0;
    }

    private void FixedUpdate()
    {
        collidingWithHand.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ClutterHander_circumferenceDisplay.isFrozen || objectInHand != null || SelectionTechniqueDistributer.currentlySetActiveTechnique.name != "MiniMap")
            return;

        if (other.gameObject.GetComponent<shapeItem_2>())
        {
            isHovering = true;

            collidingWithHand.Add(other.GetComponent<shapeItem_2>());

            ClutterHander_circumferenceDisplay.removeDuplicates();
            ClutterHander_circumferenceDisplay.insertToSpots(collidingWithHand);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (ClutterHander_circumferenceDisplay.isFrozen || objectInHand != null || SelectionTechniqueDistributer.currentlySetActiveTechnique.name != "MiniMap")
            return;

        if (other.gameObject.GetComponent<shapeItem_2>())
        {
            collidingWithHand.Remove(other.GetComponent<shapeItem_2>());

            ClutterHander_circumferenceDisplay.removeDuplicates();
            ClutterHander_circumferenceDisplay.insertToSpots(collidingWithHand);
        }
    }

    private void OnTriggerStay(Collider col)
    {
        if (objectInHand == null && grabActionReference.action.WasPressedThisFrame())
        {
            GameObject duplicate, original;

            // check if the tag of the object  is one from the circumference display
            if (col.gameObject.tag == "unclutterDuplicate")
            {
                duplicate = col.gameObject;

                //original = ClutterHandler_circumferenceDisplay.originaltoduplicatewithgameObject.FirstOrDefault(x => x.Value == col.gameObject).Key;
                //original = ClutterHandler_circumferenceDisplay.originalToDuplicate.FirstOrDefault(x => x.Value == col.GetComponent<shapeItem_3>().gameObject).Key.gameObject;

                original = duplicate.GetComponent<shapeItem_3>().original;
                GameObject originalOfFirstDuplicate = duplicate.gameObject.GetComponent<shapeItem_3>().original.gameObject.GetComponent<shapeItem_2>().original;
                miniMap.RemoveFromMinimapUponGrab(original);

                PickupObject(originalOfFirstDuplicate);

                objectInHand = originalOfFirstDuplicate;

                collidingWithHand.Clear();
                ClutterHander_circumferenceDisplay.removeDuplicates();
                ClutterHander_circumferenceDisplay.isFrozen = false;
            }
            else
            {
                original = col.gameObject.GetComponent<shapeItem_2>().original;
                duplicate = col.gameObject;

                miniMap.RemoveFromMinimapUponGrab(duplicate);

                PickupObject(original);

                objectInHand = original;

                collidingWithHand.Clear();
                if (SelectionTechniqueDistributer.currentlySetActiveTechnique.name == "MiniMap")
                {
                    ClutterHander_circumferenceDisplay.removeDuplicates();
                }
                ClutterHander_circumferenceDisplay.isFrozen = false;
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