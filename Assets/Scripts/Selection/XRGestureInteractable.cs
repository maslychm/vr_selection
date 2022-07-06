using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private Component chosenComponentToBeUsedNow;

    Scene m_Scene2;
    string m_Scene;
    private void Start()
    {
        m_Scene2 = SceneManager.GetActiveScene();
        m_Scene = m_Scene2.name;

        if (m_Scene == "PitckupTest_Shelves_3_Circular_MiniMap")
            thirdInteractorHelper = FindObjectOfType<MiniMapInteractor>();
        else if (m_Scene == "PitckupTest_Shelves_2_GridSelection")
            secondInteractorHelper = FindObjectOfType<XRGridSelectorInteractor>();
        else
            gestureInteractor = FindObjectOfType<XRGestureFilterInteractor>();

    }

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

        if (m_Scene == "PitckupTest_Shelves_3_Circular_MiniMap")
            thirdInteractorHelper.AddtoHighlighted(gameObject);
        else if (m_Scene == "PitckupTest_Shelves_2_GridSelection")
            secondInteractorHelper.AddtoHighlighted(gameObject);
        else
            gestureInteractor.AddtoHighlighted(gameObject);
    }

    private void EndHover()
    {
        dprint($"End hover: {this.name}");

        foreach (var mr in meshRenderers)
            mr.material = defaultMaterial;

        if (m_Scene == "PitckupTest_Shelves_3_Circular_MiniMap")
            thirdInteractorHelper.RemoveFromHighlighted(gameObject);
        else if (m_Scene == "PitckupTest_Shelves_2_GridSelection")
            secondInteractorHelper.RemoveFromHighlighted(gameObject);
        else
            gestureInteractor.RemoveFromHighlighted(gameObject);

    }

    private void dprint(string msg)
    {
        if (debug) print(msg);
    }
}