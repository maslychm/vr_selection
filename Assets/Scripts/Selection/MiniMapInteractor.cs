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

    [SerializeField] private Transform transformForProjection;

    [SerializeField] private Transform attachTransform;

    [Header("Gesture Direction technique-related")]
    [SerializeField] private GameObject handDebugPlane;

    [SerializeField] private GameObject flashlightCenterCone;

    [SerializeField] private Vector3 debugPlaneOffset;

    [SerializeField] private Transform hmdTransform;

    [Header("Debugging and UI")]
    [SerializeField] private TabletUI tabletUI;

    private HashSet<Interactable> allHighlightedObjects;

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

    /// <summary>
    /// Flat repr settings: norm=false, farthest=true, ignoreDepth=true
    /// </summary>

    [SerializeField] private bool normalizeOffsets = false;
    [SerializeField] private bool scaleByFarthest = true;
    [SerializeField] private bool ignoreDepth = true;

    [Tooltip("Multiplication factor before applying radius inside minimap")]
    [SerializeField] private Vector3 scaleFactor = Vector3.one;

    private Dictionary<Interactable, shapeItem_2> originalToDuplicate;
    public Dictionary<shapeItem_2, shapeItem_3> originalToDuplicate_ForCirCumference;
    private List<(shapeItem_2, Vector3)> duplicateDirections;

    public void OnEnable()
    {
        //print($"AWAKE IN MMINTERACTOR IS CALLED in {name}");
        originalToDuplicate = new Dictionary<Interactable, shapeItem_2>();
        duplicateDirections = new List<(shapeItem_2, Vector3)>();

        allHighlightedObjects = new HashSet<Interactable>();
        originalToDuplicate_ForCirCumference = new Dictionary<shapeItem_2, shapeItem_3>();

        if (defaultFlashlightScale == Vector3.zero)
            defaultFlashlightScale = new Vector3(150, 150, 560);

        if (transformForProjection == null)
            transformForProjection = transform;

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
            DestroyImmediate(g.gameObject);
        }

        foreach (var g in FindObjectsOfType<shapeItem_3>())
        {
            DestroyImmediate(g.gameObject);
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

            if (interactable.TryGetComponent<TargetInteractable>(out var ti))
            {
                if (interactable.GetComponents<cakeslice.Outline>().ToList().Count < 2)
                {
                    // If it's a target, the already set outline is the target outline, save it
                    ti.targetOutline = original.GetComponent<cakeslice.Outline>();

                    ti.targetOutline.enabled = true;

                    // Also add the highlighting outline
                    interactable.interactionOutline = original.AddComponent<cakeslice.Outline>();
                    interactable.interactionOutline.enabled = false;
                }
            }
            else
            {
                // If it's a regular object, add outline if it does not have one
                if (original.TryGetComponent<cakeslice.Outline>(out var outl))
                {
                    interactable.interactionOutline = outl;
                    interactable.interactionOutline.enabled = false;
                }
                else
                {
                    interactable.interactionOutline = original.AddComponent<cakeslice.Outline>();
                    interactable.interactionOutline.enabled = false;
                }
            }

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

            DestroyImmediate(duplicate.GetComponent<Interactable>());
            DestroyImmediate(duplicate.GetComponent<Object_collected>());

            shapeItem_2 si2 = duplicate.AddComponent<shapeItem_2>();
            si2.original = interactable;
            foreach (var outl in si2.GetComponents<cakeslice.Outline>())
            {
                if (outl.color == 0)
                {
                    si2.interactionOutline = outl;
                }
                else
                {
                    si2.targetOutline = outl;
                }
            }
            duplicate.GetComponent<Rigidbody>().isKinematic = true;
            duplicate.GetComponent<Rigidbody>().useGravity = false;

            // --------------------------------For Circumference of the mini map-----------------

            GameObject duplicateOFduplicate = Instantiate(duplicate);
            DestroyImmediate(duplicateOFduplicate.GetComponent<shapeItem_2>());
            DestroyImmediate(duplicateOFduplicate.GetComponent<Interactable>());
            DestroyImmediate(duplicateOFduplicate.GetComponent<Object_collected>());

            shapeItem_3 si3 = duplicateOFduplicate.AddComponent<shapeItem_3>();
            si3.original = interactable;
            si3.shapeItem2_parent = si2;
            foreach (var outl in si3.GetComponents<cakeslice.Outline>())
            {
                if (outl.color == 0)
                {
                    si3.interactionOutline = outl;
                }
                else
                {
                    si3.targetOutline = outl;
                }
            }
            duplicateOFduplicate.GetComponent<Rigidbody>().isKinematic = true;
            duplicateOFduplicate.GetComponent<Rigidbody>().useGravity = false;
            duplicateOFduplicate.tag = "unclutterDuplicate";

            originalToDuplicate_ForCirCumference.Add(si2, si3);
            duplicateOFduplicate.transform.position = new Vector3(50, 50, 50);

            //---------------------------------------------------------------------------------

            originalToDuplicate.Add(interactable, si2);

            duplicate.SetActive(false);

            si2.gameObject.layer = 10;
            si3.gameObject.layer = 10;
        }
    }

    private void Update()
    {
        ProcessInput();
        if (allHighlightedObjects.Count > 0)
        {
            CalculateDuplicateDirections(allHighlightedObjects);
        }

        // print("***" + allHighlightedObjects.Count());
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
        transform.localScale = newScale;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Interactable interactable))
            return;

        interactable.StartHover();
        allHighlightedObjects.Add(interactable);
    }

    public void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out Interactable interactable))
            return;
        interactable.EndHover();

        allHighlightedObjects.Remove(interactable);
    }

    public void CalculateDuplicateDirections(HashSet<Interactable> interactables)
    {
        duplicateDirections.Clear();
        Vector3 max = Vector3.zero;
        float minZ = Mathf.Infinity;
        float maxZ = -1;
        foreach (Interactable interactable in interactables)
        {
            // -----------------------Get the Distance here and then store it in the dictionary--------------------------

            if (!originalToDuplicate.ContainsKey(interactable))
            {
                continue;
            }

            shapeItem_2 duplicate = originalToDuplicate[interactable];
            if (duplicate == null)
                continue;

            // highlighted object in flashlight's corrdinate system
            var objectPositionInFlashlightCoords = transformForProjection.InverseTransformPoint(interactable.transform.position);

            if (ignoreDepth)
                objectPositionInFlashlightCoords.z = 0f;

            if (normalizeOffsets)
                objectPositionInFlashlightCoords.Normalize();

            if (objectPositionInFlashlightCoords.magnitude > max.magnitude)
                max = objectPositionInFlashlightCoords;

            if (objectPositionInFlashlightCoords.z < minZ)
                minZ = objectPositionInFlashlightCoords.z;

            if (objectPositionInFlashlightCoords.z > maxZ)
                maxZ = objectPositionInFlashlightCoords.z;

            duplicateDirections.Add((duplicate, objectPositionInFlashlightCoords));
        }

        for (int i = 0; i < duplicateDirections.Count; i++)
        {
            //if (duplicateDirections.Count == 1) break; // <- Yes?
            // TODO need to find a fix here! When there is only 1 element, normalization happens. When more, they sometimes go outside the boundaries

            Vector3 temp = duplicateDirections[i].Item2;

            temp.z -= (maxZ + minZ) / 2f;

            if (scaleByFarthest)
                temp /= max.magnitude;
            else
                temp.Scale(scaleFactor);

            duplicateDirections[i] = (duplicateDirections[i].Item1, temp);

            // TODO try dividing by current flashlight scale times some factor
            // ^^^ Yes, this is correct, because as of right now, if there's only 1 element highlighted, the above code acts as normalization. Added length check.
        }
    }

    public List<(shapeItem_2, Vector3)> GetDuplicatesAndDirections()
    {
        //print($"sizeof duplicates dirs {duplicateDirections.Count}");
        return duplicateDirections;
    }
}