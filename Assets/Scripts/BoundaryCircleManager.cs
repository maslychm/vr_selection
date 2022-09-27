using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// need to set this to switch from red to green whem the user hovers over it
/// this file should be put in the right hand controller
/// </summary>
public class BoundaryCircleManager : MonoBehaviour
{
    [SerializeField] private Renderer readyForTrialCofirmationCircle;

    [SerializeField] private InputActionReference clickedCircleForStartOfTrial_Right;
    [SerializeField] private InputActionReference clickedCircleForStartOfTrial_Left;

    [SerializeField] private Transform dominantHandTransform;
    [SerializeField] private Transform nonDominantHandTransform;
    [SerializeField] private LineRenderer dominantLine;
    [SerializeField] private LineRenderer nonDominantLine;

    [SerializeField] private HideViewOfSpheresController mimir2;

    private LayerMask confirmationCircleLayer;

    private bool circleWasClicked = false;

    private void Start()
    {
        confirmationCircleLayer = LayerMask.GetMask("confirmationCircle");

        readyForTrialCofirmationCircle.material.SetColor("_Color", Color.red);
    }

    public bool UserConfirmedReadiness() => circleWasClicked;

    public void SetWaitForUserReady()
    {
        circleWasClicked = false;
        ShowTrialReadinessVisuals();
    }

    private void ShowTrialReadinessVisuals()
    {
        RayKebabManager.turnWhite = true;

        if (!SelectionTechniqueManager.isRayKebab)
        {
            nonDominantLine.enabled = true;
        }
        dominantLine.enabled = true;

        readyForTrialCofirmationCircle.material.SetColor("_Color", Color.red);
    }

    private void HideTrialReadinessVisuals()
    {
        RayKebabManager.turnWhite = false;

        dominantLine.enabled = false;
        nonDominantLine.enabled = false;

        readyForTrialCofirmationCircle.material.SetColor("_Color", Color.green);

        mimir2.HideTheBarrier();
    }

    private void Update()
    {
        if (
            clickedCircleForStartOfTrial_Right.action.IsPressed()
            && clickedCircleForStartOfTrial_Left.action.IsPressed()
            && (ExperimentManager.state == ExperimentManager.ExperimentState.RunningLevel
            || SearchExperimentManager.state == SearchExperimentManager.ExperimentState.RunningLevel)
        )
        {
            RaycastHit[] dominantHits, nonDominantHits;

            dominantHits = Physics.RaycastAll(
                dominantHandTransform.position,
                dominantHandTransform.forward,
                Mathf.Infinity,
                confirmationCircleLayer
            );
            nonDominantHits = Physics.RaycastAll(
                nonDominantHandTransform.position,
                nonDominantHandTransform.forward,
                Mathf.Infinity,
                confirmationCircleLayer
            );

            if (dominantHits.Length == 1 && nonDominantHits.Length == 1)
            {
                circleWasClicked = true;
                HideTrialReadinessVisuals();
            }
        }
    }
}