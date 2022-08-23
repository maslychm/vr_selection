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
    [SerializeField] private InputActionReference clickedCircleForStartOfTrial_Right;

    [SerializeField] private InputActionReference clickedCircleForStartOfTrial_Left;
    [SerializeField] private Transform LeftHandTransform;
    public HideViewOfSpheresController mimir2;
    public GameObject ray;
    public GameObject rayLeft;

    public GameObject RayKebabHolder;

    // Start is called before the first frame update
    private void Start()
    {
        //hideViewRectangleHelper = FindObjectOfType<HideViewOfSpheresController>();
        //circleRenderer = circleOfTrialConfirmation.GetComponent<Renderer>();

        wasHoveredOver = false;

        // initiually will kepp the color to be red
        circleOfTrialConfirmation.GetComponent<Renderer>().material.SetColor("_Color", Color.red);

        //lineRenderer.gameObject.SetActive(true);
    }

    // Update is called once per frame
    private void Update()
    {
        // set this to not be used during the trial
        //if (wasHoveredOver == true)
        //return;

        if (wasHoveredOver == false)
            ResetParameters();

        if (SelectionTechniqueManager.isRayKebab == true)
        {
            rayLeft.SetActive(false);
        }

        if (clickedCircleForStartOfTrial_Right.action.IsPressed()
            && clickedCircleForStartOfTrial_Left.action.IsPressed()
            && ExperimentManager.state != ExperimentManager.ExperimentState.BetweenLevels)
        {
            RaycastHit[] hit, hit2;
            hit = Physics.RaycastAll(transform.position, transform.forward, Mathf.Infinity);
            hit2 = Physics.RaycastAll(LeftHandTransform.position, LeftHandTransform.forward, Mathf.Infinity);
            {
                bool check1 = false, check2 = false;

                foreach (var i in hit)
                    if (i.collider.gameObject.name == "CircleBoundaryForUser")
                    {
                        check1 = true;
                        break;
                    }

                if (check1 == false)
                    return;

                foreach (var j in hit2)
                    if (j.collider.gameObject.name == "CircleBoundaryForUser")
                    {
                        check2 = true;
                        break;
                    }

                if (check2 == false)
                    return;

                circleOfTrialConfirmation.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                wasHoveredOver = true;

                mimir2.HideTheBarrier();
                //hideViewRectangleHelper.HideTheBarrier();
                ray.SetActive(false);
                rayLeft.SetActive(false);
            }
        }
    }

    public void ResetParameters()
    {
        ray.SetActive(true);
        rayLeft.SetActive(true);
        wasHoveredOver = false;
        circleOfTrialConfirmation.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
    }
}