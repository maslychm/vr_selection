using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PassInteractablesToGrid : MonoBehaviour
{
    [SerializeField] private InputActionReference flowerConeActionRef;
    [SerializeField] private InputActionReference sameAsSelectRef;

    [SerializeField] private GameObject cone;
    [SerializeField] private ConeVolumeHighlighter coneVolumeHighlighter;
    [SerializeField] private GameObject rayGameObject;

    [SerializeField] private GameGrid grid;
    [SerializeField] private Transform rayStartPoint;

    [SerializeField] private GrabbingHand grabbingHand;

    public enum FlowerConeMode
    {
        Highlighting, SelectingWithRay, None
    }

    [ReadOnly] private FlowerConeMode mode;

    private void OnEnable()
    {
        TransitionToHighlighting();
    }

    public void AtTrialStart()
    {
        TransitionToHighlighting();
    }

    /// <summary>
    /// Send a ray and hit a copy of an interactable in a grid. Return true if something was hit, else false;
    /// </summary>
    /// <returns></returns>
    private bool SelectWithRay()
    {
        if (Physics.Raycast(rayStartPoint.position, rayStartPoint.forward, out RaycastHit hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("GridInteractable"))
            {
                Interactable og = hit.collider.transform.parent.GetComponent<GridCell>().originalInteractable;
                grabbingHand.CallPickUpObject(og);
                grid.DestroyGrid();

                return true;
            }
        }

        return false;
    }

    private void EndInteractablesHover(List<Interactable> interactables)
    {
        interactables.ForEach(x => x.EndHover());
    }

    /// <summary>
    /// Return true when grid was initialized, else return false.
    /// Initialize grid if more than 1 object is highlighted.
    /// </summary>
    /// <returns></returns>
    private uint CallGridInitialize()
    {
        List<Interactable> allHighlightedObjects = coneVolumeHighlighter.GetAllInteractables();
        EndInteractablesHover(allHighlightedObjects);

        if (allHighlightedObjects.Count == 0)
            return 0;

        if (allHighlightedObjects.Count == 1)
        {
            grabbingHand.CallPickUpObject(allHighlightedObjects[0]);

            return 1;
        }

        //print($"Num interactables in total: {allHighlightedObjects.Count}");

        //print("Destroying the previous grid");
        grid.DestroyGrid();

        //print($"Passing {allHighlightedObjects.Count} interactables");
        grid.CreateGrid(allHighlightedObjects);

        return 2;
    }

    private void ProcessInput()
    {
        // If dropped object -> go back to highlighting
        if (sameAsSelectRef.action.WasPressedThisFrame())
        // && mode != FlowerConeMode.high)
        {
            grid.DestroyGrid();
            TransitionToHighlighting();
            return;
        }

        // If did not press Trigger
        if (!flowerConeActionRef.action.WasPressedThisFrame())
        {
            return;
        }

        switch (mode)
        {
            case FlowerConeMode.None:
                break;

            case FlowerConeMode.Highlighting:
                uint gridInitCode = CallGridInitialize();

                if (gridInitCode == 0)
                { }
                else if (gridInitCode == 1)
                    TransitionToNone();
                else
                    TransitionToSelecting();

                break;

            case FlowerConeMode.SelectingWithRay:
                if (SelectWithRay())
                    TransitionToNone();
                else
                { }
                break;
        }
    }

    private void TransitionToHighlighting()
    {
        cone.SetActive(true);
        rayGameObject.SetActive(false);
        mode = FlowerConeMode.Highlighting;
    }

    private void TransitionToSelecting()
    {
        cone.SetActive(false);
        rayGameObject.SetActive(true);
        mode = FlowerConeMode.SelectingWithRay;
    }

    private void TransitionToNone()
    {
        cone.SetActive(false);
        rayGameObject.SetActive(false);
        mode = FlowerConeMode.None;

        // added for debugging purpose
        TransitionToHighlighting();
    }

    private void Update()
    {
        ProcessInput();
    }
}