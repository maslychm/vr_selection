using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClutterHandler_circumferenceDisplay : MonoBehaviour
{

    private float radius = 0.2f;
    private float offset = 1f;
    private int totalObjectsCount = 8;
    private Transform centreCircleTransform;
    private static Queue<GameObject> spotsAroundMiniMap;
    [SerializeField] public GameObject centreCircle;
    [SerializeField] public GameObject MiniMap;
    [SerializeField] public GameObject TheHand;

    [SerializeField] private InputActionReference clickedRightHandController;

    public static bool await = false;

    private Collider[] _collidersWithHand;

    // this list will store all the currently colliding with hand objects DUPLICATES
    public static  Dictionary<GameObject, GameObject> collidingWithHandDuplicates;

    void Start()
    {
        spotsAroundMiniMap = new Queue<GameObject>();

        collidingWithHandDuplicates = new Dictionary<GameObject, GameObject>();

        //allInstantiables = FindObjectOfType<XRGestureInteractable>().ToList();

        // get the centre of the MiniMap Position 
        centreCircleTransform = centreCircle.transform;


        // take care of setting our positions before we can use them
        makeSpotsReady();

        await = false;
    }

    // neeedx to set a getter to access the list/queue
    public static Queue<GameObject> getSpotsAvailable()
    {
        return spotsAroundMiniMap;
    }

    // Update is called once per frame
    void Update()
    {

        if (await == true)
            return;

        // just as a safety check we will clear the lists here again
        collidingWithHandDuplicates.Clear();
        spotsAroundMiniMap.Clear();

        makeSpotsReady();

        // get all the colliders with the hand itself and store them in array
        // ame as the radius of the rioght hand's sphere
        _collidersWithHand = Physics.OverlapSphere(TheHand.transform.position, 0.03f);

        // we only fill the spots if there is clutter 
        if (_collidersWithHand.Length <= 1)
            return;

        // print all the colliders currently saved 
        foreach (var currentlyTouching in _collidersWithHand)
        {
            Debug.Log("colliding with -> " + currentlyTouching);
        }

        // start thye duplication process 
        duplicateCurrentColliders();


        // now start the insertion process
        insertToSpots();

        // check if the user clicked the trigger 
        if (clickedRightHandController.action.WasPressedThisFrame() && _collidersWithHand.Length > 1)
        {
            await = true;
            return;
        }
        // if nothing then simply free the spots and clear the list of duplicates to be stored 
        // we can clear as techniaclly we won't have that many to instantiate again later
        else
        {

            collidingWithHandDuplicates.Clear();
            spotsAroundMiniMap.Clear();

            //// again make spots ready 
            //makeSpotsReady();
            await = false;

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
            Vector3 newPosition = new Vector3(horizental_placement, 0, vertical_placement);

            // now let's get the final position around the circle for the current iteration
            var extendedPosition = newPosition * (radius + offset) + centreCircleTransform.position;

            // let's create the placeHolder/empty gameObject now in the environment through instantiation
            var tempPlaceHolder = Instantiate(new GameObject(), extendedPosition, Quaternion.identity) as GameObject;

            // now need to set the rotation and parent of this instantiated gameObject to be the minimnap to rotate with it and even look at the centre
            tempPlaceHolder.transform.SetParent(MiniMap.transform);

            // then let's add this new empty gameObject to out queue
            spotsAroundMiniMap.Enqueue(tempPlaceHolder);

        }
    }

    // this function should take care of all the duplication process and fill the zones :)
    // In this process we assume that we are only dealing with spheres
    private void duplicateCurrentColliders()
    {

        foreach (var currentCollider in _collidersWithHand)
        {

            // store the game Object 
            GameObject currentCollidingObjWithHand = currentCollider.gameObject;

            // duplicate this colliding gameObject 
            GameObject duplicate = Instantiate(currentCollidingObjWithHand);

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

            // set to false here, means when spawning it we need to set it to active again 
            duplicate.SetActive(false);
        }

    }

    private void insertToSpots()
    {

        // first if we do not have any items to insert 
        // if we do not have any spots to fill 
        // we simply return

        if (spotsAroundMiniMap.Count == 0 || _collidersWithHand.Length <= 1)
        {
            return;
        }

        for(int i = 0; (i < collidingWithHandDuplicates.Count) && (i < _collidersWithHand.Length); i++)
        {
            // get the next available spot 
            GameObject _NextAvailableSpot = spotsAroundMiniMap.Dequeue();

            // initiate the insertion process 
            // update the position the rotation and the parent 
            collidingWithHandDuplicates[_collidersWithHand[i].gameObject].transform.position = _NextAvailableSpot.transform.position;
            collidingWithHandDuplicates[_collidersWithHand[i].gameObject].transform.rotation = _NextAvailableSpot.transform.rotation;

            // this should fix the need to rotate and position needs to be going along with the minimap/hand
            collidingWithHandDuplicates[_collidersWithHand[i].gameObject].transform.SetParent(_NextAvailableSpot.transform);
            
        }
    }
}
