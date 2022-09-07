using UnityEngine;
using UnityEngine.SceneManagement;

public class Object_collected : MonoBehaviour
{
    private Vector3 dumpsterLocation = new Vector3(100, 100, 100);
    private new Rigidbody rigidbody;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;
    private Transform originalParent;
    private Scene scene;

    private void Start()
    {
        scene = SceneManager.GetActiveScene();
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        rigidbody = GetComponent<Rigidbody>();
        originalScale = new Vector3(0.2f, 0.2f, 0.2f);
        originalParent = transform.parent;

        // if thje current scene has a search task then in that case we don't want to use an outline

        if (this.gameObject.name.Contains("arget"))
        {
            if (scene.name.Contains("SearchTask"))
            {
                if (this.gameObject.GetComponent<cakeslice.Outline>())
                {
                    this.gameObject.GetComponent<cakeslice.Outline>().enabled = false;
                }
            }

            // -- backUp Logic

            // just in case it is not existing then we add it
            //else
            //{
            //    if (!this.gameObject.GetComponent<cakeslice.Outline>())
            //    {
            //        this.gameObject.AddComponent<cakeslice.Outline>();
            //    }
            //}
        }
    }

    private void Update()
    {
        // if thje current scene has a search task then in that case we don't want to use an outline
        if (this.gameObject.name.Contains("arget"))
            if (scene.name.Contains("SearchTask"))
            {
                if (this.gameObject.GetComponent<cakeslice.Outline>())
                {
                    this.gameObject.GetComponent<cakeslice.Outline>().enabled = false;
                }
            }
    }

    //Reset to original position
    public void ResetGameObject()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
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