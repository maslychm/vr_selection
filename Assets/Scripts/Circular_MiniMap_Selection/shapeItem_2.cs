using UnityEngine;

public class shapeItem_2 : MonoBehaviour
{
    public Interactable original;
    public cakeslice.Outline interactionOutline = null;
    public cakeslice.Outline targetOutline = null;

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

        interactionOutline.enabled = true;
        original.interactionOutline.enabled = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<GrabbingHand>())
            return;

        interactionOutline.enabled = false;
        original.interactionOutline.enabled = false;
    }
}