using UnityEngine;

public class shapeItem_2 : MonoBehaviour
{
    public bool inCircle;
    public Vector3 rotation = Vector3.zero;
    public MiniMap currentMap;

    private void Start()
    {
        inCircle = false;
        currentMap = null;
    }
}