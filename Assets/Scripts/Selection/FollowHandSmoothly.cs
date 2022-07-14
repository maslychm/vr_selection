using UnityEngine;

public class FollowHandSmoothly : MonoBehaviour
{
    [Tooltip("Filter this Transform's position and rotation and apply it to the target")]
    [SerializeField] private Transform handToFollow;

    [Tooltip("Filter param: how much confidence in the new value? More = slower updates, but less jitter")]
    [Range(0, 1)]
    [SerializeField] private float newValConfidence = .3f;

    [SerializeField] private bool applyFilter = true;

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

        transform.position = handToFollow.position * newValConfidence + transform.position * (1 - newValConfidence);

        // Problem happens when we go around the 0, because then Unity automatically mods by 360
        // If the difference between old value and new value along any of the dimensions is > 180, we need to set the old value into the "opposite" rotation
        // before applying the filter

        // TODO This can probably be resolved easier using quaternions and Lerp

        Vector3 diff = transform.eulerAngles - handToFollow.eulerAngles;
        Vector3 oldNewEulerAngles = transform.eulerAngles;

        if (diff.x < -180)
        {
            oldNewEulerAngles.x += 360;
        }
        else if (diff.x > 180)
        {
            oldNewEulerAngles.x -= 360;
        }

        if (diff.y < -180)
        {
            oldNewEulerAngles.y += 360;
        }
        else if (diff.y > 180)
        {
            oldNewEulerAngles.y -= 360;
        }

        if (diff.z < -180)
        {
            oldNewEulerAngles.z += 360;
        }
        else if (diff.z > 180)
        {
            oldNewEulerAngles.z -= 360;
        }

        transform.eulerAngles = handToFollow.eulerAngles * newValConfidence + oldNewEulerAngles * (1 - newValConfidence);
    }
}