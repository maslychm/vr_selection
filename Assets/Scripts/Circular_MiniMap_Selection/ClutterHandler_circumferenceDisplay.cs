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
    private bool isDuplicatedThisFrame = false;

    // serialize these fields and assign their components manually
    // get the centre of the mini map
    // get the whole mini map component
    // get the right hand itself assigned AS GAMEOBJECT
    [SerializeField] public GameObject centreCircle;

    [SerializeField] public GameObject MiniMap;
    [SerializeField] public GameObject TheHand;

    [SerializeField] private InputActionReference clickedRightHandController;
    private Dictionary<shapeItem_2, shapeItem_3> originalToDuplicate;

    bool before = false;

    private List<GameObject> duplicatedBefore;

    public bool await = false;

    private Collider[] _collidersWithHand;

    // this list will store all the currently colliding with hand objects DUPLICATES
    public static Dictionary<GameObject, GameObject> collidingWithHandDuplicates;

    public static Dictionary<GameObject, GameObject> originaltoduplicatewithgameObject;

    private void Start()
    {
        spotsAroundMiniMap = new Dictionary<GameObject, bool>();
        duplicatedBefore = new List<GameObject>();
        originaltoduplicatewithgameObject = new Dictionary<GameObject, GameObject>();
        collidingWithHandDuplicates = new Dictionary<GameObject, GameObject>();
        originalToDuplicate = new Dictionary<shapeItem_2, shapeItem_3>();
        //allInstantiables = FindObjectOfType<XRGestureInteractable>().ToList();

        // get the centre of the MiniMap Position
        centreCircleTransform = centreCircle.transform;

        // take care of setting our positions before we can use them
        makeSpotsReady();

        await = false;
    }

    // neeedx to set a getter to access the list/queue
    public static Dictionary<GameObject, bool> getSpotsAvailable()
    {
        return spotsAroundMiniMap;
    }

    // Update is called once per frame
    private void Update()
    {
        originalToDuplicate = MiniMapInteractor.getUpdatedListOfDuplicates();
        originaltoduplicatewithgameObject = MiniMapInteractor.getUpdatedListOfDuplicates2();
        //if (clickedRightHandController.action.WasPressedThisFrame() && before == true)
        //{
        //    await = false;
        //    before = false;
        //    removeDuplicates();

        //}

        if (await == true)
        {
            if (clickedRightHandController.action.WasPressedThisFrame())
            {
                //isDuplicatedThisFrame = true;
                await = false;
                return;
            }
            else 
                return;
        }

        // just as a safety check we will clear the lists here again
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
        if (clickedRightHandController.action.WasPressedThisFrame())
        {
            //isDuplicatedThisFrame = true;
            await = true;
            //before = true;
            return;
        }
        // if nothing then simply free the spots and clear the list of duplicates to be stored
        // we can clear as techniaclly we won't have that many to instantiate again later
        else
        {
            collidingWithHandDuplicates.Clear();

            await = false;
        }
    }

    //maybe can be used later
    private void DestroySpots()
    {
        foreach (var temp in spotsAroundMiniMap.Keys)
        {
            GameObject spot = temp;
            Destroy(spot);
        }

        spotsAroundMiniMap.Clear();
    }

    public void removeDuplicates()
    {
        Vector3 originalOutCastPosition = new Vector3(50, 50, 50);

        foreach (GameObject original in originaltoduplicatewithgameObject.Keys)
        {

            originaltoduplicatewithgameObject[original].transform.position = originalOutCastPosition;
            originaltoduplicatewithgameObject[original].transform.parent = null;

        }
        foreach (var key in spotsAroundMiniMap.Keys.ToList())
        {
            spotsAroundMiniMap[key] = false;
        }
    }

    /// <summary>
    /// set the spots positions around the circle's circumference and acoomodate the presence of 8 placements
    /// </summary>
    private void makeSpotsReady()
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

    /// <summary>
    /// this will only insert when the hand actually hits the items on the mini map
    /// </summary>
    public void helper(Collider[] collidersHere)
    {
        _collidersWithHand = collidersHere;
        insertToSpots();
    }

    private void insertToSpots()
    {

        List<GameObject> temp = new List<GameObject>();

        for (int i = 0; i < _collidersWithHand.Length; i++)
        {
            if (_collidersWithHand[i].gameObject.GetComponent<shapeItem_2>())
                temp.Add(_collidersWithHand[i].gameObject);
        }
        if (temp.Count <= 1)
        {
            temp.Clear();
            return;
        }
        GameObject availableindex = null;

        for (int i = 0; (i < temp.Count); i++)
        {
            if (temp[i].gameObject.GetComponent<shapeItem_2>() == null)
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

            print("Started INSERTION ****");
            GameObject _NextAvailableSpot = availableindex;


            if (temp[i].gameObject.GetComponent<shapeItem_2>() == null)
                continue;

            if (originalToDuplicate[temp[i].gameObject.GetComponent<shapeItem_2>()] == null)
                print("Problem 2 triggered");

            print("HERE IS THE ORIGINAL TO DUPLICATE INFO ->>>>" + temp[i].gameObject.GetComponent<shapeItem_2>().gameObject.name + " " + originalToDuplicate[temp[i].gameObject.GetComponent<shapeItem_2>()].gameObject.name);
            originalToDuplicate[temp[i].gameObject.GetComponent<shapeItem_2>()].gameObject.transform.position = _NextAvailableSpot.transform.position;

            originalToDuplicate[temp[i].gameObject.GetComponent<shapeItem_2>()].gameObject.transform.rotation = _NextAvailableSpot.transform.rotation;

            // this should fix the need to rotate and position needs to be going along with the minimap/hand
            originalToDuplicate[temp[i].gameObject.GetComponent<shapeItem_2>()].gameObject.transform.SetParent(_NextAvailableSpot.transform);

            collidingWithHandDuplicates.Add(temp[i].gameObject.GetComponent<shapeItem_2>().gameObject, originalToDuplicate[temp[i].gameObject.GetComponent<shapeItem_2>()].gameObject.GetComponent<shapeItem_3>().gameObject);

            print("COMPLETED INSERTION ****");

            spotsAroundMiniMap[availableindex] = true;
        }
    }
}