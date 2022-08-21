using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// this script needs to be implemented into the mini-map script (?)

// 


public class PassInteractablesToGrid : MonoBehaviour
{
    public GameGrid grid;
    public GridCell gridObjects;
    public MiniMapInteractor interactor;

    // private float chanceOfAdding = .5f;

    // [Range(0, 100)]
    // [SerializeField] private int fixedValueToUse = 0;

    // public int numOfInteractables = 0;

    public Material[] materialsOfInteractables = new Material[200];
    // public int materialCount = 0;

    private List<Interactable> allHighlightedObjects;

    private void Start()
    {
        CallGridInitialize();
    }

    

    private void CallGridInitialize()
    {
       // materialCount = 0; // for adding materials to material list

        // chanceOfAdding = Random.Range(0f, 1f);

        allHighlightedObjects = interactor.getList();

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
        }


        // Random
        /*if (fixedValueToUse == 0)
        {
            foreach (Interactable interactable in interactables)
            {
                if (Random.Range(0f, 1f) > chanceOfAdding)
                {
                    subsetOfInteractables.Add(interactable);

                    // Adding materials
                    Material myMaterial = interactable.GetComponent<Renderer>().material;
                    materialsOfInteractables[materialCount] = myMaterial;
                    materialCount++;
                }
            }
        }
        else
        {
            for (int i = 0; i < fixedValueToUse; i++)
            {
                subsetOfInteractables.Add(interactables[i]);
                
                numOfInteractables++;
            }
        }
        */

        // Same thing as above
        //for (int i = 0; i < interactables.Count; i++)
        //{
        //    if (Random.Range(0f, 1f) > chanceOfAdding)
        //        subsetOfInteractables.Add(interactables[i]);
        //}
        
        print("Destroying the previous grid");
        // grid.DestroyGrid(subsetOfInteractables.Count, subsetOfInteractables.Count);
        grid.DestroyGrid(allHighlightedObjects.Count, allHighlightedObjects.Count);

        // print($"Passing {subsetOfInteractables.Count} interactables");
        print($"Passing {allHighlightedObjects.Count} interactables");


        grid.CreateGrid(allHighlightedObjects, allHighlightedObjects.Count, materialsOfInteractables);
    }


    // Currently runs space 
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("Space was pressed -> CallGridInitialize()");
            // numOfInteractables = 0;
            CallGridInitialize();
        }
    }
    
}