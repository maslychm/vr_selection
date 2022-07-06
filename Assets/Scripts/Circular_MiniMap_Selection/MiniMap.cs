using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    [SerializeField] private Transform centerOfMiniMap; // to store the position of the centre

    // first let's set a variable to store the duplicates and their distance for every update
    private List<(GameObject, Vector3)> m_Map;

    // make a radius variable that is public and can be change in the unity editor
    [SerializeField] public float radius = 1.0f;

    // this variable will change accordingly as the code gets executed
    private Vector3 newOffset_for_current_ObjToBeInCircle;

    private float adjusted_position_X;
    private float adjusted_position_Y;
    private float adjusted_position_Z;

    private List<shapeItem_2> listInCircle;

    // Start is called before the first frame update
    private void Start()
    {
        m_Map = new List<(GameObject, Vector3)>();

        listInCircle = new List<shapeItem_2>();

        // get the position of the center;
        if (!centerOfMiniMap)
            centerOfMiniMap = transform.GetChild(0).gameObject.transform;
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
            addToCircleMiniMap(temp);
            listInCircle.Add(temp.Item1.GetComponent<shapeItem_2>());
        }
    }

    private void ClearObjectCopies()
    {
        foreach (shapeItem_2 o in listInCircle)
        {
            o.inCircle = false;
            o.currentMap = null;
            o.gameObject.SetActive(false);
        }

        listInCircle.Clear();
    }

    public void addToCircleMiniMap((GameObject, Vector3) _objToBeInCircle2)
    {
        GameObject _objToBeInCircle = _objToBeInCircle2.Item1;
        _objToBeInCircle.gameObject.SetActive(true);

        //_objToBeInCircle.transform.SetParent(MiniMap_Original.transform, true);
        /*
                // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> Adjustment 1 <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
                // instead of using 0 as a standard position adjuster we can directly spawn the object at the center of the circle
                _objToBeInCircle.transform.localPosition = center_Circular_Minimap.transform.localPosition;
                // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> end of adj 1   <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        */

        // new additions here:
        // in this process I will detail the variable usage and have helper variables to store the info
        // just for usability later when we try and make this process dynamic :D
        {
            // now we can safely add more changes to the distance:

            //newOffset_for_current_ObjToBeInCircle = m_Map[_objToBeInCircle].normalized;
            newOffset_for_current_ObjToBeInCircle = _objToBeInCircle2.Item2;

            //debug ----------------------------
            //print("newOffset_for_current_ObjToBeInCircle + " + newOffset_for_current_ObjToBeInCircle);

            // this should take care of multiplying with the radius
            newOffset_for_current_ObjToBeInCircle *= radius;

            // set the position variables for ease of use / to be on the circular mini_map
            // this is just for easy access later when debugging : D
            adjusted_position_X = newOffset_for_current_ObjToBeInCircle.x;
            adjusted_position_Y = newOffset_for_current_ObjToBeInCircle.y;
            adjusted_position_Z = newOffset_for_current_ObjToBeInCircle.z;

            // real positon setting process
            //_objToBeInCircle.transform.localPosition = center_Circular_Minimap.transform.localPosition + newOffset_for_current_ObjToBeInCircle;
            _objToBeInCircle.transform.position = centerOfMiniMap.transform.position;

            //_objToBeInCircle.transform.localPosition = center_Circular_Minimap.transform.localPosition + newOffset_for_current_ObjToBeInCircle;

            // _objToBeInCircle.transform.position += centreOFminiMap.transform.TransformPoint(newOffset_for_current_ObjToBeInCircle);
            _objToBeInCircle.transform.localPosition += (newOffset_for_current_ObjToBeInCircle);

            //debug-------------------------------
            //print("here : _objToBeInCircle.transform.localPosition " + _objToBeInCircle.transform.localPosition);
        }
        // end of the core change for the current selection method

        _objToBeInCircle.transform.rotation = transform.rotation;
        _objToBeInCircle.GetComponent<shapeItem_2>().currentMap = this;

        if (_objToBeInCircle.tag == "infinity")
        {
            _objToBeInCircle.transform.localEulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
        }
    }

    // other potential scenario to be implemented
    /*
        The object you want to change the axis on I'll call ObjectX:

        create an empty GameObject
        make the empty GameObject the child of ObjectX
        reset the Transform of the empty GameObject (it should now center ObjectX)
        unparent the empty GameObject
        rotate the empty GameObject so that it's axis are the way you'd like them to be on ObjectX
        make the empty GameObject the parent of ObjectX

     * */

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