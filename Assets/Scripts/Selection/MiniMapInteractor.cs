using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

/**
 * This script must be attached to the hand which will be holding the flashlight
 * This script represents the Mini Map (circular) selector Interactor
 */

public class MiniMapInteractor : MonoBehaviour
{
    [Header("Use gesture shape, or direction")]
    public bool useGestureDirection = false;

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

    [Header("MiniMap Selector variables")]
    public MiniMap miniMap;

    [SerializeField] private bool normalizeOffsets = false;

    private Vector3 temp; // simple helper to store the distance later in our dictionary

    // declare the dictionary
    // initial one (key) will be a game object that contains the gesture interactable compoenent
    // the second one (value) will be the game object that has the shape item component
    private Dictionary<GameObject, GameObject> originalToDuplicate;

    // declare a dictionary to hold the duplicates and their distances
    // set it to private for now
    private static List<(GameObject, Vector3)> duplicateDirections;

    private static Dictionary<GameObject, Transform> duplicate_and_originalPosition;

    private List<string> interactableTags = new List<string>() { "cube", "sphere", "star", "pyramid", "cylinder", "infinity" };


    public void Start()
    {
        originalToDuplicate = new Dictionary<GameObject, GameObject>();
        duplicateDirections = new List<(GameObject, Vector3)>();
        duplicate_and_originalPosition = new Dictionary<GameObject, Transform>();

        CreateDuplicatesForMiniMap();

        allHighlightedObjects = new List<GameObject>();

        if (flashlightHighlighter == null)
            flashlightHighlighter = GameObject.Find("FlashlightCone");

        defaultFlashlightScale = new Vector3(150, 150, 560);

        ShrinkFlashlight();
        SetRecognizerMode();

        if (tabletUI)
            tabletUI.SetTabletActive(debug);
    }

    private void CreateDuplicatesForMiniMap()
    {
        List<XRGestureInteractable> originalInteractables = FindObjectsOfType<XRGestureInteractable>().ToList();
        foreach (var interactable in originalInteractables)
        {
            GameObject original = interactable.gameObject;
            GameObject temp = Instantiate(original);
            Destroy(temp.GetComponent<XRGestureInteractable>());

            if (temp.CompareTag("star"))
            {
                temp.transform.localScale = new Vector3(20, 20, 20);
            }
            if (temp.CompareTag("pyramid"))
            {
                temp.transform.localScale = new Vector3(5, 5, 5);
            }
            if (temp.CompareTag("infinity"))
            {
                temp.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            }
            if (temp.CompareTag("cube") || temp.CompareTag("sphere") || temp.CompareTag("cylinder"))
            {
                temp.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }

            // Destroy only the collider not intended for selection
            foreach (var c in temp.GetComponents<Collider>())
            {
                if (!c.isTrigger)
                    Destroy(c);
            }

            Destroy(temp.GetComponent<Object_collected>());

            temp.AddComponent<shapeItem_2>();
            temp.GetComponent<shapeItem_2>().original = interactable.gameObject;
            temp.GetComponent<Rigidbody>().isKinematic = true;
            temp.GetComponent<Rigidbody>().useGravity = false;


            originalToDuplicate.Add(original, temp);
            duplicate_and_originalPosition.Add(temp, original.transform);

            temp.SetActive(false);

            temp.name += "subject208";
        }
    }

    /*
    public void _DuplicationMethod1()
    {
        // duplicates and remove interactable
        // plus adding the shapeItem component

        // Method 1 --------------------
        // uses a parent and works by accessing children and so on
        // linear time
        // space efficient

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
            if (temp.tag == "infinity")
            {
                temp.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            }
            if (temp.tag == "cube" || temp.tag == "sphere" || temp.tag == "cylinder")
            {
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
    */

    // -----------------------------------------------------------------------------------------------------------

    private void Update()
    {
        ProcessInput();
        CalculateDuplicateDirections(allHighlightedObjects);
    }

    private void ProcessInput()
    {
        // make changes to allow the presend and abscense of the circular mini map in a
        // controlled way
        if (flaslightActionReference.action.WasPressedThisFrame())
        {
            ExtendFlashlight();
            miniMap.ShowMiniMap();
        }

        if (flaslightActionReference.action.IsPressed())
        {
            UpdateObjectScale();

            // fixed the first on trigger fail with this 
            miniMap.ShowMiniMap();
        }

        if (flaslightActionReference.action.WasReleasedThisFrame())
        {
            ShrinkFlashlight();
            miniMap.CloseMiniMap();
            ReleaseSelectedObject();
        }
    }

    private void SetRecognizerMode()
    {
        Gestures.Recognizer recognizer = FindObjectOfType<Gestures.Recognizer>();
        recognizer.useAsDirection = useGestureDirection;
    }


    // ----------------------------Grab behaviour ----------------------

    /*public GameObject giveOriginalFromDuplicate(GameObject temp)
    {
        if (interactableTags.Contains(temp.name))
        {
            GameObject value = temp;

            GameObject original = originalToDuplicate.FirstOrDefault(searchTool => searchTool.Value == value).Key;

            return original;
        }

        return null;
    }*/


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

    public void CalculateDuplicateDirections(List<GameObject> objects)
    {
        duplicateDirections.Clear();
        Vector3 max = new Vector3(-1, -1, -1);
        foreach (GameObject o in objects)
        {


            // -----------------------Get the Distance here and then store it in the dictionary--------------------------
            GameObject tobeInserted_Duplicate = originalToDuplicate[o];

            // work with one single object at a time as they are added
            if (tobeInserted_Duplicate != null && o != null)
            {
                // highlighted object in flashlight's corrdinate system
                var objectPositionInFlashlightCoords = transform.InverseTransformPoint(o.transform.position);
                objectPositionInFlashlightCoords.z = 0f;

                if (normalizeOffsets == true)
                    objectPositionInFlashlightCoords.Normalize();

                // add a temp helper
                temp = objectPositionInFlashlightCoords;
            }

            max = Vector3.Max(max, temp);

            duplicateDirections.Add((tobeInserted_Duplicate, temp));
        }

        // ----------------------------------
       if (normalizeOffsets == false)
        {

            for(int i = 0; i < duplicateDirections.Count; i++)
            {
                Vector3 temp = duplicateDirections[i].Item2 / Vector3.Magnitude(max);
                duplicateDirections[i] = (duplicateDirections[i].Item1, temp);
            }

            
    

        }

        // fancy way ;)
        // yet idt it will work as its still a looping process read only hmmm
        //  duplicateDirections.AsParallel().ForAll(entry => (entry.Value /= max));
    }

    public static List<(GameObject, Vector3)> GetDuplicatesAndDirections()
    {
        return duplicateDirections;
    }

    public static Dictionary<GameObject, Transform> GetDuplicatesAndOriginalPositions()
    {
        return duplicate_and_originalPosition;
    }

    #region CALLABLE BY INTERACTABLES

    public void AddtoHighlighted(GameObject o)
    {
        allHighlightedObjects.Add(o);
    }

    public void RemoveFromHighlighted(GameObject o)
    {
        allHighlightedObjects.Remove(o);
    }

    #endregion CALLABLE BY INTERACTABLES

    #region DEBUG

    private void dprint(string msg)
    {
        if (debug) print(msg);
    }

    #endregion DEBUG
}