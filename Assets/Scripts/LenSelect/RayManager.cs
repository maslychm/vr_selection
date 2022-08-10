using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class RayManager : MonoBehaviour
{
    private LineRenderer LenSelectRay;
    private List<Interactable> allObjectsInteractables;
    public GameObject leftHandController;
    public static HashSet<GameObject> HoldRayCastHitCollider = new HashSet<GameObject>();
    private float startOffsetOFspheres = 0.2f;

    [SerializeField] public InputActionReference BringItemsAligned;
    [SerializeField] public InputActionReference TakeItemsBack;

    // build a dictionary to map ouyt the transforms back to original 
    public static Dictionary<GameObject, Transform> MapToOriginalPosisition;

    // add a boolean to limit the the selection action to one button 
    public static int BringOrFlush = 0;

    RaycastHit[] hits;

    // Start is called before the first frame update
    private void Start()
    {

        MapToOriginalPosisition = new Dictionary<GameObject, Transform>();

        allObjectsInteractables = new List<Interactable>();

        allObjectsInteractables = FindObjectsOfType<Interactable>().ToList();

        LenSelectRay = this.gameObject.GetComponent<LineRenderer>();
    }

    private void Update()
    {
        // store the array of colliders hit by the raycast
        hits = Physics.RaycastAll(transform.position, transform.forward, 100.0F);

        // add an input action reference here
        if (BringItemsAligned.action.WasPerformedThisFrame() && BringOrFlush == 0)
            ProcessInputHere();
        else if (TakeItemsBack.action.WasPressedThisFrame() && BringOrFlush == 1)
        {
            releaseObjectsBackToOriginalPosition();

            if (HoldRayCastHitCollider.Count > 0)
                HoldRayCastHitCollider.Clear();

            BringOrFlush = 0;
        }
    }

    /// <summary>
    /// This typically just places the spheres hit by the raycast in the hand in a BBQ Shape
    /// ----[][][][][]-------------------
    /// </summary>
    private void ProcessInputHere()
    {
        // iterate throuigh the array of colliders and then basicvally just get the spheres
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            if (hit.collider.gameObject.name.Contains("phere"))
            {

                MapToOriginalPosisition.Add(hit.collider.gameObject, hit.collider.gameObject.transform);
                HoldRayCastHitCollider.Add(hit.collider.gameObject);
            }
        }
        if (HoldRayCastHitCollider.Count == 0)
            return;
        foreach (GameObject tempGameObjectFromSet in HoldRayCastHitCollider)
        {
            tempGameObjectFromSet.transform.SetParent(leftHandController.transform);
            tempGameObjectFromSet.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            tempGameObjectFromSet.transform.position = new Vector3(leftHandController.transform.position.x,
                leftHandController.transform.position.y, leftHandController.transform.position.z + startOffsetOFspheres);

            startOffsetOFspheres += 0.2f;
        }

        startOffsetOFspheres = 0.2f;
        BringOrFlush = 1; 
    }

    // thios functiuobn should set back the objects to their original transform 
    public static void releaseObjectsBackToOriginalPosition()
    {
        
        foreach(var temp in HoldRayCastHitCollider)
        {
            temp.transform.SetParent(null);
            temp.transform.position = MapToOriginalPosisition[temp].position;
            temp.transform.localScale = MapToOriginalPosisition[temp].localScale;
            temp.transform.rotation = MapToOriginalPosisition[temp].rotation;
        }

        BringOrFlush = 0;
    }
}