using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

/**
 * This script must be attached to the cone that will hit the Interactables.
 * This script represents the Mini Map (circular) selector Interactor
 */

[RequireComponent(typeof(Collider))]
public class MiniMapInteractor : MonoBehaviour
{
    [SerializeField] private InputActionReference flaslightActionReference;

    [Tooltip("This object must have a collider and be tagged as GestureFilter")]
    //[SerializeField] private GameObject flashlightHighlighter;

    [SerializeField] private Transform attachTransform;

    [Header("Gesture Direction technique-related")]
    [SerializeField] private GameObject handDebugPlane;

    [SerializeField] private GameObject flashlightCenterCone;

    [SerializeField] private Vector3 debugPlaneOffset;

    [SerializeField] private Transform hmdTransform;

    [Header("Debugging and UI")]
    [SerializeField] private TabletUI tabletUI;

    private List<GameObject> allHighlightedObjects;

    [Header("Flashlight Scaling")]
    [SerializeField] private Vector3 defaultFlashlightScale = Vector3.zero;

    [Header("Dynamic Scaling.")]
    [Tooltip("Enabling this will ignore the default flashlight scale.")]
    [SerializeField] private bool scaleWithDistance;

    [Tooltip("Max size of the flashlight. Ignores the default scale.")]
    public Vector3 maxFlashlightScale;

    public Vector3 shoulderOffset;

    [Header("Other")]
    public bool debug = false;

    [Header("MiniMap Selector variables")]
    public MiniMap miniMap;

    [SerializeField] private bool normalizeOffsets = false;

    private Vector3 temp; // simple helper to store the distance later in our dictionary

    private Dictionary<Interactable, shapeItem_2> originalToDuplicate;
    public Dictionary<shapeItem_2, shapeItem_3> originalToDuplicate_ForCirCumference;
    private List<(shapeItem_2, Vector3)> duplicateDirections;

    public void OnEnable()
    {
        //print($"AWAKE IN MMINTERACTOR IS CALLED in {name}");
        originalToDuplicate = new Dictionary<Interactable, shapeItem_2>();
        duplicateDirections = new List<(shapeItem_2, Vector3)>();

        allHighlightedObjects = new List<GameObject>();
        originalToDuplicate_ForCirCumference = new Dictionary<shapeItem_2, shapeItem_3>();

        if (defaultFlashlightScale == Vector3.zero)
            defaultFlashlightScale = new Vector3(150, 150, 560);

        ShrinkFlashlight();

        if (tabletUI)
            tabletUI.SetTabletActive(debug);

        CreateDuplicatesForMiniMap();
    }

    public Dictionary<shapeItem_2, shapeItem_3> getUpdatedListOfDuplicates()
    {
        return originalToDuplicate_ForCirCumference;
    }

    public void CreateDuplicatesForMiniMap()
    {
        foreach (var g in FindObjectsOfType<shapeItem_2>())
        {
            Destroy(g.gameObject);
        }

        foreach (var g in FindObjectsOfType<shapeItem_3>())
        {
            Destroy(g.gameObject);
        }

        //print($"Calling duplication in {name}");
        duplicateDirections.Clear();
        originalToDuplicate.Clear();

        originalToDuplicate_ForCirCumference.Clear();

        List<Interactable> originalInteractables = FindObjectsOfType<Interactable>().ToList();
        //print("Count of originals ->>>> " + originalInteractables.Count + " OK ");
        foreach (var interactable in originalInteractables)
        {
            GameObject original = interactable.gameObject;
            if (!original.GetComponent<cakeslice.Outline>())
                original.AddComponent<cakeslice.Outline>().enabled = false;
            else
                original.GetComponent<cakeslice.Outline>().enabled = false;

            GameObject duplicate = Instantiate(original);

            if (duplicate.CompareTag("star"))
            {
                duplicate.transform.localScale = new Vector3(20, 20, 20);
            }
            if (duplicate.CompareTag("pyramid"))
            {
                duplicate.transform.localScale = new Vector3(5, 5, 5);
            }
            if (duplicate.CompareTag("infinity"))
            {
                duplicate.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            }
            if (duplicate.CompareTag("cube") || duplicate.CompareTag("sphere") || duplicate.CompareTag("cylinder"))
            {
                duplicate.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
            }

            // Interactable prefabs used to have 2 colliders: trigger and non-trigger
            // Flashlight also had a trigger collider
            // This was causing both of the Interactable colliders to call OnTriggerEnter, causing double addition to the hihlighted list
            // I removed the trigger collider on Interactables and the issue was resolved
            foreach (var c in duplicate.GetComponents<Collider>())
                c.isTrigger = true;

            Destroy(duplicate.GetComponent<Interactable>());
            Destroy(duplicate.GetComponent<Object_collected>());

            duplicate.AddComponent<shapeItem_2>();
            duplicate.GetComponent<shapeItem_2>().original = interactable.gameObject;
            duplicate.GetComponent<Rigidbody>().isKinematic = true;
            duplicate.GetComponent<Rigidbody>().useGravity = false;

            // --------------------------------For Circumference of the mini map-----------------

            GameObject duplicateOFduplicate = Instantiate(duplicate);
            Destroy(duplicateOFduplicate.GetComponent<shapeItem_2>());
            Destroy(duplicateOFduplicate.GetComponent<Interactable>());
            Destroy(duplicateOFduplicate.GetComponent<Object_collected>());
            duplicateOFduplicate.AddComponent<shapeItem_3>();
            duplicateOFduplicate.GetComponent<shapeItem_3>().original = original.gameObject;
            duplicateOFduplicate.GetComponent<shapeItem_3>().shapeItem2_parent = duplicate.GetComponent<shapeItem_2>();
            duplicateOFduplicate.GetComponent<Rigidbody>().isKinematic = true;
            duplicateOFduplicate.GetComponent<Rigidbody>().useGravity = false;
            duplicateOFduplicate.tag = "unclutterDuplicate";

            originalToDuplicate_ForCirCumference.Add(duplicate.GetComponent<shapeItem_2>(), duplicateOFduplicate.GetComponent<shapeItem_3>());
            // remove them away
            duplicateOFduplicate.transform.position = new Vector3(50, 50, 50);

            //---------------------------------------------------------------------------------

            originalToDuplicate.Add(interactable, duplicate.GetComponent<shapeItem_2>());

            duplicate.SetActive(false);
        }
    }

    private void Update()
    {
        ProcessInput();
        if (allHighlightedObjects.Count > 0)
        {
            CalculateDuplicateDirections(allHighlightedObjects);
        }
    }

    private void ProcessInput()
    {
        if (Input.GetKeyDown("space"))
        {
            ExtendFlashlight();
            miniMap.ShowMiniMap();
        }

        if (flaslightActionReference.action.WasPressedThisFrame())
        {
            ExtendFlashlight();
            miniMap.ShowMiniMap();
        }

        if (flaslightActionReference.action.IsPressed())
        {
            UpdateFlashlightScale();
        }

        if (flaslightActionReference.action.WasReleasedThisFrame())
        {
            ShrinkFlashlight();
            //miniMap.CloseMiniMap();
            miniMap.FreezeMiniMap();
        }
    }

    private void ExtendFlashlight()
    {
        //flashlightHighlighter.transform.localScale = defaultFlashlightScale;
        transform.localScale = defaultFlashlightScale;
        flashlightCenterCone.SetActive(true);
    }

    private void ShrinkFlashlight()
    {
        //flashlightHighlighter.transform.localScale = Vector3.zero;
        transform.localScale = Vector3.zero;
        flashlightCenterCone.SetActive(false);

        // Clear hovered list
        allHighlightedObjects.Clear();
    }

    private void UpdateFlashlightScale()
    {
        if (!scaleWithDistance)
            return;

        var shoulderInWorld = hmdTransform.TransformPoint(shoulderOffset);

        float distHand = Mathf.Abs(Vector3.Distance(shoulderInWorld, transform.position));
        float factor = 1f - Mathf.Clamp(distHand * 3f / 2f, 0, .98f);

        var newScale = factor * maxFlashlightScale;
        newScale.z = 600;
        //flashlightHighlighter.transform.localScale = newScale;
        transform.localScale = newScale;
        //print(factor);
        //print($"distHand: {distHand}");
    }

    // migrated triggers from interactables
    public void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("sphere"))
            return;
        other.gameObject.GetComponent<Interactable>().dprint(other.tag);
        other.gameObject.GetComponent<Interactable>().StartHover();

        AddtoHighlighted(other.gameObject);
    }

    public void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("sphere"))
            return;
        other.gameObject.GetComponent<Interactable>().dprint(other.tag);
        other.gameObject.GetComponent<Interactable>().EndHover();

        RemoveFromHighlighted(other.gameObject);
    }

    public void CalculateDuplicateDirections(List<GameObject> objects)
    {
        //print(objects[0].name);
        duplicateDirections.Clear();
        Vector3 max = Vector3.zero;
        foreach (GameObject o in objects)
        {
            // -----------------------Get the Distance here and then store it in the dictionary--------------------------

            if (!originalToDuplicate.ContainsKey(o.GetComponent<Interactable>()))
            {
                continue;
            }

            shapeItem_2 duplicate = originalToDuplicate[o.GetComponent<Interactable>()];
            if (duplicate == null)
                continue;

            // highlighted object in flashlight's corrdinate system
            //var objectPositionInFlashlightCoords = flashlightHighlighter.transform.InverseTransformPoint(o.transform.position);
            var objectPositionInFlashlightCoords = transform.InverseTransformPoint(o.transform.position);
            objectPositionInFlashlightCoords.z = 0f;

            if (normalizeOffsets)
                objectPositionInFlashlightCoords.Normalize();

            //print($"in calculation: {objectPositionInFlashlightCoords.magnitude} - {objectPositionInFlashlightCoords}");

            if (objectPositionInFlashlightCoords.magnitude > max.magnitude)
                max = objectPositionInFlashlightCoords;

            duplicateDirections.Add((duplicate, objectPositionInFlashlightCoords));
        }

        if (normalizeOffsets == false)
        {
            for (int i = 0; i < duplicateDirections.Count; i++)
            {
                //if (duplicateDirections.Count == 1) break; // <- Yes?
                // TODO need to find a fix here! When there is only 1 element, normalization happens. When more, they sometimes go outside the boundaries

                Vector3 temp = duplicateDirections[i].Item2 / max.magnitude;
                duplicateDirections[i] = (duplicateDirections[i].Item1, temp);

                // TODO try dividing by current flashlight scale times some factor
                // ^^^ Yes, this is correct, because as of right now, if there's only 1 element highlighted, the above code acts as normalization. Added length check.
            }
        }
    }

    public List<(shapeItem_2, Vector3)> GetDuplicatesAndDirections()
    {
        //print($"sizeof duplicates dirs {duplicateDirections.Count}");
        return duplicateDirections;
    }

    #region CALLABLE BY INTERACTABLES

    public void AddtoHighlighted(GameObject o)
    {
        //print($"{o.name}");
        allHighlightedObjects.Add(o);
    }

    public void RemoveFromHighlighted(GameObject o)
    {
        allHighlightedObjects.Remove(o);
    }

    #endregion CALLABLE BY INTERACTABLES
}