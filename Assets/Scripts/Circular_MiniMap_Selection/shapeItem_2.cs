using UnityEngine;

public class shapeItem_2 : MonoBehaviour
{
    public bool inCircle;
    public Vector3 rotation = Vector3.zero;
    public MiniMap currentMap;
    public GameObject original;

    private void Start()
    {
        inCircle = false;
        currentMap = null;
    }

    private void OnTriggerEnter(Collider other)
    {

        print("We reached On Trigger Enter");
        if (!other.GetComponent<GrabbingHand>())
        {
            return;
        }

        if (other.GetComponent<GrabbingHand>().objectInHand)
        {
            return;
        }
        GetComponent<cakeslice.Outline>().enabled = true;
        original.GetComponent<cakeslice.Outline>().enabled = true;

    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<GrabbingHand>())
            return;

        GetComponent<cakeslice.Outline>().enabled = false;
        original.GetComponent<cakeslice.Outline>().enabled = false;
    }
}