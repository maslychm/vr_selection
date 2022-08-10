using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PassInteractablesToGrid : MonoBehaviour
{
    public GameGrid grid;

    private float chanceOfAdding = .5f;

    [Range(0, 100)]
    [SerializeField] private int fixedValueToUse = 0;

    private void Start()
    {
    }

    private void CallGridInitialize()
    {
        chanceOfAdding = Random.Range(0f, 1f);

        List<Interactable> interactables = FindObjectsOfType<Interactable>().ToList();
        print($"Num interactables in total: {interactables.Count}");

        List<Interactable> subsetOfInteractables = new List<Interactable>();

        if (fixedValueToUse == 0)
        {
            foreach (Interactable interactable in interactables)
            {
                if (Random.Range(0f, 1f) > chanceOfAdding)
                    subsetOfInteractables.Add(interactable);
            }
        }
        else
        {
            for (int i = 0; i < fixedValueToUse; i++)
            {
                subsetOfInteractables.Add(interactables[i]);
            }
        }

        // Same thing as above
        //for (int i = 0; i < interactables.Count; i++)
        //{
        //    if (Random.Range(0f, 1f) > chanceOfAdding)
        //        subsetOfInteractables.Add(interactables[i]);
        //}

        print("Destroying the previous grid");
        grid.DestroyGrid();

        print($"Passing {subsetOfInteractables.Count} interactables");
        grid.CreateGridForInteractables(interactables);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("Space was pressed -> CallGridInitialize()");
            CallGridInitialize();
        }
    }
}