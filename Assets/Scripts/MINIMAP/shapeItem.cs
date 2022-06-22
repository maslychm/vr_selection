
using UnityEngine;


/// <summary>
///  this is just to reference item/shape properties later for ease of use by zone.cs when they are inside the mini map
///  -- Yohan
/// </summary>
public class shapeItem : MonoBehaviour
{
    public bool inZone;
    public Vector3 zoneRotation = Vector3.zero;
    public Zone currentZone; 

}
