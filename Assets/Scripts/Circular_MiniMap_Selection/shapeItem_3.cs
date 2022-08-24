using UnityEngine;

/// <summary>
/// this matches the shapeItem2 version yet I think this still needs some changes maybe?
/// </summary>

public class shapeItem_3 : MonoBehaviour
{
    public Interactable original;
    public shapeItem_2 shapeItem2_parent;
    public cakeslice.Outline interactionOutline = null;
    public cakeslice.Outline targetOutline = null;

    private void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("GrabbingHand"))
            return;

        interactionOutline.enabled = true;
        shapeItem2_parent.interactionOutline.enabled = true;
        original.interactionOutline.enabled = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("GrabbingHand"))
            return;

        interactionOutline.enabled = false;
        shapeItem2_parent.interactionOutline.enabled = false;
        original.interactionOutline.enabled = false;
    }
}