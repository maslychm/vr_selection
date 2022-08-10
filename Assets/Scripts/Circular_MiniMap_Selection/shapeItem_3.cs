using UnityEngine;

/// <summary>
/// this matches the shapeItem2 version yet I think this still needs some changes maybe?
/// </summary>

public class shapeItem_3 : MonoBehaviour
{
    public GameObject original;
    public shapeItem_2 shapeItem2_parent;

    private void Start()
    {
        gameObject.layer = 10;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<GrabbingHand>() || other.GetComponent<GrabbingHand>().objectInHand)
            return;

        GetComponent<cakeslice.Outline>().enabled = true;
        shapeItem2_parent.GetComponent<cakeslice.Outline>().enabled = true;
        original.GetComponent<cakeslice.Outline>().enabled = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<GrabbingHand>() || other.GetComponent<GrabbingHand>().objectInHand)
            return;

        GetComponent<cakeslice.Outline>().enabled = false;
        shapeItem2_parent.GetComponent<cakeslice.Outline>().enabled = false;
        original.GetComponent<cakeslice.Outline>().enabled = false;
    }
}