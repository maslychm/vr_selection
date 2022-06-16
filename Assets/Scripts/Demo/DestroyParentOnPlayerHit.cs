using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class DestroyParentOnPlayerHit : MonoBehaviour
{
    private void DestroyParentObject()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        //print($"{tag} hit by {other.tag}");

        if (gameObject.tag.Contains(other.tag))
        {
            DestroyParentObject();
            return;
        }

        if (other.tag == "Player")
        {
            DestroyParentObject();
            return;
        }
    }
}