using Unity.XR.CoreUtils;
using UnityEngine;

/// <summary>
/// Click to move the world center under the person's current position
/// </summary>
public class ResetXROriginCenter : MonoBehaviour
{
    [SerializeField] private Transform resetTransform;
    [SerializeField] private XROrigin player;
    [SerializeField] private Camera playerHead;

    [SerializeField] private float ReferenceHeight_M = 1.70f;

    private Vector3 currentHMDPosition;

    [ContextMenu("Reset Position")]
    public void ResetPosition()
    {
        if (!Application.IsPlaying(gameObject))
            return;

        var rotationAngleY = resetTransform.rotation.eulerAngles.y - playerHead.transform.rotation.eulerAngles.y;
        player.transform.Rotate(0, rotationAngleY, 0);

        var distanceDiff = resetTransform.position - playerHead.transform.position;
        distanceDiff.y = 0;

        player.transform.position += distanceDiff;

        // adjust the height of the player based on the reference height
        currentHMDPosition = playerHead.transform.position;
        float deltaY = currentHMDPosition.y - ReferenceHeight_M;

        player.transform.position -= new Vector3(0, deltaY - 0.5f, 0);
    }
}