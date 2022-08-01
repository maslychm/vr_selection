using UnityEngine;

/// <summary>
/// this matches the shapeItem2 version yet I think this still needs some changes maybe?
/// </summary>

public class shapeItem_3 : MonoBehaviour
{
    public bool inCircle;
    public Vector3 rotation = Vector3.zero;
    public MiniMap currentMap;
    public GameObject original;

    private void Start()
    {
        inCircle = false;
        currentMap = null;
        this.gameObject.layer = 10;
    }

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
        original.GetComponent<shapeItem_2>().original.GetComponent<cakeslice.Outline>().enabled = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<GrabbingHand>())
            return;

        GetComponent<cakeslice.Outline>().enabled = false;
        original.GetComponent<cakeslice.Outline>().enabled = false;
        original.GetComponent<shapeItem_2>().original.GetComponent<cakeslice.Outline>().enabled = false;
    }
}