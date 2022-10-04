using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GravityZone : MonoBehaviour
{
    [SerializeField] private InputActionReference raySelectActionReference;
    [SerializeField] private InputActionReference gravityPullActionReference;
    [SerializeField] private Transform xrOriginTransform; // for Y offset
    [SerializeField] private Transform startPointTransform; // for X and Z offset
    [SerializeField] private float pullSpeed = 0.0025f;
    [SerializeField] private Transform rayStartPoint;
    [SerializeField] private GrabbingHand grabbingHand;
    [SerializeField] private GameObject selectionRayObject;
    [SerializeField] private float transparencyDistance = 1f;

    private List<Interactable> interactablesToPull;
    private Dictionary<Interactable, Vector3> interactablePullDirections;
    private Vector3 xrOriginFixedPosition;


    [SerializeField] private float uninteruptedAddFactor = 0.01f;
    private float uninteruptedTime;

    private void OnEnable()
    {
        // X and Z from the reference object, and Y + height from the XR Origin
        // This is to make the balls consistently go into participant's mouth
        xrOriginFixedPosition = new Vector3(
            startPointTransform.position.x,
            startPointTransform.position.y + 1.7f,
            startPointTransform.position.z
        );

        interactablesToPull = FindObjectsOfType<Interactable>()
            .ToList()
            .Where(x => x.isActiveAndEnabled)
            .ToList();

        interactablePullDirections = new Dictionary<Interactable, Vector3>();

        foreach (var interactable in interactablesToPull)
        {
            Vector3 dir = interactable.transform.position - xrOriginFixedPosition;
            interactablePullDirections.Add(interactable, dir);
        }
    }

    private void SelectWithRay()
    {
        if (Physics.Raycast(rayStartPoint.position, rayStartPoint.forward, out RaycastHit hit, Mathf.Infinity))
        {
            if (hit.collider.TryGetComponent(out Interactable interactable))
            {
                grabbingHand.CallPickUpObject(interactable);
            }
        }
    }

    public void ResetInteractables()
    {
        foreach (var i in interactablesToPull)
        {
            i.GetComponent<MeshRenderer>().enabled = true;
            i.GetComponent<Collider>().enabled = true;
            i.GetComponent<Object_collected>().ResetGameObject();
        }
    }

    public void UpdateSearchTargetDirection(SearchTargetInteractable t)
    {
        Vector3 dir = t.transform.position - xrOriginFixedPosition;
        interactablePullDirections[t] = dir;
    }

    private void FixedUpdate()
    {
        float joyYInput = gravityPullActionReference.action.ReadValue<Vector2>().y;

        if (Mathf.Abs(joyYInput) < float.Epsilon)
        {
            // reset the uninteruptedTime
            uninteruptedTime = 1;
            return;
        }
        else
        {
            uninteruptedTime += uninteruptedAddFactor;
            uninteruptedTime = Mathf.Clamp(uninteruptedTime, 1, 3);
        }

        //float processedJoyYInput = 1.2f * Mathf.Sign(joyYInput) * joyYInput * joyYInput; // 1.2x^2 
        float processedJoyYInput = Mathf.Sign(joyYInput) * joyYInput * joyYInput * uninteruptedTime; // x^2 * timeFactor 
        float pullOffset = processedJoyYInput * pullSpeed;

        if (Mathf.Abs(pullOffset) < float.Epsilon)
            return;

        foreach (var kv in interactablePullDirections)
        {
            var interactable = kv.Key;
            var direction = kv.Value;

            interactable.transform.position += direction * pullOffset;

            // if distance is too small, disable renderers and colliders on interactable
            if (Vector3.Distance(interactable.transform.position, xrOriginFixedPosition) < transparencyDistance)
            {
                interactable.GetComponent<MeshRenderer>().enabled = false;
                interactable.GetComponent<Collider>().enabled = false;
            }
            else
            {
                interactable.GetComponent<MeshRenderer>().enabled = true;
                interactable.GetComponent<Collider>().enabled = true;
            }
        }
    }

    private void Update()
    {
        if (raySelectActionReference.action.WasPressedThisFrame())
        {
            SelectWithRay();
        }
    }
}