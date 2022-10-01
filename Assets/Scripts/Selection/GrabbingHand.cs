using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrabbingHand : MonoBehaviour
{
    [SerializeField] private InputActionReference grabActionReference;
    [SerializeField] private Transform attachTransform;

    public MiniMap miniMap = null;
    public MiniMapInteractor miniMapIntreractor = null;
    public PassInteractablesToGrid flowerCone = null;
    public bool addForceOnObjectDetach = true;
    public float objPushForce = 20.0f;

    public HashSet<shapeItem_2> collidingWithHand;
    public bool isHovering = false;
    public bool circumferenceDisplayInUse = false;
    [SerializeField] private ClutterHandler_circumferenceDisplay clutterHandler_CircumferenceDisplay;

    public RayKebabManager instanceOfRayManager = null;

    private Interactable objectInHand;
    private List<Interactable> grabbedByHandHistory;

    private void Awake()
    {
        objectInHand = null;
        collidingWithHand = new HashSet<shapeItem_2>();
        grabbedByHandHistory = new List<Interactable>();
    }

    public void ClearGrabbed()
    {
        ReleaseCurrentlyHeldObject();

        ResetItemHistory();

        if (grabbedByHandHistory == null || collidingWithHand == null || grabbedByHandHistory.Count == 0 || collidingWithHand.Count == 0)
            return;

        if (grabbedByHandHistory.Count > 0)
            grabbedByHandHistory.Clear();
        if (collidingWithHand.Count > 0)
            collidingWithHand.Clear();
    }

    private void Update()
    {
        if (grabActionReference.action.WasReleasedThisFrame())
        {
            ReleaseCurrentlyHeldObject();
        }

        isHovering = collidingWithHand.Count != 0;
    }

    public void _OnTriggerEnter(Collider other)
    {
        if (objectInHand || !other.GetComponent<shapeItem_2>())
            return;

        collidingWithHand.Add(other.GetComponent<shapeItem_2>());
    }

    public void _OnTriggerExit(Collider other)
    {
        if (objectInHand || !other.GetComponent<shapeItem_2>())
            return;

        collidingWithHand.Remove(other.GetComponent<shapeItem_2>());
    }

    public void _OnTriggerStay(Collider col)
    {
        if (objectInHand == null
            && grabActionReference.action.WasPressedThisFrame()
            && (col.GetComponent<shapeItem_2>()
                || col.GetComponent<shapeItem_3>()
                || col.GetComponent<Interactable>())
           )
        {
            shapeItem_2 shapeItem2_parent;
            if (col.GetComponent<shapeItem_3>())
            {
                Interactable original = col.GetComponent<shapeItem_3>().original;
                shapeItem2_parent = col.GetComponent<shapeItem_3>().shapeItem2_parent;
                miniMap.RemoveFromMinimapUponGrab(shapeItem2_parent);
                PickupObject(original);
                collidingWithHand.Remove(shapeItem2_parent);
                clutterHandler_CircumferenceDisplay.FreeCircularSlots();
            }

            shapeItem2_parent = col.GetComponent<shapeItem_2>();
            if (shapeItem2_parent)
            {
                Interactable original = col.GetComponent<shapeItem_2>().original;
                miniMap.RemoveFromMinimapUponGrab(col.GetComponent<shapeItem_2>());
                PickupObject(original);
                collidingWithHand.Remove(shapeItem2_parent);

                if (circumferenceDisplayInUse)
                {
                    clutterHandler_CircumferenceDisplay.FreeCircularSlots();
                }
            }

            // in this case we must have an original -> dealing with raycast
            if (col.GetComponent<shapeItem_2>() == null
                && col.GetComponent<shapeItem_3>() == null)
            {
                Interactable original = col.GetComponent<Interactable>();
                original.GetComponent<Object_collected>().ResetOriginalScale();
                PickupObject(original);

                if (instanceOfRayManager)
                {
                    instanceOfRayManager.RemoveOneInteractableFromKebabList(original);

                    if (original.TryGetComponent<TargetInteractable>(out _))
                    {
                        instanceOfRayManager.ReleaseInteractablesFromRay();
                    }
                }

                if (circumferenceDisplayInUse)
                {
                    clutterHandler_CircumferenceDisplay.FreeCircularSlots();
                }
            }
        }
    }

    private void PickupObject(Interactable o)
    {
        ReleaseCurrentlyHeldObject();

        o.GetComponent<Rigidbody>().useGravity = false;
        o.GetComponent<Rigidbody>().isKinematic = true;

        o.GetComponent<Object_collected>().ResetOriginalScale();
        o.transform.SetPositionAndRotation(attachTransform.position, attachTransform.rotation);
        o.transform.parent = attachTransform;
        objectInHand = o;
        grabbedByHandHistory.Add(o);
        o.OnSelect();
    }

    public void CallPickUpObject(Interactable o)
    {
        PickupObject(o);
    }

    public void ResetItemHistory()
    {
        grabbedByHandHistory.ForEach(x => x.GetComponent<Object_collected>().ResetGameObject());
        grabbedByHandHistory.Clear();
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