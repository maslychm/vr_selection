using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class DestroyParentOnPlayerHit : MonoBehaviour
{
    private void DestroyParentObject()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }

    public void EndGameDeathAfterDelay()
    {
        //print("want to die");
        float delay = Random.Range(2, 4);
        Invoke(nameof(DestroyParentObject), delay);
    }

    public void OnTriggerEnter(Collider other)
    {
        //print($"{tag} hit by {other.tag}");

        if (gameObject.tag.Contains(other.tag))
        {
            DestroyParentObject();
            return;
        }

        if (other.CompareTag("Player"))
        {
            DestroyParentObject();
            return;
        }
    }
}