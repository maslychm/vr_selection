using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    private XRGestureFilterInteractor gestureInteractor;

    // grid selector var to be used instead of gesture interactor when needed
    private XRGridSelectorInteractor gridInteractor;

    // this is for later for the flower cone 
    

    internal List<GameObject> ToList()
    {
        throw new NotImplementedException();
    }

    // add a third access helper from the MiniMap Interactor
    private MiniMapInteractor miniMapInteractor;

    private LenSelectInteractor LenSelectInteractorHelper;

    [SerializeField] private Material hoverMaterial;

    private Material defaultMaterial;
    private List<MeshRenderer> meshRenderers;

    public bool debug = false;

    private string m_Scene;

    //GameObject prior = null;

    public static Component currentInteractor = null;

    //private void Start()
    //{
    //    //m_Scene = SceneManager.GetActiveScene().name;

    //    if (SelectionTechniqueDistributer.currentlySetActiveTechnique == null)
    //        return;

    //    else if (SelectionTechniqueDistributer.currentlySetActiveTechnique.name == "MiniMap")
    //    {
    //        miniMapInteractor = FindObjectOfType<MiniMapInteractor>();
    //        currentInteractor = miniMapInteractor;
    //    }
    //    else if (SelectionTechniqueDistributer.currentlySetActiveTechnique.name == "Grid")
    //    {
    //        gridInteractor = FindObjectOfType<XRGridSelectorInteractor>();
    //        currentInteractor = gridInteractor;
    //    }
    //    else if (SelectionTechniqueDistributer.currentlySetActiveTechnique.name == "LenSelect")
    //    {
    //        LenSelectInteractorHelper = FindObjectOfType<LenSelectInteractor>();
    //        currentInteractor = LenSelectInteractorHelper;
    //    }
    //    else if (SelectionTechniqueDistributer.currentlySetActiveTechnique.name == "FlowerCone")
    //    {
    //        // keep empty for now
    //    }
    //    else
    //    {
    //        gestureInteractor = FindObjectOfType<XRGestureFilterInteractor>();
    //        currentInteractor = gestureInteractor;
    //    }
    //    prior = SelectionTechniqueDistributer.currentlySetActiveTechnique;
    //}

    //private void Update()
    //{
    //    if (SelectionTechniqueDistributer.currentlySetActiveTechnique == prior || SelectionTechniqueDistributer.currentlySetActiveTechnique == null)
    //    {
    //        currentInteractor = null;
    //        return;
    //    }
    //    else {
    //        if (SelectionTechniqueDistributer.currentlySetActiveTechnique.name == "MiniMap")
    //        {
    //            miniMapInteractor = FindObjectOfType<MiniMapInteractor>();
    //            currentInteractor = miniMapInteractor;
    //        }
    //        else if (SelectionTechniqueDistributer.currentlySetActiveTechnique.name == "Grid")
    //        {
    //            gridInteractor = FindObjectOfType<XRGridSelectorInteractor>();
    //            currentInteractor = gridInteractor;
    //        }
    //        else if (SelectionTechniqueDistributer.currentlySetActiveTechnique.name == "LenSelect")
    //        {
    //            LenSelectInteractorHelper = FindObjectOfType<LenSelectInteractor>();
    //            currentInteractor = LenSelectInteractorHelper;
    //        }
    //        else if (SelectionTechniqueDistributer.currentlySetActiveTechnique.name == "FlowerCone")
    //        {
    //            // keep empty for now
    //        }
    //        else
    //        {
    //            gestureInteractor = FindObjectOfType<XRGestureFilterInteractor>();
    //            currentInteractor = gestureInteractor;
    //        }

    //        prior = SelectionTechniqueDistributer.currentlySetActiveTechnique;
    //    }
    //}

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

    //public void OnTriggerEnter(Collider other)
    //{
    //    dprint(other.tag);
    //    if (!other.CompareTag("GestureFilter"))
    //        return;

    //    StartHover();
    //}

    //public void OnTriggerExit(Collider other)
    //{
    //    if (!other.CompareTag("GestureFilter"))
    //        return;

    //    EndHover();
    //}


    public void StartHover()
    {
        dprint($"Start hover: {this.name}");

        foreach (var mr in meshRenderers)
            mr.material = hoverMaterial;

        //if (currentInteractor != null)
        //{
        //    if (SelectionTechniqueDistributer.currentlySetActiveTechnique.name == "MiniMap")
        //    {
        //        miniMapInteractor.AddtoHighlighted(gameObject);

        //    }
        //    else if (SelectionTechniqueDistributer.currentlySetActiveTechnique.name == "Grid")
        //    {
        //        gridInteractor.AddtoHighlighted(gameObject);

        //    }
        //    else if (SelectionTechniqueDistributer.currentlySetActiveTechnique.name == "LenSelect")
        //    {
        //        LenSelectInteractorHelper.AddtoHighlighted(gameObject);

        //    }
        //    else if (SelectionTechniqueDistributer.currentlySetActiveTechnique.name == "FlowerCone")
        //    {
        //        // keep empty for now
        //    }
        //    else
        //    {
        //        gestureInteractor.AddtoHighlighted(gameObject);
        //    }

        //}
    }

    public void EndHover()
    {
        dprint($"End hover: {this.name}");

        foreach (var mr in meshRenderers)
            mr.material = defaultMaterial;

        //if (currentInteractor != null)
        //{
        //    if (SelectionTechniqueDistributer.currentlySetActiveTechnique.name == "MiniMap")
        //    {
        //        miniMapInteractor.RemoveFromHighlighted(gameObject);

        //    }
        //    else if (SelectionTechniqueDistributer.currentlySetActiveTechnique.name == "Grid")
        //    {
        //        gridInteractor.RemoveFromHighlighted(gameObject);

        //    }
        //    else if (SelectionTechniqueDistributer.currentlySetActiveTechnique.name == "LenSelect")
        //    {
        //        LenSelectInteractorHelper.RemoveFromHighlighted(gameObject);

        //    }
        //    else if (SelectionTechniqueDistributer.currentlySetActiveTechnique.name == "FlowerCone")
        //    {
        //        // keep empty for now
        //    }
        //    else
        //    {
        //        gestureInteractor.RemoveFromHighlighted(gameObject);
        //    }

        //}
   

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