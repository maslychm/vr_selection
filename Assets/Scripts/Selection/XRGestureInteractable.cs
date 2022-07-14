using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class XRGestureInteractable : MonoBehaviour
{
    private XRGestureFilterInteractor gestureInteractor;

    // grid selector var to be used instead of gesture interactor when needed
    private XRGridSelectorInteractor gridInteractor;

    // add a third access helper from the MiniMap Interactor
    private MiniMapInteractor miniMapInteractor;

    private LenSelectInteractor LenSelectInteractorHelper;

    [SerializeField] private Material hoverMaterial;

    private Material defaultMaterial;
    private List<MeshRenderer> meshRenderers;

    public bool debug = false;

    private string m_Scene;

    private void Start()
    {
        m_Scene = SceneManager.GetActiveScene().name;

        if (m_Scene == "PitckupTest_Shelves_3_Circular_MiniMap" || m_Scene == "Demo2" || m_Scene.ToLower().Contains("minimap"))
            miniMapInteractor = FindObjectOfType<MiniMapInteractor>();
        else if (m_Scene == "PitckupTest_Shelves_2_GridSelection")
            gridInteractor = FindObjectOfType<XRGridSelectorInteractor>();
        else if (m_Scene == "LenSelect_Implementation")
            LenSelectInteractorHelper = FindObjectOfType<LenSelectInteractor>();
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

        if (m_Scene == "PitckupTest_Shelves_3_Circular_MiniMap" || m_Scene == "Demo2" || m_Scene.ToLower().Contains("minimap"))
            miniMapInteractor.AddtoHighlighted(gameObject);
        else if (m_Scene == "PitckupTest_Shelves_2_GridSelection")
            gridInteractor.AddtoHighlighted(gameObject);
        else if (m_Scene == "LenSelect_Implementation")
        {
            LenSelectInteractorHelper.AddtoHighlighted(gameObject);
        }
        else
            gestureInteractor.AddtoHighlighted(gameObject);
    }

    private void EndHover()
    {
        dprint($"End hover: {this.name}");

        foreach (var mr in meshRenderers)
            mr.material = defaultMaterial;

        if (m_Scene == "PitckupTest_Shelves_3_Circular_MiniMap" || m_Scene == "Demo2" || m_Scene.ToLower().Contains("minimap"))
            miniMapInteractor.RemoveFromHighlighted(gameObject);
        else if (m_Scene == "PitckupTest_Shelves_2_GridSelection")
            gridInteractor.RemoveFromHighlighted(gameObject);
        else if (m_Scene == "LenSelect_Implementation")
            LenSelectInteractorHelper.RemoveFromHighlighted(gameObject);
        else
            gestureInteractor.RemoveFromHighlighted(gameObject);
    }

    private void dprint(string msg)
    {
        if (debug) print(msg);
    }

    public void SetHoverMaterial(Material mat)
    {
        hoverMaterial = mat;
    }
}