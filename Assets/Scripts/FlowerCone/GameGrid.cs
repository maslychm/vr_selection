using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    // Dimensions of the grid
    // private int width = 3;
    // private int height = 3;
    // private float GridSpaceSize = 0.45f;
    private int height;
    private int width;
    [SerializeField] private float GridSpaceSize;

    // So see in inspector
    [SerializeField] private GameObject gridCellPrefab;

    // Placeholder for testing
    [SerializeField] private GameObject placeholderObject;

    // Grid array out of game objects, 2d array
    private GameObject[,] gameGrid;

    int countingObjects;

    // The Grid itself
    [SerializeField] private GameObject theGrid;

    public void start()
    {
        // Trying to get grid to spawn further from user but it isn't working
        theGrid.transform.position = new Vector3(0, 0, 6);
    }

    // Creates the grid when the game starts
    // CreateGrid(List<Interactable> interactables)
    public void CreateGrid(List<Interactable> interactables, int numOfInteractables, Material[] interactableMaterial)
    {

        // Trying to get the grid to be further back (fix later)
        theGrid.transform.position = new Vector3(0, 0, 6);



        // Calculates the sides
        height = (int)Mathf.Ceil(Mathf.Sqrt(numOfInteractables));
        width = (int)Mathf.Ceil(Mathf.Sqrt(numOfInteractables));

        // de-bugging
        print("NUM"+ numOfInteractables);
        print("height: " + height + "width: " + width);

        gameGrid = new GameObject[height, width];

        // Makes sure correct number of interactables appear
        countingObjects = 0;

        // Checks to make sure that grid cell is not empty
        // If it's empty nothing is going to work
        if (gridCellPrefab == null)
        {
            Debug.LogError("ERROR: Grid Cell Prefab on the grid is not assigned");
            return;
        }


        // Make the grid
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Create a new GridSpace object for each cell
                gameGrid[x, y] = Instantiate(gridCellPrefab, new Vector3(x * GridSpaceSize, y * GridSpaceSize), Quaternion.identity);
                gameGrid[x, y].transform.localScale *= .04f;

                gameGrid[x, y].GetComponent<GridCell>().SetPosition(x, y); // Sets position

                // Sets material & active objects
                if (countingObjects < numOfInteractables)
                {
                    gameGrid[x, y].GetComponent<GridCell>().SetMaterial(interactableMaterial[countingObjects]); 
                }
                countingObjects++;

                gameGrid[x, y].transform.parent = transform; // Everything it spawns will be a child under the grid
                gameGrid[x, y].gameObject.name = "Grid Space (X: " + x.ToString() + " , Y: " + y.ToString() + ")"; // Show x & y position to help with debugging
            }
        }

        
    }

    public void DestroyGrid(int height, int width)
    {
        // Will be called when user selected an item or exits the grid
        // For now it is called in PassInteractablesToGrid when space is pressed
        foreach (Transform child in transform) 
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    /*public void CreateGridForInteractables(List<Interactable> interactables, int numOfInteractables)
    {
        // Note: grid will be created each time user clicks.
        // Need to implement a way to destroy it in real time.

        // Calculate the sides: math_ceiling(math_sqrt(interactables.Count))
        height = (int)Mathf.Ceil(Mathf.Sqrt(numOfInteractables));
        width = (int)Mathf.Ceil(Mathf.Sqrt(numOfInteractables));

        // Create the grid -> placeholders (maybe empty objects) for interactables

        // Fill in the grid with interactables -> go through the grid and set position and rotation
        // (so that they face the user)
        // Set appropriate scaling (will require playing around with params)
    }
    */

    // Gets the grid position from world position
    public Vector2Int GetGridPosFromWorld(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / GridSpaceSize);
        int y = Mathf.FloorToInt(worldPosition.z / GridSpaceSize);

        x = Mathf.Clamp(x, 0, width);
        y = Mathf.Clamp(x, 0, height);

        return new Vector2Int(x, y);
    }

    // Gets the world position of grid position
    public Vector3 GetWorldPositionFromGridPos(Vector2Int gridPos)
    {
        float x = gridPos.x * GridSpaceSize;
        float y = gridPos.y * GridSpaceSize;

        return new Vector3(x, 0, y);
    }
}