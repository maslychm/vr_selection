using UnityEngine;

public class XRGazeReticle : MonoBehaviour
{
    [SerializeField] private Camera CameraFacing;
    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    private void Update()
    {
        RaycastHit hit;
        LayerMask layerMask = ~LayerMask.GetMask("UI");
        var ray = new Ray(CameraFacing.transform.position, CameraFacing.transform.forward);

        float distance;
        if (Physics.Raycast(ray, out hit, layerMask: layerMask, maxDistance: 30))
        {
            distance = hit.distance;
        }
        else
        {
            distance = CameraFacing.farClipPlane * .95f;
        }

        transform.position = CameraFacing.transform.position + CameraFacing.transform.rotation * Vector3.forward * distance;
        transform.forward = CameraFacing.transform.forward;
        transform.localScale = originalScale * distance;
    }
}