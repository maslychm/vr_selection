using UnityEngine;

public class GridCell : MonoBehaviour
{
    [SerializeField] private MeshRenderer internalSphereRenderer;

    public Interactable originalInteractable;

    public void FillCellValues(Interactable interactable)
    {
        originalInteractable = interactable;
        internalSphereRenderer.material = interactable.GetDefaultMaterial();
    }
}