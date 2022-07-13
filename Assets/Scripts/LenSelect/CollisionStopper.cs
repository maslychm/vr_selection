using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// this script should make the objects stop interacting with each other so that 
/// we avoid sudden jumps and keep the position untouched 
/// </summary>
public class CollisionStopper : MonoBehaviour
{
    private List<string> interactableTags = new List<string>() { "cube", "sphere", "star", "pyramid", "cylinder", "infinity" };
    public Transform originalTransform;
    private void Start()
    {
        originalTransform = this.gameObject.transform;
    }
    //private void Update()
    //{
    //    Vector3 temp = originalTransform.transform.position;
    //    transform.position = new Vector3(this.transform.position.x, temp.y, this.transform.position.z);
    //}
    void OnCollisionEnter(Collision collision)
    {

        print("did we try physics removal: ->>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");

        if (interactableTags.Contains(collision.gameObject.tag))
        {
            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
        }
     
    }
    private void OnTriggerEnter(Collider other)
    {

        print("did we try physics removal [2] ****: ->>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");

        if (interactableTags.Contains(other.gameObject.tag))
        {
            Physics.IgnoreCollision(other.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
        }

    }




}