using System.Collections.Generic;
using UnityEngine;

public class Trigger_reset : MonoBehaviour
{
    private List<string> interactableTags = new List<string>() { "cube", "sphere", "star", "pyramid", "cylinder", "infinity" };

    public void OnTriggerEnter(Collider collider)
    {
        if (interactableTags.Contains(collider.tag) && !collider.name.Contains("subject208"))
        {
            // if the component exists then in that case simply reset the game object
            collider.gameObject.GetComponent<Object_collected>().ResetGameObject();
        }
    }
}