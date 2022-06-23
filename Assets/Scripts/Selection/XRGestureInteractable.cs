using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class XRGestureInteractable : MonoBehaviour
{
    private XRGestureFilterInteractor gestureInteractor;
    // grid selector inst
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

        if (gestureInteractor == null)
            gestureInteractor = FindObjectOfType<XRGestureFilterInteractor>();
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
        gestureInteractor.AddtoHighlighted(gameObject);
    }

    private void EndHover()
    {
        dprint($"End hover: {this.name}");

        foreach (var mr in meshRenderers)
            mr.material = defaultMaterial;

        gestureInteractor.RemoveFromHighlighted(gameObject);
    }

    private void dprint(string msg)
    {
        if (debug) print(msg);
    }
}