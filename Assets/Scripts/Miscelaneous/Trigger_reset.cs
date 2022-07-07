using System.Collections.Generic;
using UnityEngine;

public class Trigger_reset : MonoBehaviour
{
    private List<string> interactableTags = new List<string>() { "cube", "sphere", "star", "pyramid", "cylinder", "infinity" };

    public void OnTriggerEnter(Collider collider)
    {
        if (interactableTags.Contains(collider.tag))
        {
            if (collider.name.Contains("subject208"))
                return;
            // in case for some reason it gets deleted from the original object
            // add the component and then reset the object then return
         /*   if (collider.gameObject.GetComponent<Object_collected>() == null)
            {
                collider.gameObject.AddComponent<Object_collected>();
                collider.gameObject.GetComponent<Object_collected>().ResetGameObject();
                return;
            }*/

            // if the component exists then in that case simply reset the game object
            collider.gameObject.GetComponent<Object_collected>().ResetGameObject();
        }
    }
}