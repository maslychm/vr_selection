using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    //private XRGestureFilterInteractor gestureInteractor;

    // grid selector var to be used instead of gesture interactor when needed
    //private XRGridSelectorInteractor gridInteractor;

    // this is for later for the flower cone

    internal List<GameObject> ToList()
    {
        throw new NotImplementedException();
    }

    // add a third access helper from the MiniMap Interactor
    //private MiniMapInteractor miniMapInteractor;

    //private LenSelectInteractor LenSelectInteractorHelper;

    [SerializeField] private Material hoverMaterial;

    private Material defaultMaterial;
    private List<MeshRenderer> meshRenderers;

    public bool debug = false;

    //private string m_Scene;

    public static Component currentInteractor = null;

    private void Awake()
    {
        meshRenderers = new List<MeshRenderer>(GetComponents<MeshRenderer>());
        if (meshRenderers.Count == 0)
            meshRenderers = new List<MeshRenderer>(GetComponentsInChildren<MeshRenderer>());
        defaultMaterial = meshRenderers[0].material;
    }

    // add a getter for the meshrenderer list
    public List<MeshRenderer> getListOfAllObjects()
    {
        return meshRenderers;
    }

    public void StartHover()
    {
        dprint($"Start hover: {this.name}");

        foreach (var mr in meshRenderers)
            mr.material = hoverMaterial;
    }

    public void EndHover()
    {
        dprint($"End hover: {this.name}");

        foreach (var mr in meshRenderers)
            mr.material = defaultMaterial;
    }

    public void dprint(string msg)
    {
        if (debug) print(msg);
    }

    public void SetHoverMaterial(Material mat)
    {
        hoverMaterial = mat;
    }
}