using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerInputManager : MonoBehaviour
{
    GameGrid gameGrid;

    // Only hit everything on that layer mask - for raycast
    [SerializeField] private LayerMask whatIsAGridLayer;

    // Start is called before the first frame update
    void Start()
    {
        gameGrid = FindObjectOfType<GameGrid>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if mouse is actually over an object
        GridCell cellMouseIsOver = IsMouseOverAGridSpace();

        // Looking for a mouse click
        // Change control
        if (cellMouseIsOver != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Debug.Log(cellMouseIsOver.isOccupied);
                cellMouseIsOver.GetComponentInChildren<SpriteRenderer>().material.color = Color.green;

            }
        }
    }

    // Returns the grid cell if mouse is over a grid cell and returns null if it is not
    private GridCell IsMouseOverAGridSpace()
    {
        // Mouse position through camera - Change
        // Axis1D.SecondaryIndexTrigger	
        // Input.mousePosition

        // Object reference not set to an instance of an object ???
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        /*if (Camera.main.ScreenPointToRay(Input.mousePosition) != null)
        {
            // Something
        }
        */

        // The ray cast and store into variable
        // If it did hit something returns it, return the gridcell compent
        // Change this
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, whatIsAGridLayer))
        {
            return hitInfo.transform.GetComponent<GridCell>();
        }
        else
        {
            return null;
        }
        
    }
}
