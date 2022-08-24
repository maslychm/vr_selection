using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClutterHandler_circumferenceDisplay : MonoBehaviour
{
    private int totalObjectsCount = 8;
    [SerializeField] public float radius = 0.015f;
    [SerializeField] public float offset;

    [SerializeField] private Transform centreCircleTransform;
    [SerializeField] private MiniMapInteractor miniMapInteractor;
    [SerializeField] private MiniMap MiniMap;
    [SerializeField] private GrabbingHand grabbingHand;
    [SerializeField] private InputActionReference clickedRightHandController;

    private Dictionary<GameObject, shapeItem_3> slotsAroundMiniMap;
    public Dictionary<shapeItem_2, shapeItem_3> originalToDuplicate;

    public bool isFrozen = false;
    public bool runCircumference = false;

    private void Start()
    {
        slotsAroundMiniMap = new Dictionary<GameObject, shapeItem_3>();

        PrepareSlots();
        isFrozen = false;
    }

    private void OnEnable()
    {
        isFrozen = false;
    }

    private void ProcessFreezing()
    {
        if (clickedRightHandController.action.WasPressedThisFrame())
        {
            if (isFrozen)
            {
                //print("was frozen -> unfroze");
                FreeCircularSlots();
                grabbingHand.collidingWithHand.Clear();
                isFrozen = false;
            }
            else
            {
                if (grabbingHand.isHovering)
                {
                    //print("was hovering -> froze");
                    isFrozen = true;
                }
                else
                {
                    //print("was not hovering -> unfroze");
                    grabbingHand.collidingWithHand.Clear();
                    isFrozen = false;
                }
            }
        }
    }

    private void Update()
    {
        ProcessFreezing();

        originalToDuplicate = miniMapInteractor.getUpdatedListOfDuplicates();

        if (isFrozen)
            return;

        FreeCircularSlots();
        InsertToSlots(grabbingHand.collidingWithHand);
    }

    /// <summary>
    /// Free up the circular slots around the MiniMap
    /// </summary>
    public void FreeCircularSlots()
    {
        isFrozen = false;

        Vector3 originalOutCastPosition = new Vector3(50, 50, 50);

        foreach (var key in slotsAroundMiniMap.Keys.ToList())
        {
            if (slotsAroundMiniMap[key] == null)
                continue;

            slotsAroundMiniMap[key].transform.position = originalOutCastPosition;
            slotsAroundMiniMap[key].transform.parent = null;
            slotsAroundMiniMap[key] = null;
        }
    }

    /// <summary>
    /// Set the slots positions around the circle's circumference and acoomodate the presence of 8 placements
    /// </summary>
    private void PrepareSlots()
    {
        for (int i = 0; i < totalObjectsCount; i++)
        {
            // get the distance that is around the circle
            float angle = i * Mathf.PI * 2f / totalObjectsCount;

            // let's try and get the vertical placement and horizental before combining them
            float vertical_placement = Mathf.Sin(angle);
            float horizental_placement = Mathf.Cos(angle);

            // combine both placement into a vector3
            Vector3 newPosition = new Vector3(horizental_placement, vertical_placement, 0);

            // now let's get the final position around the circle for the current iteration

            // ITS 0.3F TO GET THE RIGHT CIRCUMFERENCE POSITIONING
            // MOVE BY 0.1 DEPENDING ON THE RESIZING OF THE MINIMAP
            var extendedPosition = (newPosition * Mathf.Abs(radius - 0.3f)) + centreCircleTransform.localPosition;

            // checkpoint 1
            GameObject placeHolder = Instantiate(new GameObject(), extendedPosition, Quaternion.identity);
            placeHolder.transform.SetParent(MiniMap.transform);
            placeHolder.transform.localPosition = extendedPosition;

            slotsAroundMiniMap.Add(placeHolder, null);
        }
    }

    public void InsertToSlots(HashSet<shapeItem_2> toInsert)
    {
        GameObject slotToFill = null;

        foreach (shapeItem_2 sa2 in toInsert)
        {
            // get the next available slot
            foreach (GameObject slot in slotsAroundMiniMap.Keys)
            {
                if (slotsAroundMiniMap[slot] == null)
                {
                    slotToFill = slot;
                    break;
                }
            }

            if (slotToFill == null)
            {
                print("All slots around the map are full.");
                return;
            }

            originalToDuplicate[sa2].transform.position = slotToFill.transform.position;
            originalToDuplicate[sa2].transform.rotation = slotToFill.transform.rotation;
            originalToDuplicate[sa2].transform.SetParent(slotToFill.transform);

            slotsAroundMiniMap[slotToFill] = originalToDuplicate[sa2];
        }
    }
}