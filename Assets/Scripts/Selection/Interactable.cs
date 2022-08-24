using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    [SerializeField] private Material hoverMaterial;
    public cakeslice.Outline interactionOutline = null;

    private Material defaultMaterial;
    private List<MeshRenderer> meshRenderers;

    public bool debug = false;

    private void Awake()
    {
        meshRenderers = new List<MeshRenderer>(GetComponents<MeshRenderer>());
        if (meshRenderers.Count == 0)
            meshRenderers = new List<MeshRenderer>(GetComponentsInChildren<MeshRenderer>());
        defaultMaterial = meshRenderers[0].material;
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

    public void OnSelect()
    {
        if (ExperimentTrial.activeTrial == null)
        {
            return;
        }

        if (TryGetComponent(out TargetInteractable _))
        {
            ExperimentTrial.activeTrial.RecordTargetHit();
            GetComponent<Object_collected>().ResetGameObject();
        }
        else
        {
            ExperimentTrial.activeTrial.RecordTargetMiss();
            GetComponent<Object_collected>().ResetGameObject();
        }
    }
}