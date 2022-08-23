using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SphereGrabbingTool : MonoBehaviour
{

    public GrabbingHand handHelper;
    private void OnTriggerEnter(Collider other)
    {
        handHelper._OnTriggerStay(other);
    }

    private void OnTriggerExit(Collider other)
    {
        handHelper._OnTriggerExit(other);
    }

    private void OnTriggerStay(Collider col)
    {
        handHelper._OnTriggerStay(col);
    }
}
