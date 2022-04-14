using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class XRGestureFilterInteractor : MonoBehaviour
{
    [SerializeField] private InputActionReference flaslightActionReference;

    [Tooltip("This object must have a collider and be tagged as GestureFilter")]
    [SerializeField] private GameObject flashlightHighlighter;

    [SerializeField] private Transform attachTransform;

    [Header("Gesture Direction technique-related")]
    [SerializeField] private GameObject handDebugPlane;

    [SerializeField] private Vector3 debugPlaneOffset;
    [SerializeField] private Transform hmdTransform;

    private Dictionary<string, List<GameObject>> highlightedObjects;
    private bool isHighlighting = false;
    private GameObject selectedObject;
    private Vector3 defaultFlashlightScale;

    private GameObject otherHandDebug;

    private bool debug = true;

    public void Start()
    {
        otherHandDebug = GameObject.Find("RightHand Controller");

        // Pre-populate for O(1) type access
        highlightedObjects = new Dictionary<string, List<GameObject>>();
        foreach (var s in SelectionConstants.objTypeNames)
        {
            highlightedObjects.Add(s, new List<GameObject>());
        }

        SelectionEvents.FilterSelection.AddListener(PickupObjectOfType);
        SelectionEvents.DirectionSelection.AddListener(PickupObjectInDirection);

        if (flashlightHighlighter == null)
        {
            flashlightHighlighter = GameObject.Find("FlashlightCone");
        }

        defaultFlashlightScale = new Vector3(200, 200, 560);

        print(defaultFlashlightScale);

        ShrinkFlashlight();
    }

    private void Update()
    {
        ProcessInput();
        DrawDebugStuff();
    }

    private void DrawDebugStuff()
    {
        // Draw a vector from hmd to the controller
        //Debug.DrawRay(hmdTransform.position, transform.position - hmdTransform.position, Color.red);
        //Debug.DrawLine(otherHandDebug.transform.position, otherHandDebug.transform.position - hmdTransform.position, Color.red);
        //Debug.DrawRay()
        //print("anything???");

        handDebugPlane.transform.position = otherHandDebug.transform.position + debugPlaneOffset;
        handDebugPlane.transform.forward = otherHandDebug.transform.position - hmdTransform.position;
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
        o.transform.position = attachTransform.position;
        o.transform.rotation = attachTransform.rotation;
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
        foreach (var kv in highlightedObjects)
        {
            kv.Value.Clear();
        }
    }

    private void PickupObjectInDirection(RecognitionResult r)
    {
        if (!isHighlighting)
            return;

        //Vector3 centerPoint = (r.startPt + r.endPt) / 2f;

        // TODO DISABLING "ROTATE WITH LOOK" FOR GESTURAL INPUT MAKES THIS NOT WORK
        // (^^ turns out plane project does not flip the sign based on normal direction) 
        // TODO UNPROJECTED WORKS BETTER THAN PROJECTED FOR SOME REASON. SINCE ALREADY ROTATED WITH LOOK?
        // TODO NEXT: CALCULATE PER-OBJECT DIRECTIONS AND COMPARE THE ONE FROM HERE TO THE BEST ONE THERE

        Vector3 unprojectedDirection = (r.endPt - r.startPt).normalized;
        //Vector3 planeNormal = (otherHandDebug.transform.position - hmdTransform.position).normalized;
        Vector3 planeNormal = (r.startPt - hmdTransform.position).normalized;
        dprint($"projecting {unprojectedDirection} onto {planeNormal}");
        Vector3 projectedDirection = Vector3.ProjectOnPlane(unprojectedDirection, planeNormal);

        dprint($"DIR: {projectedDirection}");

        // DEBUG
        Vector3 flippedPlaneProjectedDirection = Vector3.ProjectOnPlane(unprojectedDirection, -planeNormal);
        dprint($"FLIPPED DIR: {flippedPlaneProjectedDirection}");
    }

    private void PickupObjectOfType(string objectType)
    {
        if (!isHighlighting)
            return;

        dprint($"FILTERING FOR {objectType}");
        if (highlightedObjects[objectType].Count == 0)
        {
            dprint($"No objects of {objectType}");
            return;
        }
        selectedObject = highlightedObjects[objectType][0];

        ShrinkFlashlight();

        dprint($"selected {selectedObject.name}");

        PickupObject(selectedObject);
    }

    #region CALLABLE BY INTERACTABLES

    public void AddtoHighlighted(GameObject o)
    {
        highlightedObjects[o.tag].Add(o);
    }

    public void RemoveFromHighlighted(GameObject o)
    {
        highlightedObjects[o.tag].Remove(o);
    }

    #endregion CALLABLE BY INTERACTABLES

    #region DEBUG

    private void dprint(string msg)
    {
        if (debug) print(msg);
    }

    #endregion DEBUG
}