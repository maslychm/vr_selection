using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/**
 * This script must be attached to the hand which will be holding the flashlight
 * This script represents the Mini Map (circular) selector Interactor
 */

/// add the update here 

public class MiniMapInteractor : MonoBehaviour
{
    [Header("Use gesture shape, or direction")]
    public bool useGestureDirection = false;

    public GameObject currentShapeParent;

    [SerializeField] private InputActionReference flaslightActionReference;

    [Tooltip("This object must have a collider and be tagged as GestureFilter")]
    [SerializeField] private GameObject flashlightHighlighter;

    [SerializeField] private Transform attachTransform;

    [Header("Gesture Direction technique-related")]
    [SerializeField] private GameObject handDebugPlane;

    [SerializeField] private GameObject flashlightCenterCone;

    [SerializeField] private Vector3 debugPlaneOffset;

    // Head hmd
    [SerializeField] private Transform hmdTransform;

    [Header("Debugging and UI")]
    [SerializeField] private TabletUI tabletUI;

    private List<GameObject> allHighlightedObjects;

    private static bool isHighlighting = false;
    private GameObject selectedObject;

    [Header("Flashlight Scaling")]
    [SerializeField] private Vector3 defaultFlashlightScale;

    [Header("Dynamic Scaling")]
    [SerializeField] private bool scaleWithDistance;

    public Vector3 shoulderOffset;

    public Vector3 maxFlashlightScale;

    [Header("Other")]
    public bool debug = false;

    public bool addForceOnObjectDetach = false;
    public float objPushForce = 20.0f;

    // add a helper 
    public XRGestureInteractable ListOfMeshRenderers_Getter;

    Vector3 temp; // simple helper to store the distance later in our dictionary 

    // declare the dictionary 
    // initial one (key) will be a game object that contains the gesture interactable compoenent 
    // the second one (value) will be the game object that has the shape item component 
    Dictionary<GameObject, GameObject> origin_and_duplicate_registery;

    // declare an extra dictionary to hold the item and its zone-that holds it 
    // item is the key
    // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>This one might not be needed >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
    // Dictionary<GameObject, GameObject> zone_plus_its_item;
    // public GridSelection_Initial temp;
    // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    // declare a dictionary to hold the duplicates and their distances
    // set it to private for now 
    private static List<(GameObject, Vector3)> duplicateObject_and_distance_registery;
    private static Dictionary<GameObject, Transform> duplicate_and_originalPosition;

    public MiniMapCircular_Selection_initial _availableCircle;

    public void Start()
    {

        // initialize the dictionary 
        origin_and_duplicate_registery = new Dictionary<GameObject, GameObject>();

        // initilizer the object and distance dictionary 
        duplicateObject_and_distance_registery = new List<(GameObject, Vector3)>();

        duplicate_and_originalPosition = new Dictionary<GameObject, Transform>();

        // initialize the dictionary for the zone and its items 
        // zone_plus_its_item = new Dictionary<GameObject, GameObject>();

        // duplicates and remove interactable 
        // plus adding the shapeItem component

        // Method 1 --------------------
        // uses a parent and works by accessing children and so on 
        // linear time 
        // space efficient 

        _DuplicationMethod1();


        //---------------------------
        //highlightedObjectsByType = new Dictionary<string, List<GameObject>>();
        allHighlightedObjects = new List<GameObject>();
        // Pre-populate for O(1) type access
        //foreach (var s in SelectionConstants.objTypeNames)
           // highlightedObjectsByType.Add(s, new List<GameObject>());

        if (flashlightHighlighter == null)
            flashlightHighlighter = GameObject.Find("FlashlightCone");

        defaultFlashlightScale = new Vector3(150, 150, 560);

        ShrinkFlashlight();
        SetRecognizerMode();

        if (tabletUI)
            tabletUI.SetTabletActive(debug);
    }

    // refer to start function for explanation 
    // ----------------------------------------------------------------------------------------------------------
    public void _DuplicationMethod1 ()
    {
        // first we need to get the number of shapes that can be duplicaed 
        int children = currentShapeParent.transform.childCount;

        // --- duplicate---
        // and destroy the extra script coponent 
        for (int i = 0; i < children; i++)
        {
            GameObject _original = currentShapeParent.transform.GetChild(i).gameObject;
            GameObject temp = Instantiate(_original);
            Component scriptToBeDestroyed = temp.GetComponent<XRGestureInteractable>();
            Destroy(scriptToBeDestroyed);

            // extra items for deletion
            //temp.GetComponent<Rigidbody>().isKinematic = false;
            if (temp.tag == "star")
            {

                temp.transform.localScale = new Vector3(20, 20, 20);
              
            }
            if (temp.tag == "pyramid")
            {
                temp.transform.localScale = new Vector3(5, 5, 5);

               
            }
            if (temp.tag != "star" && temp.tag != "pyramid")
            {
                if (temp.tag == "infinity")
                {
                    temp.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

                }
                else
                    temp.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }

            Destroy(temp.GetComponent<Rigidbody>());
            Destroy(temp.GetComponent<Collider>());
            Destroy(temp.GetComponent<Object_collected>());

            // add item shape script component
            // modified this....................
            // to differentiate from the GridSelection Process
            temp.AddComponent<shapeItem_2>();

            // add to dictionary (still need to figure out how this is clearly making the items distinguishable later!!! )
            origin_and_duplicate_registery.Add(_original, temp);


            //store the duplicate and its original position 
            duplicate_and_originalPosition.Add(temp, _original.transform);

        }
    }
    /*
    public void _DuplicationMethod2 ()
    {
        List<MeshRenderer> temp2 = ListOfMeshRenderers_Getter.getListOfAllObjects();

        for(int i = 0; i < temp2.Count; i++)
        {

            GameObject _original2 = temp2[i].gameObject;
            GameObject _temp = Instantiate(_original2);
            Component _scriptToBeDestroyed = _temp.GetComponent<XRGestureInteractable>();
            Destroy(_scriptToBeDestroyed);

            // add the item shape compoenent 
            _temp.AddComponent<shapeItem>();

            // add to dictionary (still need to figure out how this is clearly making the items distinguishable later!!! )
            origin_and_duplicate_registery.Add(_original2, _temp);
        }
    }*/

    // -----------------------------------------------------------------------------------------------------------

    private void Update()
    {
        ProcessInput();
        updateList(allHighlightedObjects);
    }

    private void ProcessInput()
    {
        // make changes to allow the presend and abscense of the circular mini map in a 
        // controlled way 
        if (flaslightActionReference.action.WasPressedThisFrame())
        {
            ExtendFlashlight();

            _availableCircle.showMiniMap(true);
            
        }

        if (flaslightActionReference.action.IsPressed())
        {
            UpdateObjectScale();

            _availableCircle.showMiniMap(true);
        }

        if (flaslightActionReference.action.WasReleasedThisFrame())
        {
            _availableCircle.closeMiniMap(true);

            ShrinkFlashlight();
            ReleaseSelectedObject();
        }
    }

    private void SetRecognizerMode()
    {
        Gestures.Recognizer recognizer = FindObjectOfType<Gestures.Recognizer>();
        recognizer.useAsDirection = useGestureDirection;
    }

    private void ReleaseSelectedObject()
    {
        if (selectedObject == null)
            return;

        selectedObject.transform.parent = null;
        selectedObject.GetComponent<Rigidbody>().useGravity = true;
        selectedObject.GetComponent<Rigidbody>().isKinematic = false;

        if (addForceOnObjectDetach)
        {
            Vector3 applyForce = selectedObject.transform.forward * objPushForce;
            selectedObject.GetComponent<Rigidbody>().AddForce(applyForce, ForceMode.Impulse);
        }

        selectedObject = null;        
    }

    private void PickupObject(GameObject o)
    {
        o.transform.SetPositionAndRotation(attachTransform.position, attachTransform.rotation);
        o.transform.parent = attachTransform;
        o.GetComponent<Rigidbody>().useGravity = false;
        o.GetComponent<Rigidbody>().isKinematic = true;
    }

    private void ExtendFlashlight()
    {
        isHighlighting = true;
        flashlightHighlighter.transform.localScale = defaultFlashlightScale;

        //if (useGestureDirection) flashlightCenterCone.SetActive(true);
        flashlightCenterCone.SetActive(true);
    }

    private void ShrinkFlashlight()
    {
        isHighlighting = false;
        flashlightHighlighter.transform.localScale = new Vector3(0, 0, 0);
        flashlightCenterCone.SetActive(false);

        // Clear hovered list
        allHighlightedObjects.Clear();

    }

    private void UpdateObjectScale()
    {
        if (!scaleWithDistance)
            return;

        var shoulderInWorld = hmdTransform.TransformPoint(shoulderOffset);

        print($"head: {hmdTransform.position} shoulder: {shoulderInWorld}");

        float distHand = Vector3.Distance((hmdTransform.position - shoulderInWorld), transform.position);
        flashlightHighlighter.transform.localScale = (1 - Mathf.Abs(distHand) * 1.66667f) * maxFlashlightScale;

        print($"distHand: {distHand}");
    }

    // create a getter to enable access to this list
    public static List<(GameObject, Vector3)> get_Duplicate_and_Direction()
    {
        return duplicateObject_and_distance_registery;

    }

    public static Dictionary<GameObject, Transform> get_Duplictae_and_originalPosition()
    {
        return duplicate_and_originalPosition;
    }
    #region CALLABLE BY INTERACTABLES

    public void updateList(List<GameObject> temp2)
    {
        duplicateObject_and_distance_registery.Clear();
        foreach (GameObject o in temp2)
        {
            // -----------------------Get the Distance here and then store it in the dictionary--------------------------
            GameObject tobeInserted_Duplicate = origin_and_duplicate_registery[o];

            //(GameObject obj, float score) bestObject = (null, -2f);

            // work with one single object at a time as they are added 
            if (tobeInserted_Duplicate != null && o != null)
            {
                // highlighted object in flashlight's corrdinate system
                var objectPositionInFlashlightCoords = transform.InverseTransformPoint(o.transform.position);
                objectPositionInFlashlightCoords.z = 0f;
                objectPositionInFlashlightCoords.Normalize();

                // add a temp helper
                temp = objectPositionInFlashlightCoords;

            }

             duplicateObject_and_distance_registery.Add((tobeInserted_Duplicate, temp));
        }

    }

    public void AddtoHighlighted(GameObject o)
    {


        allHighlightedObjects.Add(o);

        updateList(allHighlightedObjects);

        // -----------------------Get the Distance here and then store it in the dictionary--------------------------
       /* GameObject tobeInserted_Duplicate = origin_and_duplicate_registery[o];

        //(GameObject obj, float score) bestObject = (null, -2f);

        // work with one single object at a time as they are added 
        if(tobeInserted_Duplicate != null && o != null)
        {
            // highlighted object in flashlight's corrdinate system
            var objectPositionInFlashlightCoords = transform.InverseTransformPoint(o.transform.position);
            objectPositionInFlashlightCoords.z = 0f;
            objectPositionInFlashlightCoords.Normalize();

            // add a temp helper
            temp = objectPositionInFlashlightCoords;

            // Dot product to measure alignment between passed direction and object's projected direction
           /* var dot = Vector3.Dot(direction, objectPositionInFlashlightCoords);
            if (dot > bestObject.score)
            {
                bestObject.obj = o;
                bestObject.score = dot;
            }*/
        

        // store the duplicate and the distance to be used
       /* if (!duplicateObject_and_distance_registery.ContainsKey(tobeInserted_Duplicate))

            duplicateObject_and_distance_registery.Add(tobeInserted_Duplicate, temp);
        */
        // ----------------------------------------------------------------------------------------------------------

         //_availableCircle.addToCircleMiniMap(tobeInserted_Duplicate);
        
        
    }

    public void RemoveFromHighlighted(GameObject o)
    {
        Debug.Log(o.tag + " " + o.name );
        allHighlightedObjects.Remove(o);

        GameObject toberemoved = origin_and_duplicate_registery[o];

        //duplicateObject_and_distance_registery.Remove(toberemoved);

        //_availableCircle.removeFromCircle(toberemoved);

    }

    #endregion CALLABLE BY INTERACTABLES

    #region DEBUG

    private void dprint(string msg)
    {
        if (debug) print(msg);
    }

    #endregion DEBUG
}