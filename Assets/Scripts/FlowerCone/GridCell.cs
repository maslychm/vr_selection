using UnityEngine;

// This is a script on every cell on the grid
public class GridCell : MonoBehaviour
{
    public GameObject ActivateObject;

    public PassInteractablesToGrid passInteractablesToGrid;
    [SerializeField] private GameObject cellObjects;

    // Stores actual position on the grid that the cell is
    private int posX;

    private int posY;

    // Set the position of this grid cell on the grid
    public void SetPosition(int x, int y)
    {
        posX = x;
        posY = y;
    }

    // Saves a reference to the gameobject that gets placed on this cell
    // public GameObject objectInThisGridSpace = null;
    public GameObject objectInThisGridSpace;

    public void SetMaterial(Material interactable)
    {
        // objectInThisGridSpace = interactable;

        // Change the material to match interactable
        ActivateObject.GetComponent<MeshRenderer>().material = interactable;

        //print("Material" + myMaterial);

        ActivateObject.SetActive(true);
    }

    // refernce the highlighted object in the grid. todo - if the object in the grid is select gets orginal
    public void SetReference(GameObject reference)
    {
        if (ActivateObject.activeSelf)
        {
            objectInThisGridSpace = reference;
        }
    }

    // Saves if the grid space is occupied or not
    // public bool isOccupied = false;
    public bool isOccupied = true;

    // Get the position of this grid space on the grid
    public Vector2Int GetPosition()
    {
        return new Vector2Int(posX, posY);
    }
}