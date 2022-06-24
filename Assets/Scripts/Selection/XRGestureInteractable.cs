using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class XRGestureInteractable : MonoBehaviour
{
    private XRGestureFilterInteractor gestureInteractor;

    // grid selector var to be used instead of gesture interactor when needed
    private XRGridSelectorInteractor secondInteractorHelper; 
    
    [SerializeField] private Material hoverMaterial;

    private Material defaultMaterial;
    private List<MeshRenderer> meshRenderers;

    public bool debug = false;

    private void Awake()
    {
        meshRenderers = new List<MeshRenderer>(GetComponents<MeshRenderer>());
        if (meshRenderers.Count == 0)
            meshRenderers = new List<MeshRenderer>(GetComponentsInChildren<MeshRenderer>());
        defaultMaterial = meshRenderers[0].material;

        if (secondInteractorHelper == null)
            gestureInteractor = FindObjectOfType<XRGestureFilterInteractor>();

        // added to override the inability to access components through the gesture Interactor 
            if (gestureInteractor == null)
                secondInteractorHelper = FindObjectOfType<XRGridSelectorInteractor>();
    }

    // add a getter for the meshrenderer list 
    public List<MeshRenderer> getListOfAllObjects()
    {
        return meshRenderers;
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

        foreach (var mr in meshRenderers)
            mr.material = hoverMaterial;

        // check which one is ready to be used 
        if (gestureInteractor != null)

            gestureInteractor.AddtoHighlighted(gameObject);

        else

            secondInteractorHelper.AddtoHighlighted(gameObject);
    }

    private void EndHover()
    {
        dprint($"End hover: {this.name}");

        foreach (var mr in meshRenderers)
            mr.material = defaultMaterial;

        // check which one is ready to be used 
        if(gestureInteractor != null)

            gestureInteractor.RemoveFromHighlighted(gameObject);

        else

            secondInteractorHelper?.RemoveFromHighlighted(gameObject);
    }

    private void dprint(string msg)
    {
        if (debug) print(msg);
    }
}