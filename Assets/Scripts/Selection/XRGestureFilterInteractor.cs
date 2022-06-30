using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/**
 * This script must be attached to the hand which will be holding the flashlight
 */

public class XRGestureFilterInteractor : MonoBehaviour
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

    private Dictionary<string, List<GameObject>> highlightedObjectsByType;
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

   
    public MINIMAPInitial temp;
    public void Start()
    {

        // temp = new MINIMAPInitial();

        highlightedObjectsByType = new Dictionary<string, List<GameObject>>();
        allHighlightedObjects = new List<GameObject>();
        // Pre-populate for O(1) type access
        foreach (var s in SelectionConstants.objTypeNames)
            highlightedObjectsByType.Add(s, new List<GameObject>());

        if (flashlightHighlighter == null)
            flashlightHighlighter = GameObject.Find("FlashlightCone");

        defaultFlashlightScale = new Vector3(150, 150, 560);

        ShrinkFlashlight();
        SetRecognizerMode();

        if (tabletUI)
            tabletUI.SetTabletActive(debug);
    }

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
        foreach (var kv in highlightedObjectsByType)
        {
            kv.Value.Clear();
        }
    }

    private void UpdateObjectScale()
    {
        if (!scaleWithDistance || selectedObject)
            return;

        var shoulderInWorld = hmdTransform.TransformPoint(shoulderOffset);
        //print($"head: {hmdTransform.position} shoulder: {shoulderInWorld}");

        float distHand = Vector3.Distance(shoulderInWorld, transform.position);
        //print($"distHand={distHand}");

        // Mykola: this distance ranges from .2 to .7 for me so adjusting the values below
        // to bring (.2, .7) range into (0,1) range
        distHand -= .2f;
        distHand = Mathf.Clamp(distHand, 0f, 1f);
        distHand *= 2;

        var newFlashlightScale = Mathf.Abs(1 - distHand) * maxFlashlightScale;
        newFlashlightScale.z = maxFlashlightScale.z;
        flashlightHighlighter.transform.localScale = newFlashlightScale;
    }

    #region CALLABLE BY INTERACTABLES

    public void AddtoHighlighted(GameObject o)
    {
        highlightedObjectsByType[o.tag].Add(o);
        allHighlightedObjects.Add(o);
    }

    public void RemoveFromHighlighted(GameObject o)
    {
        highlightedObjectsByType[o.tag].Remove(o);
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