using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SetHighlightedMaterials : MonoBehaviour
{
    private void Start()
    {
        /**
         * Assign lighter materials to each interactable in the scene.
         * Material swapping gets laggy when flicking over >20 objects >4 times per second.
         */
        List<Interactable> interactables = FindObjectsOfType<Interactable>().ToList();
        foreach (var interactable in interactables)
        {
            Material highlightedMat = new Material(interactable.GetComponent<Renderer>().material);
            highlightedMat.color += new Color(.4f, .4f, .4f);
            interactable.SetHoverMaterial(highlightedMat);
        }
    }
}