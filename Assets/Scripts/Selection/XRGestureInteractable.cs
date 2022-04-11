using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class XRGestureInteractable : MonoBehaviour
{
    private XRGestureFilterInteractor gestureInteractor;

    [SerializeField] private Material hoverMaterial;

    private Material defaultMaterial;
    private MeshRenderer meshRenderer;

    public bool debug = false;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        defaultMaterial = meshRenderer.material;

        if (gestureInteractor == null)
        {
            gestureInteractor = FindObjectOfType<XRGestureFilterInteractor>();
        }
    }

    private void Update()
    {
    }

    public void OnTriggerEnter(Collider other)
    {
        dprint(other.tag);
        if (!other.CompareTag("GestureFilter"))
            return;

        StartHover();
    }

    public void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("GestureFilter"))
            return;

        EndHover();
    }

    private void StartHover()
    {
        dprint($"Start hover: {this.name}");
        meshRenderer.material = hoverMaterial;
        gestureInteractor.AddtoHighlighted(gameObject);
    }

    private void EndHover()
    {
        dprint($"End hover: {this.name}");
        meshRenderer.material = defaultMaterial;
        gestureInteractor.RemoveFromHighlighted(gameObject);
    }

    private void dprint(string msg)
    {
        if (debug) print(msg);
    }
}