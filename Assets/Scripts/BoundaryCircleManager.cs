using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// need to set this to switch from red to green whem the user hovers over it
/// this file should be put in the right hand controller
/// </summary>
public class BoundaryCircleManager : MonoBehaviour
{
    // this needs to store the circle itself
    [SerializeField] private GameObject circleOfTrialConfirmation;

    //private Renderer circleRenderer;

    public static bool wasHoveredOver = false;
    //private HideViewOfSpheresController hideViewRectangleHelper;

    // this should store the controller of the right hand
    //[SerializeField] private GameObject transformReference;

    // add an action reference to simulate the click
    // this should be the select button of the righ thand
    [SerializeField] public InputActionReference clickedCircleForStartOfTrial;

    // Start is called before the first frame update
    private void Start()
    {
        //hideViewRectangleHelper = FindObjectOfType<HideViewOfSpheresController>();
        //circleRenderer = circleOfTrialConfirmation.GetComponent<Renderer>();

        wasHoveredOver = false;

        // initiually will kepp the color to be red
        circleOfTrialConfirmation.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
    }

    // Update is called once per frame
    private void Update()
    {
        // set this to not be used during the trial
        //if (wasHoveredOver == true)
            //return;

        //if (wasHoveredOver == false)
            //ResetParameters();

        if (clickedCircleForStartOfTrial.action.WasPressedThisFrame())
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
            {
                // CHECK if the hit is actually the circle
                if (hit.collider.gameObject.name == "CircleBoundaryForUser")
                {
                    circleOfTrialConfirmation.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                    wasHoveredOver = true;
                    //hideViewRectangleHelper.HideTheBarrier();
                }
            }
        }
    }

    public void ResetParameters()
    {
        wasHoveredOver = false;
        circleOfTrialConfirmation.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
    }
}