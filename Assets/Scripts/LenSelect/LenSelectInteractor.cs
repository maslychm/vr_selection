using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// LenSelect Implementation
/// </summary>

public class LenSelectInteractor : MonoBehaviour
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
    private static List<GameObject> ObjectsToBeSetBackToOriginalSize;
    private static List<GameObject> addtoHighlightedV2;

    private GameObject selectedObject;

    [Header("Flashlight Scaling")]
    [SerializeField] private Vector3 defaultFlashlightScale;

    [Header("Dynamic Scaling")]
    [SerializeField] private bool scaleWithDistance;

    public Vector3 shoulderOffset;

    public Vector3 maxFlashlightScale;

    [Header("Other")]
    public bool debug = false;

    [Header("MiniMap Selector variables")]
    public MiniMap miniMap;


    [Header("LenSelect Selector variables")]
    public LenSelect LenSelect;


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

        // added this line just for a matter to add a component to stop the objects 
        // from interacting with each other 
        //LenSelect_CollisionStopper_Adder();

        CreateDuplicatesForMiniMap();

        allHighlightedObjects = new List<GameObject>();
        addtoHighlightedV2 = new List<GameObject>();
        ObjectsToBeSetBackToOriginalSize = new List<GameObject>();

        if (flashlightHighlighter == null)
            flashlightHighlighter = GameObject.Find("FlashlightCone");

        defaultFlashlightScale = new Vector3(150, 150, 560);

        ShrinkFlashlight();
        SetRecognizerMode();

        if (tabletUI)
            tabletUI.SetTabletActive(debug);
    }
    private void setInteractablesToAppropriateLayer()
    {

            List<Interactable> parentinteractables = FindObjectsOfType<Interactable>().ToList();
            
        // iterate and set layer
    }

    private void CreateDuplicatesForMiniMap()
    {
        List<Interactable> originalInteractables = FindObjectsOfType<Interactable>().ToList();
        foreach (var interactable in originalInteractables)
        {
            GameObject original = interactable.gameObject;
            GameObject temp = Instantiate(original);
            Destroy(temp.GetComponent<Interactable>());

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

   /* void LenSelect_CollisionStopper_Adder()
    {
        List<XRGestureInteractable> originalInteractables = FindObjectsOfType<XRGestureInteractable>().ToList();
        foreach (var interactable in originalInteractables)
        {
            interactable.gameObject.AddComponent<CollisionStopper>();
        }
    }*/
    private void Update()
    {
        ProcessInput();
        CalculateDuplicateDirections(allHighlightedObjects);
    }


    private void ProcessInput()
    {
       // if (flaslightActionReference.action.WasPressedThisFrame())
        {
            ExtendFlashlight();
            
        }

      /*  if (flaslightActionReference.action.IsPressed())
        {
            UpdateFlashlightScale();
        }

        if (flaslightActionReference.action.WasReleasedThisFrame())
        {
            ShrinkFlashlight();

        }*/
    }

    private void SetRecognizerMode()
    {
        Gestures.Recognizer recognizer = FindObjectOfType<Gestures.Recognizer>();
        recognizer.useAsDirection = useGestureDirection;
    }

    private void ExtendFlashlight()
    {
        flashlightHighlighter.transform.localScale = defaultFlashlightScale;
        //flashlightCenterCone.SetActive(true);
    }

    private void ShrinkFlashlight()
    {
        flashlightHighlighter.transform.localScale = Vector3.zero;
        //flashlightCenterCone.SetActive(false);

        // Clear hovered list
        allHighlightedObjects.Clear();
    }

    private void UpdateFlashlightScale()
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

        if (normalizeOffsets == false)
        {
            for (int i = 0; i < duplicateDirections.Count; i++)
            {
                Vector3 temp = duplicateDirections[i].Item2 / Vector3.Magnitude(max);
                duplicateDirections[i] = (duplicateDirections[i].Item1, temp);

            }
        }
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
    System.Collections.IEnumerator CoroutineHelper()
    {
        //Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits for 2 second.
        yield return new WaitForSeconds(10);

       // Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }

    System.Collections.IEnumerator CoroutineHelper2()
    {
       // Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits for 2 second.
        yield return new WaitForSeconds(3);

       // Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }
    public void AddtoHighlighted(GameObject o)
    {
        allHighlightedObjects.Add(o);
        addtoHighlightedV2.Add(o);


        StartCoroutine(CoroutineHelper());
        // increase their size here 
        LenSelect.automaticResizerInCone_LinearScaling();

      
        if (ObjectsToBeSetBackToOriginalSize.Contains(o))
            ObjectsToBeSetBackToOriginalSize.Remove(o);
    }

    public void RemoveFromHighlighted(GameObject o)
    {
        ObjectsToBeSetBackToOriginalSize.Add(o);
        allHighlightedObjects.Remove(o);

        // call the resize function in reverse 
        // add a yield returnb 

        StartCoroutine(CoroutineHelper2());

        LenSelect.OriginalPositionResetter();
    }

    public static List<GameObject> getAllHighlighted()
    {

        return addtoHighlightedV2;
    }
    public static List<GameObject> getAllObjectsExitedTheCone()
    {
        return ObjectsToBeSetBackToOriginalSize;
    }

    #endregion CALLABLE BY INTERACTABLES

    #region DEBUG

    private void dprint(string msg)
    {
        if (debug) print(msg);
    }

    #endregion DEBUG
}