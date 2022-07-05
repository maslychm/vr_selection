using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class XRGestureInteractable : MonoBehaviour
{
    private XRGestureFilterInteractor gestureInteractor;

    // grid selector var to be used instead of gesture interactor when needed
    private XRGridSelectorInteractor secondInteractorHelper;

    // add a third access helper from the MiniMap Interactor 
    private MiniMapInteractor thirdInteractorHelper;
    
    [SerializeField] private Material hoverMaterial;

    private Material defaultMaterial;
    private List<MeshRenderer> meshRenderers;

    public bool debug = false;

    private List<Component> listOfFiles_Components_ToBe_Used;

    private Component chosenComponentToBeUsedNow;

    private void Awake()
    {
        // store the whole components in one list for ease of use and dynamic access/switch 
        listOfFiles_Components_ToBe_Used = new List<Component>();
        listOfFiles_Components_ToBe_Used.Add(gestureInteractor);
        listOfFiles_Components_ToBe_Used.Add(secondInteractorHelper);
        listOfFiles_Components_ToBe_Used.Add(thirdInteractorHelper);


        meshRenderers = new List<MeshRenderer>(GetComponents<MeshRenderer>());
        if (meshRenderers.Count == 0)
            meshRenderers = new List<MeshRenderer>(GetComponentsInChildren<MeshRenderer>());
        defaultMaterial = meshRenderers[0].material;

        if (secondInteractorHelper == null && thirdInteractorHelper == null)
            gestureInteractor = FindObjectOfType<XRGestureFilterInteractor>();

        // added to override the inability to access components through the gesture Interactor 
        if (gestureInteractor == null && thirdInteractorHelper == null)
            secondInteractorHelper = FindObjectOfType<XRGridSelectorInteractor>();

        // new
        if (thirdInteractorHelper != null)
            thirdInteractorHelper  =  FindObjectOfType<MiniMapInteractor>();


        // iterate through all the possible options to call the functions later 
        // Suggestion ---------------------------------------------TO BE CHECKED---------------------------------------------
        // ------------------------------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------------------------------
            /* for(int i = 0; i < listOfFiles_Components_ToBe_Used.Count; i++)
             {
                 if (listOfFiles_Components_ToBe_Used[i] == gestureInteractor && listOfFiles_Components_ToBe_Used[i] != null)
                 {
                     gestureInteractor =  FindObjectOfType<XRGestureFilterInteractor>();
                     listOfFiles_Components_ToBe_Used[i] =  FindObjectOfType<XRGestureFilterInteractor>();
                     chosenComponentToBeUsedNow = gestureInteractor;
                 }
                 else if (listOfFiles_Components_ToBe_Used[i] == secondInteractorHelper && listOfFiles_Components_ToBe_Used[i] != null)
                 {
                     secondInteractorHelper = FindObjectOfType<XRGridSelectorInteractor>();
                     listOfFiles_Components_ToBe_Used[i] =  FindObjectOfType<XRGridSelectorInteractor>();
                     chosenComponentToBeUsedNow = secondInteractorHelper;
                 }
                 else if (listOfFiles_Components_ToBe_Used[i] == thirdInteractorHelper && listOfFiles_Components_ToBe_Used[i] != null)
                 {
                     thirdInteractorHelper = FindObjectOfType<MiniMapInteractor>();
                     listOfFiles_Components_ToBe_Used[i] =  FindObjectOfType<MiniMapInteractor>();
                     chosenComponentToBeUsedNow = thirdInteractorHelper;
                 }
             }*/
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

        /* for(int i = 0; i < listOfFiles_Components_ToBe_Used.Count; i++)
             if (listOfFiles_Components_ToBe_Used[i] != null)
                 listOfFiles_Components_ToBe_Used[i].AddtoHighlighted(gameObject);

         */
        if (secondInteractorHelper == null && thirdInteractorHelper == null)
            gestureInteractor.AddtoHighlighted(gameObject);

        if (gestureInteractor == null && thirdInteractorHelper == null)
            secondInteractorHelper.AddtoHighlighted(gameObject);

        if (thirdInteractorHelper != null)
            thirdInteractorHelper.AddtoHighlighted(gameObject);

    }

    private void EndHover()
    {
        dprint($"End hover: {this.name}");

        foreach (var mr in meshRenderers)
            mr.material = defaultMaterial;


        if (secondInteractorHelper == null && thirdInteractorHelper == null)
            gestureInteractor.RemoveFromHighlighted(gameObject);

        if (gestureInteractor == null && thirdInteractorHelper == null)
            secondInteractorHelper.RemoveFromHighlighted(gameObject);

        if (thirdInteractorHelper != null)
            thirdInteractorHelper.RemoveFromHighlighted(gameObject);


    }

    private void dprint(string msg)
    {
        if (debug) print(msg);
    }
}