using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class RayManager : MonoBehaviour
{
    private LineRenderer LenSelectRay;
    private List<Interactable> allObjectsInteractables;
    public GameObject leftHandController;
    public HashSet<GameObject> HoldRayCastHitCollider = new HashSet<GameObject>();
    private float startOffsetOFspheres = 0.05f;

    [SerializeField] public InputActionReference BringItemsAligned;
    [SerializeField] public InputActionReference TakeItemsBack;

    public static bool turnWhite = false;

    // build a dictionary to map ouyt the transforms back to original
    //public Dictionary<GameObject, Transform> MapToOriginalPosisition;

    public LineRenderer lineRenderer; // stor ethe current puyrpole linerenderer

    public Material whiteMaterial, RedMaterial;

    // add a boolean to limit the the selection action to one button
    public static int BringOrFlush = 0;

    private RaycastHit[] hits;

    private Transform currentTransformOfTarget;

    public GrabbingHand theHand;

    // Start is called before the first frame update
    private void Start()
    {
        //MapToOriginalPosisition = new Dictionary<GameObject, Transform>();

        allObjectsInteractables = new List<Interactable>();

        allObjectsInteractables = FindObjectsOfType<Interactable>().ToList();

        LenSelectRay = this.gameObject.GetComponent<LineRenderer>();

        lineRenderer = this.GetComponent<LineRenderer>();

        lineRenderer.material = whiteMaterial;
    }

    private void OnEnable()
    {
        theHand = FindObjectOfType<GrabbingHand>();
    }

    private void Update()
    {
        // addedthis for later debugging purposes 
        //if(lineRenderer.material == RedMaterial && HoldRayCastHitCollider.Count == 0)
        if (turnWhite == true || (lineRenderer.material == RedMaterial && HoldRayCastHitCollider.Count == 0))
        {
            lineRenderer.material = whiteMaterial;
        }

        // add an input action reference here
        if (BringItemsAligned.action.WasPerformedThisFrame() && BringOrFlush == 0)
        {
            currentTransformOfTarget = ExperimentTrial.targetInteractable.transform;
            ProcessInputHere();
        }
        else if (TakeItemsBack.action.WasPressedThisFrame() && BringOrFlush == 1)
        {
            if (HoldRayCastHitCollider.Count() < 2)
            {
                lineRenderer.material = whiteMaterial;
                BringOrFlush = 0;
                HoldRayCastHitCollider.Clear();
                return;
            }
            releaseObjectsBackToOriginalPosition();

            if (HoldRayCastHitCollider.Count > 0)
                HoldRayCastHitCollider.Clear();
            lineRenderer.material = whiteMaterial;
            BringOrFlush = 0;
        }
    }

    /// <summary>
    /// This typically just places the spheres hit by the raycast in the hand in a BBQ Shape
    /// ----[][][][][]-------------------
    /// </summary>
    private void ProcessInputHere()
    {
        // store the array of colliders hit by the raycast
        hits = Physics.RaycastAll(transform.position, transform.forward, 100.0F);
        //MapToOriginalPosisition.Clear();
        HoldRayCastHitCollider.Clear();
        // iterate throuigh the array of colliders and then basicvally just get the spheres
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            if (hit.collider.GetComponent<Interactable>())
            {
                HoldRayCastHitCollider.Add(hit.collider.gameObject);
            }
        }
        if (HoldRayCastHitCollider.Count == 0)
            return;

        GameObject priorPlacedInHand;

        if (HoldRayCastHitCollider.Count == 1)
        {
            theHand.callPickUpObject(HoldRayCastHitCollider.ElementAt(0).GetComponent<Interactable>());

            lineRenderer.material = RedMaterial;
            lineRenderer.material = whiteMaterial;
            startOffsetOFspheres = 0.05f;
            //BringOrFlush = 1;
            BringOrFlush = 0;

            return;
        }
        foreach (GameObject tempGameObjectFromSet in HoldRayCastHitCollider)
        {
            tempGameObjectFromSet.transform.SetParent(leftHandController.transform);

            // for some reason the scale of these is too small
            tempGameObjectFromSet.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
            tempGameObjectFromSet.transform.position = transform.TransformPoint(new Vector3(leftHandController.transform.localPosition.x, leftHandController.transform.localPosition.y, leftHandController.transform.localPosition.z + startOffsetOFspheres));
            priorPlacedInHand = tempGameObjectFromSet;
            startOffsetOFspheres += 0.085f;
        }

        lineRenderer.material = RedMaterial;
        startOffsetOFspheres = 0.05f;
        BringOrFlush = 1;
    }

    // thios functiuobn should set back the objects to their original transform
    public void releaseObjectsBackToOriginalPosition()
    {
        foreach (var temp in HoldRayCastHitCollider)
        {

            if (temp.name != "TargetInteractable")
            {
                temp.transform.SetParent(null);
                temp.GetComponent<Object_collected>().ResetGameObject();
            }
            else if (temp.name == "TargetInteractable" && ExperimentTrial.activeTrial != null)
            {
                Debug.Log("We have reached a reset state mid trial but it fails ");
                temp.transform.position = currentTransformOfTarget.position;
            }
            else if (temp.name == "TargetInteractable" && ExperimentTrial.activeTrial == null)
            {
                temp.transform.SetParent(null);
                temp.GetComponent<Object_collected>().ResetGameObject();
            }

            // backup generic code for resetting
            temp.transform.SetParent(null);
            temp.GetComponent<Object_collected>().ResetGameObject();
        }
        lineRenderer.material = whiteMaterial;
        BringOrFlush = 0;
    }
}