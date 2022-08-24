using UnityEngine;

public class GridCell : MonoBehaviour
{
    [SerializeField] private MeshRenderer internalSphereRenderer;

    public Interactable originalInteractable;

    [SerializeField] private GameObject underlyingSphere;

    public void FillCellValues(Interactable interactable)
    {
        originalInteractable = interactable;
        internalSphereRenderer.material = interactable.GetDefaultMaterial();

        if (interactable.GetComponent<TargetInteractable>())
        {
            cakeslice.Outline targetoutl = underlyingSphere.AddComponent<cakeslice.Outline>();
            targetoutl.color = 1;
            targetoutl.enabled = true;
        }
    }
}