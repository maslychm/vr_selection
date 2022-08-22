using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class GrabbingHand : MonoBehaviour
{
    [SerializeField] private InputActionReference grabActionReference;
    [SerializeField] private Transform attachTransform;

    public MiniMap miniMap = null;
    public MiniMapInteractor miniMapIntreractor = null;
    public bool addForceOnObjectDetach = true;
    public float objPushForce = 20.0f;

    public HashSet<shapeItem_2> collidingWithHand;
    public bool isHovering = false;
    public bool circumferenceDisplayInUse = false;
    [SerializeField] private ClutterHandler_circumferenceDisplay clutterHandler_CircumferenceDisplay;

    public GameObject objectInHand;

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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (objectInHand || !other.GetComponent<shapeItem_2>())
            return;

        collidingWithHand.Add(other.GetComponent<shapeItem_2>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (objectInHand || !other.GetComponent<shapeItem_2>())
            return;

        collidingWithHand.Remove(other.GetComponent<shapeItem_2>());
    }

    private void OnTriggerStay(Collider col)
    {
        if (objectInHand == null
            && grabActionReference.action.WasPressedThisFrame()
            && (col.GetComponent<shapeItem_2>() || col.GetComponent<shapeItem_3>() || col.GetComponent<Interactable>()))
        {
            shapeItem_2 shapeItem2_parent;
            if (col.GetComponent<shapeItem_3>())
            {
                GameObject original = col.GetComponent<shapeItem_3>().original;
                shapeItem2_parent = col.GetComponent<shapeItem_3>().shapeItem2_parent;
                miniMap.RemoveFromMinimapUponGrab(shapeItem2_parent);
                PickupObject(original);
                collidingWithHand.Remove(shapeItem2_parent);
                clutterHandler_CircumferenceDisplay.FreeCircularSlots();
            }

            shapeItem2_parent = col.GetComponent<shapeItem_2>();
            if (shapeItem2_parent)
            {
                GameObject original = col.GetComponent<shapeItem_2>().original;
                miniMap.RemoveFromMinimapUponGrab(col.GetComponent<shapeItem_2>());
                PickupObject(original);
                collidingWithHand.Remove(shapeItem2_parent);

                if (circumferenceDisplayInUse)
                {
                    clutterHandler_CircumferenceDisplay.FreeCircularSlots();
                }
            }

            // in this case we must have  an original 
            if(col.GetComponent<shapeItem_2>() == null && col.GetComponent<shapeItem_3>() == null)
            {
                GameObject original = col.gameObject;
                RayManager.HoldRayCastHitCollider.Remove(col.gameObject);
                PickupObject(original);

                RayManager.releaseObjectsBackToOriginalPosition();

                if (circumferenceDisplayInUse)
                {
                    clutterHandler_CircumferenceDisplay.FreeCircularSlots();
                }
            }

            
        }
    }


    public void PickupObject(GameObject o)
    {
        o.transform.SetPositionAndRotation(attachTransform.position, attachTransform.rotation);
        o.transform.parent = attachTransform;
        o.GetComponent<Rigidbody>().useGravity = false;
        o.GetComponent<Rigidbody>().isKinematic = true;
        objectInHand = o;
    }

    private void ReleaseCurrentlyHeldObject()
    {
        if (objectInHand == null)
            return;

        objectInHand.transform.parent = null;

        if (addForceOnObjectDetach)
        {
            Vector3 applyForce = objectInHand.transform.forward * objPushForce;
            objectInHand.GetComponent<Rigidbody>().AddForce(applyForce, ForceMode.Impulse);
        }

        objectInHand = null;
    }
}