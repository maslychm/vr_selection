using UnityEngine;

public class GridCell : MonoBehaviour
{
    [SerializeField] private MeshRenderer internalSphereRenderer;

    public Interactable originalInteractable;

    public void SetReference(Interactable interactable)
    {
        originalInteractable = interactable;
        internalSphereRenderer.material = interactable.GetComponent<MeshRenderer>().material;
    }
}