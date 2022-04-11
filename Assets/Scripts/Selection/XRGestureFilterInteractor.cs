using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CapsuleCollider))]
public class XRGestureFilterInteractor : MonoBehaviour
{
    [SerializeField] private InputActionReference flaslightActionReference;
    [SerializeField] private Transform attachTransform;

    private Dictionary<string, List<GameObject>> highlightedObjects;
    private CapsuleCollider extendableCollider;
    private bool isHighlighting = false;
    private GameObject selectedObject;

    private bool debug = true;

    public void Start()
    {
        // Pre-populate for O(1) type access
        highlightedObjects = new Dictionary<string, List<GameObject>>();
        foreach (var s in SelectionConstants.objTypeNames)
        {
            highlightedObjects.Add(s, new List<GameObject>());
        }

        SelectionEvents.FilterSelection.AddListener(PickupObjectOfType);

        extendableCollider = GetComponent<CapsuleCollider>();
        ShrinkCollider();
    }

    private void Update()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        if (flaslightActionReference.action.WasPressedThisFrame())
        {
            ExtendCollider();
        }

        if (flaslightActionReference.action.WasReleasedThisFrame())
        {
            ShrinkCollider();
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

    private void ExtendCollider()
    {
        isHighlighting = true;
        extendableCollider.height = 4.9f;
        extendableCollider.radius = 0.65f;
    }

    private void ShrinkCollider()
    {
        isHighlighting = false;
        extendableCollider.height = 0.0f;
        extendableCollider.radius = 0.0f;

        // Clear hovered list
        foreach (var kv in highlightedObjects)
        {
            kv.Value.Clear();
        }
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

        ShrinkCollider();

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