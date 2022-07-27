using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClutterHandler_circumferenceDisplay : MonoBehaviour
{
    [SerializeField] public float radius = 0.015f;
    [SerializeField] public float offset = 0f;
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

        if (await == true)
            return;

        // get all the colliders with the hand itself and store them in array
        // ame as the radius of the rioght hand's sphere
        //_collidersWithHand = Physics.OverlapSphere(TheHand.transform.position, 0.03f);

        ////if(await == false)
        ////    makeSpotsReady();
        //// we only fill the spots if there is clutter
        //if (_collidersWithHand.Length <= 1)
        //    return;

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

        //// get all the colliders with the hand itself and store them in array
        //// ame as the radius of the rioght hand's sphere
        //_collidersWithHand = Physics.OverlapSphere(TheHand.transform.position, 0.03f);

        ////if(await == false)
        ////    makeSpotsReady();
        //// we only fill the spots if there is clutter
        //if (_collidersWithHand.Length <= 1)
        //    return;

        // print all the colliders currently saved
        //foreach (var currentlyTouching in _collidersWithHand)
        //{
        //    if (currentlyTouching.gameObject.name.Contains("phere"))
        //        Debug.Log("colliding with -> " + currentlyTouching);
        //}

        // check if the user clicked the trigger
        if (clickedRightHandController.action.WasPressedThisFrame())
        {
            //isDuplicatedThisFrame = true;
            await = true;
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
        print("WE REACHED A re outcasting process 11111 ///////''''");
        foreach (GameObject original in originaltoduplicatewithgameObject.Keys)
        {
            print("WE REACHED A re outcasting process ///////''''");
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
            float vertical_placement = Mathf.Cos(angle);
            float horizental_placement = Mathf.Sin(angle);

            // combine both placement into a vector3
            Vector3 newPosition = new Vector3(horizental_placement, vertical_placement, 0);

            // now let's get the final position around the circle for the current iteration
            var extendedPosition = newPosition * (radius + offset) + centreCircleTransform.localPosition;

            // let's create the placeHolder/empty gameObject now in the environment through instantiation
            //var tempPlaceHolder = Instantiate(new GameObject(), extendedPosition, Quaternion.identity) as GameObject;

            // checkpoint 1
            var tempPlaceHolder = Instantiate(new GameObject(), extendedPosition, Quaternion.identity) as GameObject;
            tempPlaceHolder.transform.SetParent(MiniMap.transform);
            tempPlaceHolder.transform.localPosition = extendedPosition;

            // then let's add this new empty gameObject to out queue
            spotsAroundMiniMap.Add(tempPlaceHolder, false);
        }
    }

    // this function should take care of all the duplication process and fill the zones :)
    // In this process we assume that we are only dealing with spheres
    /* private void duplicateCurrentColliders()
     {
         List<XRGestureInteractable> originalInteractables = FindObjectsOfType<XRGestureInteractable>().ToList();
         // isTrigger?????
         foreach (var currentCollider in _collidersWithHand)
         {
             // check for anythin g that is nbot a sphere
             if (!currentCollider.gameObject.name.Contains("phere") )
             {
                 continue;
             }

             // store the game Object
             GameObject currentCollidingObjWithHand = currentCollider.gameObject;

             if (duplicatedBefore.Count != 0 && duplicatedBefore.Contains(currentCollidingObjWithHand))
                 continue;
             // duplicate this colliding gameObject
             GameObject duplicate = Instantiate(currentCollidingObjWithHand);

             // when we duplicate the object it will already have the outline component in it
             // no need for extra component addition

             // keep the same scale as the one shown in the Mini Map
             duplicate.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);

             // the object that is in the minimap will have these already deleted
             // HOWEVER
             // add sanity checks
             if (duplicate.GetComponent<XRGestureInteractable>() != null)
                 Destroy(duplicate.GetComponent<XRGestureInteractable>());
             if (duplicate.GetComponent<Object_collected>() != null)
                 Destroy(duplicate.GetComponent<Object_collected>());

             // ADD shapeItem_3, Mykola asked for this, need to ask why again?????
             // this technically only tackles a highlight display
             duplicate.AddComponent<shapeItem_3>();
             duplicate.GetComponent<shapeItem_3>().original = currentCollider.gameObject;

             // no need to add a collider already present in original

             // set kinematic and remove gravity
             duplicate.GetComponent<Rigidbody>().isKinematic = true;
             duplicate.GetComponent<Rigidbody>().useGravity = false;

             duplicate.tag = "unclutterDuplicate";

             // now simply add this duplicate to a list for access
             collidingWithHandDuplicates.Add(currentCollidingObjWithHand, duplicate);

             duplicatedBefore.Add(currentCollidingObjWithHand);

             // set to false here, means when spawning it we need to set it to active again
             duplicate.SetActive(false);
         }
     }*/

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
        // first if we do not have any items to insert
        // if we do not have any spots to fill
        // we simply return

        //if (_collidersWithHand.Length < 2)
        //{
        //    return;
        //}

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

            print("MID INSERTION ****");
            // initiate the insertion process
            // update the position the rotation and the parent

            if (temp[i].gameObject.GetComponent<shapeItem_2>() == null)
                continue;
            if (originalToDuplicate[temp[i].gameObject.GetComponent<shapeItem_2>()] == null)
                print("Problem 2 triggered");
            print("HERE IS THE ORIGINAL TO DUPLICATE INFO ->>>>" + temp[i].gameObject.GetComponent<shapeItem_2>().gameObject.name + " " + originalToDuplicate[temp[i].gameObject.GetComponent<shapeItem_2>()].gameObject.name);
            originalToDuplicate[temp[i].gameObject.GetComponent<shapeItem_2>()].gameObject.transform.position = _NextAvailableSpot.transform.position;

            originalToDuplicate[temp[i].gameObject.GetComponent<shapeItem_2>()].gameObject.transform.rotation = _NextAvailableSpot.transform.rotation;

            print("ARE WE HERE ???????????????????");

            // this should fix the need to rotate and position needs to be going along with the minimap/hand
            originalToDuplicate[temp[i].gameObject.GetComponent<shapeItem_2>()].gameObject.transform.SetParent(_NextAvailableSpot.transform);

            collidingWithHandDuplicates.Add(temp[i].gameObject.GetComponent<shapeItem_2>().gameObject, originalToDuplicate[temp[i].gameObject.GetComponent<shapeItem_2>()].gameObject.GetComponent<shapeItem_3>().gameObject);

            print("COMPLETED INSERTION ****");

            spotsAroundMiniMap[availableindex] = true;

            /*  if(originaltoduplicatewithgameObject.ContainsKey(_collidersWithHand[i].gameObject) == false)
              {
                  print("PROBLEM 2");
                  continue;
              }

              originaltoduplicatewithgameObject[_collidersWithHand[i].gameObject].transform.position = _NextAvailableSpot.transform.position;
              originaltoduplicatewithgameObject[_collidersWithHand[i].gameObject].transform.rotation = _NextAvailableSpot.transform.rotation;

              print("ARE WE HERE ???????????????????");

              // this should fix the need to rotate and position needs to be going along with the minimap/hand
              originaltoduplicatewithgameObject[_collidersWithHand[i].gameObject].transform.SetParent(_NextAvailableSpot.transform);

              collidingWithHandDuplicates.Add(_collidersWithHand[i].gameObject, originaltoduplicatewithgameObject[_collidersWithHand[i].gameObject]);

              print("COMPLETED INSERTION ****");

              spotsAroundMiniMap[availableindex] = true;*/
        }
    }
}