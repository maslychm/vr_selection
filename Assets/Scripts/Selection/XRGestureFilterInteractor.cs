using System;
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

    [Header("Debugging and UI")]
    [SerializeField] private TabletUI tabletUI;

    private Dictionary<string, List<GameObject>> highlightedObjectsByType;
    private List<GameObject> allHighlightedObjects;

    private bool isHighlighting = false;
    private GameObject selectedObject;
    private Vector3 defaultFlashlightScale;

    private GameObject otherHandDebug;

    private bool debug = true;

    public void Start()
    {
        otherHandDebug = GameObject.Find("RightHand Controller");

        // Pre-populate for O(1) type access
        highlightedObjectsByType = new Dictionary<string, List<GameObject>>();
        allHighlightedObjects = new List<GameObject>();
        foreach (var s in SelectionConstants.objTypeNames)
        {
            highlightedObjectsByType.Add(s, new List<GameObject>());
        }

        SelectionEvents.FilterSelection.AddListener(SelectObjectOfType);
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
        allHighlightedObjects.Clear();
        foreach (var kv in highlightedObjectsByType)
        {
            kv.Value.Clear();
        }
    }

    private void PickupObjectInDirection(RecognitionResult r)
    {
        if (!isHighlighting || allHighlightedObjects.Count == 0)
            return;

        /**
        * TODO DISABLING "ROTATE WITH LOOK" FOR GESTURAL INPUT MAKES THIS NOT WORK
        * (^^ turns out plane project does not flip the sign based on normal direction) 
        * TODO UNPROJECTED WORKS BETTER THAN PROJECTED FOR SOME REASON. SINCE ALREADY ROTATED WITH LOOK?
        * TODO NEXT: CALCULATE PER-OBJECT DIRECTIONS AND COMPARE THE ONE FROM HERE TO THE BEST ONE THERE
        * I came back the next day, changed only that instead of first or last point, the center pt
        * is used to pick the normal, and now it works really well with simply "rotate with look" enabled
        */

        Vector3 unprojectedDirection = (r.endPt - r.startPt).normalized;
        Vector3 gestureCenterPoint = (r.startPt + r.endPt) / 2f;
        Vector3 planeNormal = (gestureCenterPoint - hmdTransform.position).normalized;
        Vector3 projectedDirection = Vector3.ProjectOnPlane(unprojectedDirection, planeNormal).normalized;

        //dprint($"projecting {unprojectedDirection} onto {planeNormal}");
        //dprint($"DIR: {projectedDirection.normalized}");
        //tabletUI.UpdateText($"gesture DIR: {projectedDirection}");

        selectedObject = FindObjectWithMostProjectionOverlap(projectedDirection);

        ShrinkFlashlight();

        dprint($"selected {selectedObject.name}");

        PickupObject(selectedObject);
    }

    private GameObject FindObjectWithMostProjectionOverlap(Vector3 direction)
    {
        // pre-process direction to only leave X and Y data
        direction.z = 0f;
        direction.Normalize();

        // For each object in the highlighted objects list, compute their projections onto flashlight forward plane
        // and compute the difference between target direction and their directions to find the most likely one
        (GameObject obj, float score) bestObject = (null, -2f);

        foreach (var o in allHighlightedObjects)
        {
            // highlighted object in flashlight's corrdinate system
            var objectPositionInFlashlightCoords = transform.InverseTransformPoint(o.transform.position);
            objectPositionInFlashlightCoords.z = 0f;
            objectPositionInFlashlightCoords.Normalize();

            // flashlight's position in it's own coords (for computing direction)
            var flashlightPositionInFlashlightCoords = transform.localPosition;

            // final direction towards the object in flashlight's coordinate system
            var uprojectedDirection = objectPositionInFlashlightCoords - flashlightPositionInFlashlightCoords;
            uprojectedDirection.z = 0f;
            uprojectedDirection.Normalize();

            // Normal to project the direction onto (flashlight's forward)
            var planeNormal = transform.forward;

            // object's direction projected onto flashlight's plane defined by normal
            var projectedDirection = Vector3.ProjectOnPlane(uprojectedDirection, planeNormal);
            projectedDirection.z = 0f;
            projectedDirection.Normalize();

            // Dot product to measure alignment between passed direction and object's projected direction
            var dot = Vector3.Dot(direction, projectedDirection);
            if (dot > bestObject.score)
            {
                bestObject.obj = o;
                bestObject.score = dot;
            }
            //tabletUI.WriteLine($"{o.tag} dir: {projectedDirection}, target: {direction}, dot: {dot}");
            //directionDifferences.Add((o, dot));
            //tabletUI.WriteLine($"{o.tag}, {dot}");
            //dprint($"{o.tag}, {dot}");
        }

        //tabletUI.WriteLine($"best: {bestObject.obj.tag}, {bestObject.score}");

        return bestObject.obj;
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