using cakeslice;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class RayManager : MonoBehaviour
{
    [SerializeField] private InputActionReference selectActionButton;
    Vector3 fwd;
    LineRenderer LenSelectRay;

    public CircleConfirmationSpawner circle;

    private GameObject helper56;

    RaycastHit hit;
    private List<XRGestureInteractable> allObjectsInteractables;
    public static bool selectWasClicked = false;

    Vector3 fromPosition;
    Vector3 toPosition;

    public GameObject leftHandController;
    GameObject Destination;

    GameObject helperT;

    bool wasSelectedBefore = false;
    bool debug = false;

    [SerializeField] private InputActionReference confirmSelectionButton;

    GameObject Prior;

    public OutlineEffect temp5;
    public static GameObject currentGameObjectHighlighted;
    // Start is called before the first frame update
    void Start()
    {


        allObjectsInteractables = new List<XRGestureInteractable>();

        allObjectsInteractables = FindObjectsOfType<XRGestureInteractable>().ToList();

        selectWasClicked = false;
        //Ray  ray = new Ray(leftHandController.transform.position, transform.forward);

        LenSelectRay = this.gameObject.GetComponent<LineRenderer>();

        helper56 = null;
    }

    private void Update()
    {


        Detecthis();

    }
    IEnumerator waiter(GameObject other)
    {
        print("we reached the coroutine roger that }}}}}}}}}}}}}}}}??????????????????");
        //Wait for 2 seconds
        yield return new WaitForSeconds(0.5f);
        other.GetComponent<Outline>().eraseRenderer = true;
        other.SetActive(false);
    }

    private void Detecthis()
    {
        
        string[] temp = { "LenSelectInteractables", "confirmationCircle" };
        int layerMask = LayerMask.GetMask(temp);
        if (Physics.Raycast(leftHandController.transform.position, transform.forward, out hit, Mathf.Infinity, layerMask))
        {

            // add an if

            //if (helper56 != null && hit.transform.gameObject != helper56 && helper56.name != "FlashLightCone" && helper56.name != "ConfirmationSphere")
            //   helper56.gameObject.GetComponent<Outline>().eraseRenderer = true;

            print("ok we reached this after raycasting ->>>>>>>" + hit.transform.gameObject.name);            
            GameObject other = hit.transform.gameObject;
            if (other.name == "FlashLightCone")
                return;

            if (wasSelectedBefore == false)
            {
                if (other.GetComponent<Outline>() == null)
                     other.AddComponent<Outline>();

            }
            currentGameObjectHighlighted = other;

            if (debug == true && wasSelectedBefore == true)
            {
                if (other != Prior)
                    return;

                wasSelectedBefore = false;
                debug = false;
                
                if (other == Prior)
                    StartCoroutine(waiter(Prior));
   
                return;

            }

            if (confirmSelectionButton.action.WasPressedThisFrame() && wasSelectedBefore == true && Prior != null && Prior.name != "FlashLightCone")
            {
                
                circle.ConfirmSelection();

                
                debug = true;
            }

            //LenSelectRay.transform.position = new Vector3(LenSelectRay.transform.position.x, LenSelectRay.transform.position.y, other.transform.position.z);
            else if (selectActionButton.action.WasPressedThisFrame() && wasSelectedBefore == false)
            {
                selectWasClicked = true;
                wasSelectedBefore = true;
                circle.Inraycastmodification(other);
                Prior = other;
                return;
            }
            else 
            helper56 = other;

        }
        NothittingAnymore();
        selectWasClicked = false;
    }
    private void NothittingAnymore()
    {

        if (selectActionButton.action.WasReleasedThisFrame())
            selectWasClicked = false;
    }

    public static bool getStatusOfGrip()
    {
        return selectWasClicked;
    }
}
