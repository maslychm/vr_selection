using UnityEngine;

public class shapeItem_2 : MonoBehaviour
{
    public Interactable original;
    public cakeslice.Outline interactionOutline = null;
    public cakeslice.Outline targetOutline = null;

    private void Start()
    {
        gameObject.layer = 10;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("GrabbingHand"))
            return;

        interactionOutline.enabled = true;
        original.interactionOutline.enabled = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("GrabbingHand"))
            return;

        interactionOutline.enabled = false;
        original.interactionOutline.enabled = false;
    }
}