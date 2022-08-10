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

    void OnCollisionEnter(Collision collision)
    {

       
        if (interactableTags.Contains(collision.gameObject.tag))
        {
            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
        }
     
    }
    private void OnTriggerEnter(Collider other)
    {

       
        if (interactableTags.Contains(other.gameObject.tag))
        {
            Physics.IgnoreCollision(other.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
        }

    }




}