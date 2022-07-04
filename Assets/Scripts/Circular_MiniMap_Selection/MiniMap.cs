using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{

    // first let's set a variable to store the duplicates and their distance for every update 
    private Dictionary<GameObject, Vector3> m_Map;

    private Dictionary<GameObject, Transform> duplicates_and_originalPosition;

    MiniMapInteractor m_Interactor; 

    private GameObject center_Circular_Minimap;

    Transform centreOFminiMap; // to store the position of the centre

    public static bool isActive = false;

    // store the parent of the circular mini map and its anchor
    private GameObject MiniMap_Original;
    public GameObject Anchor_MiniMaporiginal;

    // make a radius variable that is public and can be change in the unity editor 
    [SerializeField] public float radius;

    // this variable will change accordingly as the code gets executed
    private Vector3 newOffset_for_current_ObjToBeInCircle;

    // for ease of use : ->
    private float adjusted_position_X;
    private float adjusted_position_Y;
    private float adjusted_position_Z;

    // Start is called before the first frame update
    void Start()
    {
        m_Map = new Dictionary<GameObject, Vector3>();

        MiniMap_Original = this.gameObject;
        isActive = false;

        // get the center of the circle 

        // method 1:
        // center_Circular_Minimap = this.GetComponentInChildren<GameObject>();

        // method 2:
        center_Circular_Minimap = MiniMap_Original.transform.GetChild(0).gameObject;

        // get the position of the center;
        centreOFminiMap = center_Circular_Minimap.transform;

        // initially doesn't need to appear
        MiniMap_Original.SetActive(false);

        // let's set the radius initially to 1.00
        radius = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {

        // update the dictionary every frame
        m_Map = m_Interactor.get_Duplicate_and_Direction();

        // get the duplicates and their original positoon 
        duplicates_and_originalPosition = m_Interactor.get_Duplictae_and_originalPosition();

        foreach (var temp in duplicates_and_originalPosition)
        {
            if (m_Map.ContainsKey(temp.Key))
            {
                addToCircleMiniMap(temp.Key);
            }
            else
                removeFromCircle(temp.Key);
        }
    }

    public void showCircularMap(bool t)
    {
        if (t == true)
        {
            isActive = true;
            MiniMap_Original.SetActive(true);
        }

        if (isActive == true)
        {
            MiniMap_Original.transform.position = Anchor_MiniMaporiginal.transform.position;

            MiniMap_Original.transform.eulerAngles = new Vector3(Anchor_MiniMaporiginal.transform.eulerAngles.x + 15, Anchor_MiniMaporiginal.transform.eulerAngles.y, 0);

        }
    }

    public void closeCircularMap(bool t)
    {
        if(t == true)
        {
            isActive = false;
            MiniMap_Original.SetActive(false);
        }
    }

    public void removeFromCircle(GameObject _CurrentInCircle)
    {

        _CurrentInCircle.transform.SetParent(MiniMap_Original.transform, false);

        _CurrentInCircle.transform.position = duplicates_and_originalPosition[_CurrentInCircle].position;
        _CurrentInCircle.transform.eulerAngles = duplicates_and_originalPosition[_CurrentInCircle].eulerAngles;
        _CurrentInCircle.transform.rotation = duplicates_and_originalPosition[_CurrentInCircle].rotation;


        _CurrentInCircle.GetComponent<shapeItem_2>().inCircle = false;

        _CurrentInCircle.GetComponent<shapeItem_2>().currentMap = null;


    }

    public void addToCircleMiniMap(GameObject _objToBeInCircle)
    {
        if (_objToBeInCircle.tag == "star")
        {
            _objToBeInCircle.transform.localEulerAngles = new Vector3(180.0f, 0.0f, 0.0f);
        }
        if (_objToBeInCircle.tag == "pyramid")
        {
            _objToBeInCircle.transform.localEulerAngles = new Vector3(180.0f, 0.0f, 0.0f);

        }

        _objToBeInCircle.transform.SetParent(MiniMap_Original.transform, true);
        _objToBeInCircle.transform.localPosition = Vector3.zero;

        // new additions here: 
        // in this process I will detail the variable usage and have helper variables to store the info 
        // just for usability later when we try and make this process dynamic :D
        {
            // now we can safely add more changes to the distance:

            newOffset_for_current_ObjToBeInCircle = m_Map[_objToBeInCircle].normalized;

            // this should take care of multiplying with the radius
            newOffset_for_current_ObjToBeInCircle *= radius;

            // set the position variables for ease of use / to be on the circular mini_map
            // this is just for easy access later when debugging : D
            adjusted_position_X = newOffset_for_current_ObjToBeInCircle.x;
            adjusted_position_Y = newOffset_for_current_ObjToBeInCircle.y;
            adjusted_position_Z = newOffset_for_current_ObjToBeInCircle.z;

            // real positon setting process 
            _objToBeInCircle.transform.position = center_Circular_Minimap.transform.position + newOffset_for_current_ObjToBeInCircle;

        }
        // end of the core change for the current selection method


        _objToBeInCircle.transform.rotation = MiniMap_Original.transform.rotation;
        _objToBeInCircle.GetComponent<shapeItem_2>().currentMap = this;
        

        if (_objToBeInCircle.tag == "infinity")
        {
            _objToBeInCircle.transform.localEulerAngles = new Vector3(90.0f, 0.0f, 0.0f);

        }

    }

}
