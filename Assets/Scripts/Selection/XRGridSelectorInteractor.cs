using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/**
 * This script must be attached to the hand which will be holding the flashlight
 */

/// add the update here 

public class XRGridSelectorInteractor : MonoBehaviour
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

    // declare the dictionary 
    // initial one (key) will be a game object that contains the gesture interactable compoenent 
    // the second one (value) will be the game object that has the shape item component 
    Dictionary<GameObject, GameObject> origin_and_duplicate_registery;

    // declare an extra dictionary to hold the item and its zone-that holds it 
    // item is the key
    Dictionary<GameObject, GameObject> zone_plus_its_item;

    public GridSelection_Initial temp;
    public void Start()
    {

        // initialize the dictionary 
        origin_and_duplicate_registery = new Dictionary<GameObject, GameObject>();

        // initialize the dictionary for the zone and its items 
        zone_plus_its_item = new Dictionary<GameObject, GameObject>();

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

            // add item shape script componet 
            temp.AddComponent<shapeItem>();

            // add to dictionary (still need to figure out how this is clearly making the items distinguishable later!!! )
            origin_and_duplicate_registery.Add(_original, temp);

        }
    }

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
    }

    // -----------------------------------------------------------------------------------------------------------

    private void Update()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        
        if (flaslightActionReference.action.WasPressedThisFrame())
        {
            ExtendFlashlight();
            temp.showMiniMap(true);
            
        }

        if (flaslightActionReference.action.IsPressed())
        {
            UpdateObjectScale();
            temp.showMiniMap(true);
        }

        if (flaslightActionReference.action.WasReleasedThisFrame())
        {
            temp.closeMiniMap(true);
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

        flashlightHighlighter.transform.localScale = defaultFlashlightScale;

        flashlightCenterCone.SetActive(true);
    }

    private void ShrinkFlashlight()
    {

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

    #region CALLABLE BY INTERACTABLES

    public void AddtoHighlighted(GameObject o)
    {

        // mention to Mykola: -> another simple way to iedntify a duplicate is to use a UUID or GUID
        //                      without having to check for the component presence 


        allHighlightedObjects.Add(o);

        // get the corresponding game object from the dictionary (shape item)
        GameObject tobeInserted = origin_and_duplicate_registery[o];

        Grid_Inventory_Manager helper = FindObjectOfType<Grid_Inventory_Manager>();

        // get the next available zone for insertion 
        GameObject _availableZone = helper.getAvailablePositions();

        // assign both the zone and shape to dictionary 
        // if there is actually a zone to insert to 
        if (_availableZone != null)
        {
            if (!zone_plus_its_item.ContainsKey(tobeInserted))
            {
                zone_plus_its_item.Add(tobeInserted, _availableZone);
 
                _availableZone.GetComponent<Grid_Zone>().InsertItem(tobeInserted);
            }
            
        }
        
    }

    public void RemoveFromHighlighted(GameObject o)
    {
        //Debug.Log(o.tag + " " + o.name );
        allHighlightedObjects.Remove(o);

        GameObject toberemoved = origin_and_duplicate_registery[o];
        if (zone_plus_its_item.ContainsKey(toberemoved))
        {
            GameObject zoneHoldingIt = zone_plus_its_item[toberemoved];

            zoneHoldingIt.GetComponent<Grid_Zone>().removeFromZone(toberemoved);

            //remove the itemn from the dict
            //zone_plus_its_item[toberemoved] = null;
            zone_plus_its_item.Remove(toberemoved);
        }

    }

    #endregion CALLABLE BY INTERACTABLES

    #region DEBUG

    private void dprint(string msg)
    {
        if (debug) print(msg);
    }

    #endregion DEBUG
}