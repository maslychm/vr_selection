using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// NEED TO CHANGE VALUES OF BOX COLLIDER

// This is a script on every cell on the grid
public class GridCell : MonoBehaviour
{
    // Stores actual position on the grid that the cell is
    private int posX;
    private int posY;

    // Saves a reference to the gameobject that gets placed on this cell
    public GameObject objectInThisGridSpace = null;

    // Saves if the grid space is occupied or not
    public bool isOccupied = false;

    // Set the position of this grid cell on the grid
    public void SetPosition(int x, int y)
    {
        posX = x;
        posY = y;
    }

    // Get the position of this grid space on the grid
    public Vector2Int GetPosition()
    {
        return new Vector2Int(posX, posY);
    }
}
