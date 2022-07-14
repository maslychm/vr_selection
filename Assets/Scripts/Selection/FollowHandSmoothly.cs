using UnityEngine;

public class FollowHandSmoothly : MonoBehaviour
{
    [Tooltip("Filter this Transform's position and rotation and apply it to the target")]
    [SerializeField] private Transform handToFollow;

    [Tooltip("Filter param: how much confidence in the new value? More = slower updates, but less jitter")]
    [Range(0, 1)]
    [SerializeField] private float newValConfidence = .3f;

    [SerializeField] private bool applyFilter = true;
    [SerializeField] private bool isChildOfTracked = true;

    private Vector3 lastFilteredPosition;
    private Quaternion lastFilteredQuaternion;

    private void Start()
    {
        if (handToFollow.gameObject.activeInHierarchy == false)
        {
            handToFollow = GameObject.Find("LeftHand Controller -YES").transform;
        }
    }

    private void Update()
    {
        if (!applyFilter)
        {
            transform.position = handToFollow.position;
            transform.eulerAngles = handToFollow.eulerAngles;
            return;
        }

        if (isChildOfTracked)
            SmoothingStepMethodAsChildOfTracked();
        else
            SmoothingStepMethodAsChildOfNotTracked();
    }

    private void SmoothingStepMethodAsChildOfTracked()
    {
        transform.position = Vector3.Lerp(lastFilteredPosition, handToFollow.position, newValConfidence);
        lastFilteredPosition = transform.position;

        transform.rotation = Quaternion.Lerp(lastFilteredQuaternion, handToFollow.rotation, newValConfidence);
        lastFilteredQuaternion = transform.rotation;
    }

    private void SmoothingStepMethodAsChildOfNotTracked()
    {
        transform.SetPositionAndRotation(
            Vector3.Lerp(transform.position, handToFollow.position, newValConfidence),
            Quaternion.Lerp(transform.rotation, handToFollow.rotation, newValConfidence)
            );
    }
}