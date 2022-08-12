using UnityEngine;

public class Object_collected : MonoBehaviour
{
    private Vector3 homePos;

    private Vector3 dumpsterLocation = new Vector3(100, 100, 100);
    private new Rigidbody rigidbody;
    private Quaternion originalRotaion;
    private Vector3 originalScale;
    private Transform originalParent;

    private void Start()
    {
        homePos = transform.position;
        originalRotaion = transform.rotation;
        rigidbody = GetComponent<Rigidbody>();
        originalScale = new Vector3(0.2f, 0.2f, 0.2f);
        originalParent = transform.parent;
    }

    //Reset to original position
    public void ResetGameObject()
    {
        transform.position = homePos;
        transform.rotation = originalRotaion;
        rigidbody.velocity = Vector3.zero;
        transform.localScale = originalScale;

        transform.SetParent(originalParent);
    }

    public void ResetOriginalScale()
    {
        transform.localScale = originalScale;
    }

    public void MoveOutsideReach()
    {
        transform.position = dumpsterLocation;
    }
}