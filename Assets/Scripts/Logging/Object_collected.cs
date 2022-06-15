using UnityEngine;
using UnityEngine.SceneManagement;

public class Object_collected : MonoBehaviour
{
    private Logging_XR logger;

    //private Transform _transform;
    private Vector3 _home_pos;

    private Vector3 _dumpster_location = new Vector3(12, 1, -3);
    private Quaternion _home_rot;
    private Rigidbody _rigidbody;

    private void Start()
    {
        _home_pos = transform.position;
        _home_rot = transform.rotation;
        _rigidbody = GetComponent<Rigidbody>();

        // Don't need the object collection stuff if we're in demo
        // Workaround for bypassing making new prefabs

        if (SceneManager.GetActiveScene().name.Contains("Demo"))
            return;

        var logObj = GameObject.Find("XR_Logging_Obj");

        if (logObj != null)
            logger = logObj.GetComponent<Logging_XR>();

        if (logger == null)
            Debug.LogWarning("No Logger found.");
    }

    //Reset to original position
    public void ResetGameObject()
    {
        _rigidbody.velocity = Vector3.zero;
        transform.position = _home_pos;
        transform.rotation = _home_rot;
    }

    public void MoveOutsideReach()
    {
        transform.position = _dumpster_location + new Vector3(
            2 * UnityEngine.Random.Range(0, 10) * 0.1f, 0,
            2 * UnityEngine.Random.Range(0, 1) * 0.1f);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (logger != null)
        {
            switch (collider.tag)
            {
                case "end_manipulator":
                    break;

                //case "desk_goal":
                //    //We reached the goal now explode.
                //    logger.StopTaskTimer();
                //    //GameObject.Destroy(this);
                //    MoveOutsideReach();
                //    break;

                case "failure":
                    //Return this object back to the hell it came from.
                    transform.position = _home_pos;
                    transform.rotation = _home_rot;
                    break;
            }
        }
    }
}