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

    private Vector3 lastFilteredPosition, lastFilteredRotation;

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
        // Saves vectors for positions externally instead of relying on own transform.position for getting last position
        // The suspicion is that Unity updates the transforms and positions of children before the update is called -> this is true

        transform.position = handToFollow.position * newValConfidence + lastFilteredPosition * (1 - newValConfidence);
        lastFilteredPosition = transform.position;

        // Problem happens when we go around the 0, because then Unity automatically mods by 360
        // If the difference between old value and new value along any of the dimensions is > 180, we need to set the old value into the "opposite" rotation
        // before applying the filter

        Vector3 diff = lastFilteredRotation - handToFollow.eulerAngles;
        Vector3 oldFixedEulerAngles = lastFilteredRotation;

        if (diff.x < -180)
        {
            oldFixedEulerAngles.x += 360;
        }
        else if (diff.x > 180)
        {
            oldFixedEulerAngles.x -= 360;
        }

        if (diff.y < -180)
        {
            oldFixedEulerAngles.y += 360;
        }
        else if (diff.y > 180)
        {
            oldFixedEulerAngles.y -= 360;
        }

        if (diff.z < -180)
        {
            oldFixedEulerAngles.z += 360;
        }
        else if (diff.z > 180)
        {
            oldFixedEulerAngles.z -= 360;
        }

        transform.eulerAngles = handToFollow.eulerAngles * newValConfidence + oldFixedEulerAngles * (1 - newValConfidence);
        lastFilteredRotation = transform.eulerAngles;
    }

    private void SmoothingStepMethodAsChildOfNotTracked()
    {
        transform.position = handToFollow.position * newValConfidence + transform.position * (1 - newValConfidence);

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