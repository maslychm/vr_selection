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

    [SerializeField] private Vector3 debugPlaneOffset;
    [SerializeField] private Transform hmdTransform;

    [Header("Debugging and UI")]
    [SerializeField] private TabletUI tabletUI;

    private Dictionary<string, List<GameObject>> highlightedObjectsByType;
    private List<GameObject> allHighlightedObjects;

    private bool isHighlighting = false;
    private GameObject selectedObject;
    private Vector3 defaultFlashlightScale;

    private bool debug = false;

    public void Start()
    {
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

        SelectionEvents.FilterSelection.AddListener(SelectObjectOfType);
        SelectionEvents.DirectionSelection.AddListener(SelectObjectInDirection);
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
        }

        if (flaslightActionReference.action.WasReleasedThisFrame())
        {
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
    }

    private void ShrinkFlashlight()
    {
        isHighlighting = false;
        flashlightHighlighter.transform.localScale = new Vector3(0, 0, 0);

        // Clear hovered list
        allHighlightedObjects.Clear();
        foreach (var kv in highlightedObjectsByType)
        {
            kv.Value.Clear();
        }
    }

    private void SelectObjectOfType(string objectType)
    {
        if (!isHighlighting || highlightedObjectsByType[objectType].Count == 0)
        {
            dprint($"Not highlighting any {objectType}");
            return;
        }

        dprint($"FILTERING FOR {objectType}");

        selectedObject = highlightedObjectsByType[objectType][0];

        ShrinkFlashlight();

        dprint($"selected {selectedObject.name}");

        PickupObject(selectedObject);
    }

    private void SelectObjectInDirection(RecognitionResult r)
    {
        if (!isHighlighting || allHighlightedObjects.Count == 0)
            return;

        Vector3 unprojectedDirection = (r.endPt - r.startPt).normalized;
        Vector3 gestureCenterPoint = (r.startPt + r.endPt) / 2f;
        Vector3 planeNormal = (gestureCenterPoint - hmdTransform.position).normalized;
        Vector3 projectedDirection = Vector3.ProjectOnPlane(unprojectedDirection, planeNormal).normalized;

        selectedObject = FindObjectWithMostProjectionOverlap(projectedDirection);
        ShrinkFlashlight();
        PickupObject(selectedObject);
        dprint($"selected {selectedObject.name}");
    }

    private GameObject FindObjectWithMostProjectionOverlap(Vector3 direction)
    {
        // pre-process direction to only leave X and Y data
        direction.z = 0f;
        direction.Normalize();

        // For each object in the highlighted objects list, compute dot product between it's XY direction
        // vector in the flashlight's coordinate system with the drawn direction
        (GameObject obj, float score) bestObject = (null, -2f);

        foreach (var o in allHighlightedObjects)
        {
            // highlighted object in flashlight's corrdinate system
            var objectPositionInFlashlightCoords = transform.InverseTransformPoint(o.transform.position);
            objectPositionInFlashlightCoords.z = 0f;
            objectPositionInFlashlightCoords.Normalize();

            // Dot product to measure alignment between passed direction and object's projected direction
            var dot = Vector3.Dot(direction, objectPositionInFlashlightCoords);
            if (dot > bestObject.score)
            {
                bestObject.obj = o;
                bestObject.score = dot;
            }

            // debug
            //tabletUI.WriteLine($"{o.tag} dir: {objectPositionInFlashlightCoords}, target: {direction}, dot: {dot}");
            //directionDifferences.Add((o, dot));
            //tabletUI.WriteLine($"{o.tag}, {dot}");
            //dprint($"{o.tag}, {dot}");
        }

        //tabletUI.WriteLine($"best: {bestObject.obj.tag}, {bestObject.score}");

        return bestObject.obj;
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