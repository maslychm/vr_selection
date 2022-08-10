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
        float delay = Random.Range(2, 4);
        Invoke(nameof(DestroyParentObject), delay);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (gameObject.tag.Contains(collision.collider.tag))
        {
            DestroyParentObject();
            return;
        }

        if (collision.collider.CompareTag("Player"))
        {
            DestroyParentObject();
            return;
        }
    }
}