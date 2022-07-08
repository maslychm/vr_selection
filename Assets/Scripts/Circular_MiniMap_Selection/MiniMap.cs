using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    [SerializeField] private Transform centerOfMiniMap; // to store the position of the centre

    // first let's set a variable to store the duplicates and their distance for every update
    private List<(GameObject, Vector3)> m_Map;

    // make a radius variable that is public and can be change in the unity editor
    [SerializeField] public float radius = 0.2f;

    // this variable will change accordingly as the code gets executed
    private Vector3 newOffset_for_current_ObjToBeInCircle;

    private float adjusted_position_X;
    private float adjusted_position_Y;
    private float adjusted_position_Z;

    private List<shapeItem_2> listInCircle;

    private Vector3 trashLocation = new Vector3(1000, 1000, 1000);

    // Start is called before the first frame update
    private void Start()
    {
        m_Map = new List<(GameObject, Vector3)>();

        listInCircle = new List<shapeItem_2>();

        // get the position of the center;
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
        ClearObjectCopies();
        DisplayObjectCopies();
    }

    private void DisplayObjectCopies()
    {
        // update the dictionary every frame
        m_Map = MiniMapInteractor.GetDuplicatesAndDirections();

        foreach (var temp in m_Map)
        {
            RenderObjectInDirectionOnMinimap(temp);
            listInCircle.Add(temp.Item1.GetComponent<shapeItem_2>());
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

    public void RenderObjectInDirectionOnMinimap((GameObject o, Vector3 dir) _objToBeInCircle2)
    {
        GameObject _objToBeInCircle = _objToBeInCircle2.o;

        newOffset_for_current_ObjToBeInCircle = _objToBeInCircle2.dir;

        _objToBeInCircle.gameObject.SetActive(true);

        newOffset_for_current_ObjToBeInCircle *= radius;

        // set the position variables for ease of use / to be on the circular mini_map
        // this is just for easy access later when debugging : D
        adjusted_position_X = newOffset_for_current_ObjToBeInCircle.x;
        adjusted_position_Y = newOffset_for_current_ObjToBeInCircle.y;
        adjusted_position_Z = newOffset_for_current_ObjToBeInCircle.z;

        _objToBeInCircle.transform.position = centerOfMiniMap.transform.position;
        _objToBeInCircle.transform.position += transform.TransformDirection(newOffset_for_current_ObjToBeInCircle);

        _objToBeInCircle.GetComponent<shapeItem_2>().currentMap = this;

        _objToBeInCircle.transform.rotation = transform.rotation;

        if (_objToBeInCircle.tag == "infinity")
        {
            _objToBeInCircle.transform.Rotate(Vector3.right, 90f);
        }

        if (_objToBeInCircle.tag == "pyramid")
        {
            _objToBeInCircle.transform.Rotate(Vector3.right, -90f);
        }
    }

    private void MoveToTrash(GameObject o)
    {
        o.transform.position = trashLocation;
    }

    public void ShowMiniMap()
    {
        gameObject.SetActive(true);
    }

    public void CloseMiniMap()
    {
        ClearObjectCopies();
        gameObject.SetActive(false);
    }
}