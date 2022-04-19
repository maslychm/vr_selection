using UnityEngine;

public class Object_collected : MonoBehaviour
{
    private Logging_XR logger;

    //private Transform _transform;
    private Vector3 _home_pos;

    private Vector3 _dumpster_location = new Vector3(12,1,-3);
    private Quaternion _home_rot;
    private Renderer _renderer;
    private Collider _collider;
    private Rigidbody _rigidbody;

    private void Start()
    {
        logger = GameObject.Find("XR_Logging_Obj").GetComponent<Logging_XR>();
        if (logger == null)
            Debug.LogWarning("No Logger found.");
        //_transform = GetComponent<Transform>();
        _home_pos = transform.position;
        _home_rot = transform.rotation;
        //_renderer = GetComponent<Renderer>();
        //_collider = GetComponent<Collider>();
        //_rigidbody = GetComponent<Rigidbody>();
    }

    //Reset to original position
    public void ResetGameObject()
    {
        //_renderer.enabled = true;
        //_collider.enabled = true;
        //_rigidbody.isKinematic = false;
        transform.position = _home_pos;
        transform.rotation = _home_rot;
        logger.stop_task_timer();
    }

    private void FreezeGameObject()
    {
        //_renderer.enabled = false;
        //_collider.enabled = false;
        //_rigidbody.isKinematic = true;
        transform.position = _dumpster_location + new Vector3(2 * UnityEngine.Random.Range(0,10)*0.1f,0,2* UnityEngine.Random.Range(0,1)*0.1f);
        // logger.stop_task_timer();
    }

    public void StopCountdownAndFreeze()
    {
        logger.stop_task_timer();
        FreezeGameObject();

        // // TODO FIXME: decide what to do with correctly picked object
        // ResetGameObject();
    }

    //Use this function when starting to pick up an object.
    public void StartCountdown()
    {
        logger.start_task_timer();
    }

    //This assumes we put the gameobject inside a sphere with collision that triggers our tracking.
    private void OnTriggerEnter(Collider collider)
    {
        if (logger != null)
        {
            switch (collider.tag)
            {
                case "end_manipulator":
                    //Pickup
                    logger.start_task_timer();
                    break;

                case "desk_goal":
                    //We reached the goal now explode.
                    logger.stop_task_timer();
                    //GameObject.Destroy(this);
                    FreezeGameObject();
                    break;

                case "failure":
                    //Return this object back to the hell it came from.
                    logger.stop_task_timer();
                    transform.position = _home_pos;
                    transform.rotation = _home_rot;
                    break;
            }
        }
    }
}