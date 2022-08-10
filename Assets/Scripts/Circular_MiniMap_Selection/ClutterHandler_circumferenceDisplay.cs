using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClutterHandler_circumferenceDisplay : MonoBehaviour
{
    [SerializeField] public float radius = 0.015f;
    [SerializeField] public float offset;
    private int totalObjectsCount = 8;
    private Transform centreCircleTransform;
    private static Dictionary<GameObject, bool> spotsAroundMiniMap;

    // serialize these fields and assign their components manually
    // get the centre of the mini map
    // get the whole mini map component
    // get the right hand itself assigned AS GAMEOBJECT
    [SerializeField] public GameObject centreCircle;

    [SerializeField] public GameObject MiniMap;
    [SerializeField] public GameObject TheHand;

    [SerializeField] private InputActionReference clickedRightHandController;
    public static Dictionary<shapeItem_2, shapeItem_3> originalToDuplicate;

    public bool isFrozen = false;
    public static bool runCircumference = false;

    // this list will store all the currently colliding with hand objects DUPLICATES
    public static Dictionary<GameObject, GameObject> collidingWithHandDuplicates;

    public static MiniMapInteractor miniMapInteractor;

    private void Start()
    {
        spotsAroundMiniMap = new Dictionary<GameObject, bool>();
        collidingWithHandDuplicates = new Dictionary<GameObject, GameObject>();
        originalToDuplicate = new Dictionary<shapeItem_2, shapeItem_3>();

        // get the centre of the MiniMap Position
        centreCircleTransform = centreCircle.transform;

        // take care of setting our positions before we can use them
        MakeSpotsReady();

        isFrozen = false;
    }

    // neeedx to set a getter to access the list/queue
    public static Dictionary<GameObject, bool> getSpotsAvailable()
    {
        return spotsAroundMiniMap;
    }

    // Update is called once per frame
    private void Update()
    {
        if (
            runCircumference ||
            (SelectionTechniqueDistributer.currentlySetActiveTechnique != null
            && SelectionTechniqueDistributer.currentlySetActiveTechnique.name == "MiniMapWithoutExpansion"))
        {
            return;
        }

        originalToDuplicate = miniMapInteractor.getUpdatedListOfDuplicates();
        //originaltoduplicatewithgameObject = miniMapInteractor.getUpdatedListOfDuplicates2();

        if (isFrozen == true)
        {
            if (clickedRightHandController.action.WasPressedThisFrame())
            {
                isFrozen = false;
                removeDuplicates();
                TheHand.GetComponent<GrabbingHand>().collidingWithHand.Clear();
                return;
            }
            else
                return;
        }

        // just as a safety check we will clear the lists here again
        if (collidingWithHandDuplicates == null)
        {
            print("I AM NULL");
        }
        if (collidingWithHandDuplicates.Count > 0)
        {
            collidingWithHandDuplicates.Clear();
        }
        //removeDuplicates();
        foreach (var key in spotsAroundMiniMap.Keys.ToList())
        {
            spotsAroundMiniMap[key] = false;
        }

        // check if the user clicked the trigger
        if (clickedRightHandController.action.WasPressedThisFrame() && GrabbingHand.isHovering == true)
        {
            isFrozen = true;
            return;
        }
        // if nothing then simply free the spots and clear the list of duplicates to be stored
        // we can clear as techniaclly we won't have that many to instantiate again later
        else
        {
            collidingWithHandDuplicates.Clear();

            isFrozen = false;
        }
    }

    public void removeDuplicates()
    {
        Vector3 originalOutCastPosition = new Vector3(50, 50, 50);

        foreach (shapeItem_2 original in originalToDuplicate.Keys)
        {
            originalToDuplicate[original].transform.position = originalOutCastPosition;
            originalToDuplicate[original].transform.parent = null;
        }
        foreach (var key in spotsAroundMiniMap.Keys.ToList())
        {
            spotsAroundMiniMap[key] = false;
        }
    }

    /// <summary>
    /// set the spots positions around the circle's circumference and acoomodate the presence of 8 placements
    /// </summary>
    private void MakeSpotsReady()
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
            var tempPlaceHolder = Instantiate(new GameObject(), extendedPosition, Quaternion.identity) as GameObject;
            tempPlaceHolder.transform.SetParent(MiniMap.transform);
            tempPlaceHolder.transform.localPosition = extendedPosition;

            // then let's add this new empty gameObject to out queue
            spotsAroundMiniMap.Add(tempPlaceHolder, false);
        }
    }

    public void insertToSpots(HashSet<shapeItem_2> toInsert)
    {
        //print(" TOINSERT COUNT " + toInsert.Count + " ORIGINAL TO DUP -> " + originalToDuplicate.Count);

        List<GameObject> temp = new List<GameObject>();

        foreach (var obj in toInsert)
        {
            temp.Add(obj.gameObject);
        }
        if (temp.Count <= 1)
        {
            temp.Clear();
            return;
        }
        GameObject availableindex = null;

        for (int i = 0; (i < temp.Count); i++)
        {
            var shapeItem2 = temp[i].GetComponent<shapeItem_2>();
            if (shapeItem2 == null)
                continue;

            // get the next available spot
            foreach (GameObject j in spotsAroundMiniMap.Keys)
            {
                if (spotsAroundMiniMap[j] == false)
                {
                    availableindex = j;
                    spotsAroundMiniMap[j] = true;
                    break;
                }
            }

            GameObject _NextAvailableSpot = availableindex;

            originalToDuplicate[shapeItem2].transform.position = _NextAvailableSpot.transform.position;
            originalToDuplicate[shapeItem2].transform.rotation = _NextAvailableSpot.transform.rotation;

            // this should fix the need to rotate and position needs to be going along with the minimap/hand
            originalToDuplicate[shapeItem2].transform.SetParent(_NextAvailableSpot.transform);

            if (collidingWithHandDuplicates.ContainsKey(shapeItem2.gameObject))
            {
                // skip if already present
                continue;
            }

            collidingWithHandDuplicates.Add(shapeItem2.gameObject, originalToDuplicate[shapeItem2].GetComponent<shapeItem_3>().gameObject);

            spotsAroundMiniMap[availableindex] = true;
        }
    }
}