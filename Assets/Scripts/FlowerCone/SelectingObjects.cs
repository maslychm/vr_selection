using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectingObjects : MonoBehaviour
{
    // Add tag
    [SerializeField] private string selectedTag = "";

    [SerializeField] private Material defaultMaterial;

    [SerializeField] private Material highlightMaterial;

    private Transform _selection;


    // Update is called once per frame
    void Update()
    {
        if (_selection != null)
        {
            var selectionRenderer = _selection.GetComponent<Renderer>();
            selectionRenderer.material = defaultMaterial;
            _selection = null;
        }

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            var selection = hit.transform;
            var selectionRenderer = selection.GetComponent<Renderer>();
            if (selectionRenderer != null)
            {
                selectionRenderer.material = highlightMaterial;
            }

            _selection = selection;
        }
    }
}
