using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    [SerializeField] private Transform centerOfMiniMap;

    // make a radius variable that is public and can be change in the unity editor
    [SerializeField] private float radius = 0.2f;

    private List<shapeItem_2> listInCircle;

    private Vector3 trashLocation = new Vector3(1000, 1000, 1000);

    private bool isFrozen = false;

    // Start is called before the first frame update
    private void Start()
    {
        listInCircle = new List<shapeItem_2>();

        if (!centerOfMiniMap)
            centerOfMiniMap = transform.GetChild(0).gameObject.transform;
    }

    public void RemoveFromMinimapUponGrab(GameObject toBeRemoved)
    {
        MoveToTrash(toBeRemoved);
    }

    // Update is called once per frame
    private void Update()
    {
        if (isFrozen)
            return;

        ClearObjectCopies();
        DisplayObjectCopies();
    }

    private void DisplayObjectCopies()
    {
        List<(shapeItem_2 s, Vector3 dir)> ShapeItems_Directions = MiniMapInteractor.GetDuplicatesAndDirections();

        foreach (var shapeItem_Dir in ShapeItems_Directions)
        {
            RenderObjectInDirectionOnMinimap(shapeItem_Dir);
            listInCircle.Add(shapeItem_Dir.s.GetComponent<shapeItem_2>());
        }
    }

    private void ClearObjectCopies()
    {
        foreach (shapeItem_2 o in listInCircle)
        {
            o.inCircle = false;
            o.currentMap = null;

            // Deactivating causes multiple triggers, so move far away instead
            MoveToTrash(o.gameObject);
        }

        listInCircle.Clear();
    }

    public void RenderObjectInDirectionOnMinimap((shapeItem_2 si, Vector3 dir) shapeItem_Dir)
    {
        GameObject shapeItemObject = shapeItem_Dir.si.gameObject;
        Vector3 dir = shapeItem_Dir.dir;

        shapeItemObject.SetActive(true);

        dir *= radius;

        shapeItemObject.transform.position = centerOfMiniMap.transform.position;
        shapeItemObject.transform.position += transform.TransformDirection(dir);

        shapeItemObject.GetComponent<shapeItem_2>().currentMap = this;

        shapeItemObject.transform.rotation = transform.rotation;

        if (shapeItemObject.tag == "infinity")
        {
            shapeItemObject.transform.Rotate(Vector3.right, 90f);
        }

        if (shapeItemObject.tag == "pyramid")
        {
            shapeItemObject.transform.Rotate(Vector3.right, -90f);
        }

        shapeItemObject.transform.parent = centerOfMiniMap.transform;
    }

    private void MoveToTrash(GameObject o)
    {
        o.transform.position = trashLocation;
    }

    public void ShowMiniMap()
    {
        isFrozen = false;
        gameObject.SetActive(true);
    }

    public void FreezeMiniMap()
    {
        isFrozen = true;
    }

    public void CloseMiniMap()
    {
        isFrozen = false;
        ClearObjectCopies();
        gameObject.SetActive(false);
    }
}