using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    // Dimensions of the grid
    // private int width = 3;
    // private int height = 3;
    // private float GridSpaceSize = 0.45f;
    [SerializeField] private int height;
    [SerializeField] private int width;
    [SerializeField] private float GridSpaceSize;

    // So see in inspector
    [SerializeField] private GameObject gridCellPrefab;


    // Grid array out of game objects, 2d array
    private GameObject[,] gameGrid;

    // Start is called before the first frame update
    void Start()
    {
        CreateGrid();
    }

    // Creates the grid when the game starts
    private void CreateGrid()
    {
        gameGrid = new GameObject[height, width];

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
                gameGrid[x,y] = Instantiate(gridCellPrefab, new Vector3(x * GridSpaceSize, y * GridSpaceSize), Quaternion.identity);

                gameGrid[x,y].GetComponent<GridCell>().SetPosition(x,y); // Sets position

                gameGrid[x,y].transform.parent = transform; // Everything it spawns will be a child under the grid
                gameGrid[x,y].gameObject.name = "Grid Space (X: " + x.ToString() + " , Y: " + y.ToString() + ")"; // Show x & y position to help with debugging
            }
        }
    }

    // Gets the grid positioin from world position

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
