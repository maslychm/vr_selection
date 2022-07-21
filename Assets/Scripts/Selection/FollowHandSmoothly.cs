using UnityEngine;

public class FollowHandSmoothly : MonoBehaviour
{
    [Tooltip("Filter this Transform's position and rotation and apply it to the target")]
    [SerializeField] private Transform handToFollow;

    [Tooltip("Filter param: how much confidence in the new value? More = slower updates, but less jitter")]
    [Range(0, 1)]
    [SerializeField] private float newValConfidence = .3f;

    [SerializeField] private bool applyFilter = true;

    private Vector3 localOffsetInTracked, lastFilteredPosition;
    private Quaternion localQuatInTracked, lastFilteredQuaternion;

    private void Start()
    {
        if (handToFollow.gameObject.activeInHierarchy == false)
        {
            handToFollow = GameObject.Find("LeftHand Controller -YES").transform;
        }

        // Calculate local offsets
        localOffsetInTracked = handToFollow.InverseTransformVector(transform.position - handToFollow.position);
        localQuatInTracked = transform.rotation;
    }

    private void Update()
    {
        if (!applyFilter)
            return;
        ApplySmoother();
    }

    private void ApplySmoother()
    {
        transform.position = Vector3.Lerp(lastFilteredPosition, handToFollow.TransformPoint(localOffsetInTracked), newValConfidence);
        lastFilteredPosition = transform.position;

        transform.rotation = Quaternion.Lerp(lastFilteredQuaternion, handToFollow.rotation * localQuatInTracked, newValConfidence);
        lastFilteredQuaternion = transform.rotation;
    }
}