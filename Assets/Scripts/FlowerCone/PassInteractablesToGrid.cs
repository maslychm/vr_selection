using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// this script needs to be implemented into the mini-map script (?)

//

public class PassInteractablesToGrid : MonoBehaviour
{
    [SerializeField] private InputActionReference controlFreezing;
    public GameGrid grid; // Add scipt to set position to camera
    public GridCell gridObjects;
    public MiniMapInteractor interactor;

    // private float chanceOfAdding = .5f;

    // [Range(0, 100)]
    // [SerializeField] private int fixedValueToUse = 0;

    // public int numOfInteractables = 0;

    public Material[] materialsOfInteractables = new Material[300];
    // public int materialCount = 0;

    public GameObject[] oringalInteractables = new GameObject[300];

    private List<Interactable> allHighlightedObjects;
    private List<GameObject> ogInteractables;

    public Transform rayStartPoint;
    [SerializeField] private InputActionReference sendRaycast;

    private void Start()
    {
        //CallGridInitialize();
    }

    private void SelectWithRay()
    {
        // Creates a Ray from this object, moving forward
        Ray ray = new Ray(rayStartPoint.position, rayStartPoint.forward);

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(rayStartPoint.position, rayStartPoint.forward, out hit, Mathf.Infinity))
        {
            print(hit.collider.name);

            if (hit.collider.CompareTag("GridInteractable"))
            {
                GameObject og = hit.collider.transform.parent.GetComponent<GridCell>().objectInThisGridSpace;
                FindObjectOfType<GrabbingHand>().PickupObject(og);
                grid.DestroyGrid();
            }
        }
        else
        {
            Debug.Log("Did not Hit");
        }
    }

    private void CallGridInitialize()
    {
        // materialCount = 0; // for adding materials to material list

        // chanceOfAdding = Random.Range(0f, 1f);

        allHighlightedObjects = interactor.getList();
        ogInteractables = interactor.getObjectsList();

        /* List<Interactable> interactables = FindObjectsOfType<Interactable>().ToList();
         List<Interactable> interactables = new List<Interactable>();

        foreach (var t in temp)
        {
            interactables.Add(t.GetComponent<Interactable>());
        }
        */

        print($"Num interactables in total: {allHighlightedObjects.Count}");

        // List<Interactable> subsetOfInteractables = new List<Interactable>();

        // Gets list of materials
        for (int i = 0; i < allHighlightedObjects.Count; i++)
        {
            Material myMaterial = allHighlightedObjects[i].GetComponent<Renderer>().material;
            materialsOfInteractables[i] = myMaterial;

            oringalInteractables[i] = ogInteractables[i];
        }

        print("Destroying the previous grid");
        // grid.DestroyGrid(subsetOfInteractables.Count, subsetOfInteractables.Count);
        grid.DestroyGrid();

        // print($"Passing {subsetOfInteractables.Count} interactables");
        print($"Passing {allHighlightedObjects.Count} interactables");

        grid.CreateGrid(allHighlightedObjects, allHighlightedObjects.Count, materialsOfInteractables, oringalInteractables);
    }

    // Currently runs space
    private void Update()
    {
        if (controlFreezing.action.WasPressedThisFrame())
        {
            CallGridInitialize();
        }

        if (sendRaycast.action.WasPerformedThisFrame())
        {
            SelectWithRay();
        }

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    print("Space was pressed -> CallGridInitialize()");
        //    // numOfInteractables = 0;
        //    CallGridInitialize();
        //}
    }
}