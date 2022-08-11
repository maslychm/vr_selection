using UnityEngine;

public class HighlightObject : MonoBehaviour
{
    public bool isTriggered = false;
    public float count = 2.0f;
    public GameObject current;

    private void Start()
    {
        isTriggered = false;
    }

    private void Update()
    {
        if (isTriggered == true)
        {
            if (count == 0)
            {
                turnOffShader();
                count = 2.0f;
            }
        }
    }

    private void turnOffShader()
    {
        if (current != null)
        {
            current.GetComponent<cakeslice.Outline>().enabled = false;
        }
    }
}