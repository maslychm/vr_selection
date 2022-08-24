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

    [SerializeField] private GameObject gridCellPrefab;

    private GameObject[,] gameGrid;

    private int objectIndex;

    private void Start()
    {
        transform.position = new Vector3(0, 0, 6);
    }

    public void CreateGrid(List<Interactable> interactables)
    {
        int numOfInteractables = interactables.Count;

        transform.position = new Vector3(0, 0, 4);

        height = (int)Mathf.Ceil(Mathf.Sqrt(numOfInteractables));
        width = (int)Mathf.Ceil(Mathf.Sqrt(numOfInteractables));

        gameGrid = new GameObject[height, width];

        objectIndex = 0;

        if (gridCellPrefab == null)
        {
            Debug.LogError("ERROR: Grid Cell Prefab on the grid is not assigned");
            return;
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (objectIndex > numOfInteractables - 1)
                    break;

                gameGrid[x, y] = Instantiate(gridCellPrefab, new Vector3(x * GridSpaceSize, y * GridSpaceSize), Quaternion.identity);
                gameGrid[x, y].transform.localScale *= .04f;

                gameGrid[x, y].transform.parent = transform;
                gameGrid[x, y].name = "Grid Space (X: " + x.ToString() + " , Y: " + y.ToString() + ")";

                GridCell gridCell = gameGrid[x, y].GetComponent<GridCell>();
                gridCell.FillCellValues(interactables[objectIndex]);

                objectIndex++;
            }
        }
        transform.SetPositionAndRotation(new Vector3(0, 0, -2), Quaternion.identity);
    }

    public void DestroyGrid()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public Vector2Int GetGridPosFromWorld(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / GridSpaceSize);
        int y = Mathf.FloorToInt(worldPosition.z / GridSpaceSize);

        x = Mathf.Clamp(x, 0, width);
        y = Mathf.Clamp(x, 0, height);

        return new Vector2Int(x, y);
    }

    public Vector3 GetWorldPositionFromGridPos(Vector2Int gridPos)
    {
        float x = gridPos.x * GridSpaceSize;
        float y = gridPos.y * GridSpaceSize;

        return new Vector3(x, 0, y);
    }
}