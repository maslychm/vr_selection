
using UnityEngine;

public class shapeItem : MonoBehaviour
{
    public bool inZone;
    public Vector3 zoneRotation = Vector3.zero;
    public Grid_Zone currentZone;

    void start()
    {
        inZone = false;
        currentZone = null;
    }

}
