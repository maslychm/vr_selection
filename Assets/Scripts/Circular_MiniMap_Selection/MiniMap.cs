using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    
    // first let's set a variable to store the duplicates and their distance for every update 
    private List<(GameObject, Vector3)> m_Map;

    private Dictionary<GameObject, Transform> duplicates_and_originalPosition;

    private GameObject center_Circular_Minimap;

    Transform centreOFminiMap; // to store the position of the centre

    public static bool isActive = false;

    // store the parent of the circular mini map and store its anchor
    public GameObject MiniMap_Original;
    public GameObject Anchor_MiniMaporiginal;

    // make a radius variable that is public and can be change in the unity editor 
    [SerializeField] public float radius = 1.0f;

    // this variable will change accordingly as the code gets executed
    private Vector3 newOffset_for_current_ObjToBeInCircle;

    // for ease of use : ->
    private float adjusted_position_X;
    private float adjusted_position_Y;
    private float adjusted_position_Z;

    private List<shapeItem_2> listInCircle;

    // Start is called before the first frame update
    void Start()
    {
        m_Map = new List<(GameObject, Vector3)>();

        listInCircle = new List<shapeItem_2>();

        MiniMap_Original = this.gameObject;

        // get the center of the circle 
        center_Circular_Minimap = transform.GetChild(0).gameObject;
        //center_Circular_Minimap = MiniMap_Original.transform.parent.transform.GetChild(1).gameObject;


        // get the position of the center;
        centreOFminiMap = center_Circular_Minimap.transform;

    }

    // Update is called once per frame
    void Update()
    {
        foreach(shapeItem_2 o in listInCircle)
        {

           // o.gameObject.transform.SetParent(MiniMap_Original.transform, false);

            o.inCircle = false;

            o.currentMap = null;

            o.gameObject.SetActive(false);
        }

        listInCircle.Clear();
        // update the dictionary every frame
        m_Map = MiniMapInteractor.get_Duplicate_and_Direction();

        foreach (var temp in m_Map)
        {
            addToCircleMiniMap(temp);
            listInCircle.Add(temp.Item1.GetComponent<shapeItem_2>());
    
        }
    }

    public void addToCircleMiniMap((GameObject, Vector3) _objToBeInCircle2)
    {

        GameObject _objToBeInCircle = _objToBeInCircle2.Item1;
        _objToBeInCircle.gameObject.SetActive(true);

        // maybe for the rotation these should be adjusted and removed 

   
        //_objToBeInCircle.transform.SetParent(MiniMap_Original.transform, true);
/*
        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> Adjustment 1 <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        // instead of using 0 as a standard position adjuster we can directly spawn the object at the center of the circle
        _objToBeInCircle.transform.localPosition = center_Circular_Minimap.transform.localPosition;
        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> end of adj 1   <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
*/

       // _objToBeInCircle.transform.localPosition = Vector3.zero;

        // new additions here: 
        // in this process I will detail the variable usage and have helper variables to store the info 
        // just for usability later when we try and make this process dynamic :D
        {
            // now we can safely add more changes to the distance:

            //newOffset_for_current_ObjToBeInCircle = m_Map[_objToBeInCircle].normalized;
            newOffset_for_current_ObjToBeInCircle = _objToBeInCircle2.Item2;

            //debug ----------------------------
            print("newOffset_for_current_ObjToBeInCircle + " + newOffset_for_current_ObjToBeInCircle);

            // this should take care of multiplying with the radius
            newOffset_for_current_ObjToBeInCircle *= radius;

            // set the position variables for ease of use / to be on the circular mini_map
            // this is just for easy access later when debugging : D
            adjusted_position_X = newOffset_for_current_ObjToBeInCircle.x;
            adjusted_position_Y = newOffset_for_current_ObjToBeInCircle.y;
            adjusted_position_Z = newOffset_for_current_ObjToBeInCircle.z;

            // real positon setting process 
            //_objToBeInCircle.transform.localPosition = center_Circular_Minimap.transform.localPosition + newOffset_for_current_ObjToBeInCircle;
            _objToBeInCircle.transform.position = centreOFminiMap.transform.position;

            //_objToBeInCircle.transform.localPosition = center_Circular_Minimap.transform.localPosition + newOffset_for_current_ObjToBeInCircle;

           // _objToBeInCircle.transform.position += centreOFminiMap.transform.TransformPoint(newOffset_for_current_ObjToBeInCircle);
            _objToBeInCircle.transform.localPosition += (newOffset_for_current_ObjToBeInCircle);

            //debug-------------------------------
            print("here : _objToBeInCircle.transform.localPosition " + _objToBeInCircle.transform.localPosition);

        }
        // end of the core change for the current selection method


        _objToBeInCircle.transform.rotation = MiniMap_Original.transform.rotation;
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

}
