using UnityEngine;
using UnityEngine.InputSystem;

public class GrabbingHand : MonoBehaviour
{
    [SerializeField] private InputActionReference grabActionReference;
    [SerializeField] private Transform attachTransform;

    public MiniMap temp;
    public MiniMapInteractor temp2;

    public bool addForceOnObjectDetach = true;
    public float objPushForce = 20.0f;

    public GameObject objectInHand;

    private void Start()
    {
        objectInHand = null;
    }

    private void Update()
    {
        if (grabActionReference.action.WasPressedThisFrame()) { }

        if (grabActionReference.action.WasReleasedThisFrame())
        {
            ReleaseCurrentlyHeldObject();
        }
    }

    private void OnTriggerStay(Collider col)
    {
        if (objectInHand == null && grabActionReference.action.WasPressedThisFrame() && col.gameObject.GetComponent<shapeItem_2>())
        {
            GameObject original = col.gameObject.GetComponent<shapeItem_2>().original;
            GameObject duplicate = col.gameObject;
            temp.RemoveFromMinimapUponGrab(duplicate);

            PickupObject(original);

            objectInHand = original;
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