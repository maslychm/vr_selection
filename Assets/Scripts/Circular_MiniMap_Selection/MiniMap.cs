using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class MiniMap : MonoBehaviour
{
    [SerializeField] private Transform centerOfMiniMap;

    [SerializeField] private InputActionReference writeFileActionReference;

    // make a radius variable that is public and can be change in the unity editor
    [SerializeField] private float radius = 0.2f;

    private List<shapeItem_2> listInCircle;

    private Vector3 trashLocation = new Vector3(1000, 1000, 1000);

    private bool isFrozen = false;

    [SerializeField] private MiniMapInteractor miniMapInteractor;

    private void Start()
    {
        listInCircle = new List<shapeItem_2>();

        if (!centerOfMiniMap)
            centerOfMiniMap = transform.GetChild(0).gameObject.transform;
    }

    /// <summary>
    /// ShapeItem2 which to be teleported away from the minimap.
    /// Should correspond to the original one being grabbed.
    /// </summary>
    /// <param name="toBeRemoved"></param>
    public void RemoveFromMinimapUponGrab(shapeItem_2 toBeRemoved)
    {
        MoveToTrash(toBeRemoved.gameObject);
    }

    private void Update()
    {
        if (isFrozen)
            return;

        ClearObjectCopies();
        DisplayObjectCopies();

        if (writeFileActionReference.action.WasPressedThisFrame())
        {
            StartCoroutine(WriteShapesAndDirectionsToFile(miniMapInteractor.GetDuplicatesAndDirections()));
        }
    }

    private void DisplayObjectCopies()
    {
        List<(shapeItem_2 s, Vector3 dir)> ShapeItems_Directions = miniMapInteractor.GetDuplicatesAndDirections();

        foreach (var shapeItem_Dir in ShapeItems_Directions)
        {
            RenderObjectInDirectionOnMinimap(shapeItem_Dir);
            listInCircle.Add(shapeItem_Dir.s);
        }
    }

    private IEnumerator WriteShapesAndDirectionsToFile(List<(shapeItem_2 s, Vector3 dir)> ShapeItems_Directions)
    {
        var folder = Application.streamingAssetsPath;
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

        var dateStr = DateTime.Now.ToString("d").Replace("/", "-");
        var timeStr = DateTime.Now.ToString("T").Replace(" ", "");
        var secondStr = DateTime.Now.ToString("ss").Replace(" ", "");
        var fileName = $"shape_directions_{dateStr}_{timeStr}".Replace(":", "-");
        var filePath = Path.Combine(folder, fileName + ".txt");

        using (var writer = new StreamWriter(filePath, true))
        {
            foreach (var (s, dir) in ShapeItems_Directions)
            {
                writer.WriteLineAsync($"{s.name.Replace(" ", "_")}, {dir.x}, {dir.y}, {dir.z}");
            }

            print($"Wrote TXT to: {filePath}");
        }

        yield return null;
    }

    public void ClearObjectCopies()
    {
        foreach (shapeItem_2 o in listInCircle)
        {
            if (o == null) continue;

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

        //shapeItemObject.transform.rotation = transform.rotation;

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