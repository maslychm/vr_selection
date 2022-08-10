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

    // build a dictionary to map ouyt the transforms back to original 
    public Dictionary<GameObject, Transform> MapToOriginalPosisition;

    public LineRenderer lineRenderer; // stor ethe current puyrpole linerenderer 

    public Material whiteMaterial, RedMaterial;

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

        lineRenderer = this.GetComponent<LineRenderer>();

        lineRenderer.material = whiteMaterial;
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
        MapToOriginalPosisition.Clear();
        HoldRayCastHitCollider.Clear();
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

        GameObject priorPlacedInHand;
        foreach (GameObject tempGameObjectFromSet in HoldRayCastHitCollider)
        {
            tempGameObjectFromSet.transform.SetParent(leftHandController.transform);
            tempGameObjectFromSet.transform.localScale = new Vector3(0.06f, 0.06f, 0.06f);
            tempGameObjectFromSet.transform.position = transform.TransformPoint(new Vector3(leftHandController.transform.localPosition.x, leftHandController.transform.localPosition.y, leftHandController.transform.localPosition.z + startOffsetOFspheres));
            priorPlacedInHand = tempGameObjectFromSet;
            startOffsetOFspheres += 0.065f;
        }

        lineRenderer.material = RedMaterial;
        startOffsetOFspheres = 0.05f;
        BringOrFlush = 1; 
    }

    // thios functiuobn should set back the objects to their original transform 
    public void releaseObjectsBackToOriginalPosition()
    {
        
        foreach(var temp in HoldRayCastHitCollider)
        {
            temp.transform.SetParent(null);
            temp.GetComponent<Object_collected>().ResetGameObject();
        }
        lineRenderer.material = whiteMaterial;
        BringOrFlush = 0;
    }
}