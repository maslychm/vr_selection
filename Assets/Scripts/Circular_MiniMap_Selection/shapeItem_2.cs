using UnityEngine;

public class shapeItem_2 : MonoBehaviour
{
    public GameObject original;

    private void OnTriggerEnter(Collider other)
    {
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