using UnityEngine;

public class Object_collected : MonoBehaviour
{
    //private Transform _transform;
    private Vector3 _home_pos;

    private Vector3 _dumpster_location = new Vector3(12, 1, -3);
    private Quaternion _home_rot;
    private Rigidbody _rigidbody;
    public Vector3 originalScale;

    private void Start()
    {
        _home_pos = transform.position;
        _home_rot = transform.rotation;
        _rigidbody = GetComponent<Rigidbody>();
        originalScale = new Vector3(0.2f, 0.2f, 0.2f);

        this.gameObject.AddComponent<cakeslice.Outline>();

        this.gameObject.GetComponent<cakeslice.Outline>().eraseRenderer = true;
    }


     // use thisb for p[osition reset 
    //Reset to original position
    public void ResetGameObject()
    {
        transform.position = _home_pos;
        transform.rotation = _home_rot;
        _rigidbody.velocity = Vector3.zero;
        transform.localScale = originalScale;
    }


    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.name == "RightHand Controller")
            this.gameObject.GetComponent<cakeslice.Outline>().eraseRenderer = true;
    }
    public void MoveOutsideReach()
    {
        transform.position = _dumpster_location + new Vector3(
            2 * UnityEngine.Random.Range(0, 10) * 0.1f, 0,
            2 * UnityEngine.Random.Range(0, 1) * 0.1f);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.name == "RightHand Controller")
            this.gameObject.GetComponent<cakeslice.Outline>().eraseRenderer = false;
        switch (collider.tag)
        {
            case "end_manipulator":
                break;

            case "failure":
                transform.position = _home_pos;
                transform.rotation = _home_rot;
                break;
        }
    }
}