using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class RayKebabManager : MonoBehaviour
{
    private List<Interactable> rayCastedInteractables;
    private float startOffsetOFspheres = 0.05f;

    [SerializeField] private InputActionReference raySelectActionReference;

    public static bool turnWhite = false;

    public LineRenderer lineRenderer; // stor ethe current puyrpole linerenderer

    public Material whiteMaterial, RedMaterial;

    // add a boolean to limit the the selection action to one button
    public static int BringOrFlush = 0;

    private RaycastHit[] hits;

    public GrabbingHand grabbingHand;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.material = whiteMaterial;
    }

    private void OnEnable()
    {
        rayCastedInteractables = new List<Interactable>();
        grabbingHand = FindObjectOfType<GrabbingHand>();
    }

    private void Update()
    {
        if (turnWhite == true || (lineRenderer.material == RedMaterial && rayCastedInteractables.Count == 0))
        {
            lineRenderer.material = whiteMaterial;
        }

        if (raySelectActionReference.action.WasPressedThisFrame() && BringOrFlush == 0)
        {
            ProcessInputHere();
        }
        else if (raySelectActionReference.action.WasPressedThisFrame() && BringOrFlush == 1)
        {
            if (rayCastedInteractables.Count() < 2)
            {
                lineRenderer.material = whiteMaterial;
                BringOrFlush = 0;
                rayCastedInteractables.Clear();
                return;
            }
            ReleaseInteractablesFromRay();

            if (rayCastedInteractables.Count > 0)
                rayCastedInteractables.Clear();
            lineRenderer.material = whiteMaterial;
            BringOrFlush = 0;
        }
    }

    /// <summary>
    /// This typically just places the spheres hit by the raycast in the hand in a BBQ Shape
    /// ----[][][][][]-------------------
    /// </summary>
    private void ProcessInputHere()
    {
        // store the array of colliders hit by the raycast
        hits = Physics.RaycastAll(transform.position, transform.forward, 100.0F);
        rayCastedInteractables.Clear();

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.TryGetComponent(out Interactable interactable))
                rayCastedInteractables.Add(interactable);
        }

        if (rayCastedInteractables.Count == 0)
            return;

        if (rayCastedInteractables.Count == 1)
        {
            grabbingHand.CallPickUpObject(rayCastedInteractables.ElementAt(0).GetComponent<Interactable>());
            grabbingHand.ClearGrabbed();

            startOffsetOFspheres = 0.05f;
            BringOrFlush = 0;

            return;
        }
        foreach (Interactable tempGameObjectFromSet in rayCastedInteractables)
        {
            tempGameObjectFromSet.transform.SetParent(transform);

            tempGameObjectFromSet.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
            tempGameObjectFromSet.transform.position = transform.TransformPoint(transform.localPosition + new Vector3(0, 0, startOffsetOFspheres));

            startOffsetOFspheres += 0.085f;
        }

        lineRenderer.material = RedMaterial;
        startOffsetOFspheres = 0.05f;
        BringOrFlush = 1;
    }

    // thios functiuobn should set back the objects to their original transform
    public void ReleaseInteractablesFromRay()
    {
        foreach (Interactable interactable in rayCastedInteractables)
        {
            if (ExperimentTrial.activeTrial == null 
                && SearchExperimentTrial.activeTrial == null)
            {
                interactable.GetComponent<Object_collected>().ResetGameObject();
            }
            else
            {
                // If a trial is active and the wrong object was picked up, target
                // interactable should go to it's last position, not the "reset" one
                if (interactable.TryGetComponent(out TargetInteractable target))
                {
                    print("wrong -> manually resetting target in curr trial");
                    target.ResetTargetForCurrentTrial();
                }
                else
                {
                    interactable.GetComponent<Object_collected>().ResetGameObject();
                }
            }
        }
        lineRenderer.material = whiteMaterial;
        BringOrFlush = 0;
    }

    public void RemoveOneInteractableFromKebabList(Interactable interactable)
    {
        rayCastedInteractables.Remove(interactable);
    }
}