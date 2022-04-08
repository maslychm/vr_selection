using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_collected : MonoBehaviour
{
    private Logging_XR logger;
    private Transform _transform;
    private Vector3 _home_pos;
    private Quaternion _home_rot;
    private Renderer _renderer;
    private Collider _collider;
    private Rigidbody _rigidbody;

    void Start()
    { 
        logger = GameObject.Find("XR_Logging_Obj").GetComponent<Logging_XR>();
        if(logger == null)
            Debug.LogWarning("No Logger found.");
        _transform = GetComponent<Transform>();
        _home_pos = _transform.position;
        _home_rot = _transform.rotation;
        _renderer = GetComponent<Renderer>();
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    //Reset to original position
    public void ResetGameObject() 
    {
        _renderer.enabled = true;
        _collider.enabled = true;
        _rigidbody.isKinematic = false;
        _transform.position = _home_pos;
        _transform.rotation = _home_rot;
    }

    void FreezeGameObject()
    { 
        _renderer.enabled = false;  
        _collider.enabled = false;
        _rigidbody.isKinematic = true;
    }

    //This assumes we put the gameobject inside a sphere with collision that triggers our tracking.
    void OnTriggerEnter(Collider collider)
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
                    _transform.position = _home_pos;
                    _transform.rotation = _home_rot;
                    break;
            }
        }
    }

}
