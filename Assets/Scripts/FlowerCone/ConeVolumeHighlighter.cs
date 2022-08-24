using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * This script must be attached to the cone that will hit the Interactables.
 * This script represents the Mini Map (circular) selector Interactor
 */

[RequireComponent(typeof(Collider))]
public class ConeVolumeHighlighter : MonoBehaviour
{
    private HashSet<Interactable> allHighlightedObjects;

    public void OnEnable()
    {
        allHighlightedObjects = new HashSet<Interactable>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Interactable interactable))
            return;

        interactable.StartHover();
        allHighlightedObjects.Add(interactable);
    }

    public void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out Interactable interactable))
            return;
        interactable.EndHover();

        allHighlightedObjects.Remove(interactable);
    }

    public List<Interactable> GetAllInteractables()
    {
        return allHighlightedObjects.ToList();
    }
}