using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table_request : MonoBehaviour
{
    List<GameObject> objects; //Objects in the space.
    List<Renderer> renderers;
    Renderer renderer; //Renderer
    GameObject curr_object; 
    List<string> tags = new List<string>(){ "Cube", "Sphere", "Star", "Pyramid", "Cylinder"};

    void AddObjectsToList(string name)
    { 
        GameObject[] items = GameObject.FindGameObjectsWithTag(name);
        objects.AddRange(items);
        renderers.Add(items[0].GetComponent<Renderer>());
    }


    public List<T> Shuffle<T>(List<T> list)  
    {  
        int n = list.Count;  
        while (n > 1) {  
            n--;  
            int k = UnityEngine.Random.Range(0,n + 1);  
            T value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }  
        return list; 
    }
    void Start()
    { 
        renderer = GetComponent<Renderer>();
        StartExperiment();   
    }


    public void StartExperiment()
    { 
        AddObjectsToList("Cube");
        AddObjectsToList("Sphere");
        AddObjectsToList("Star");
        AddObjectsToList("Pyramid");
        AddObjectsToList("Cylinder");
        objects = Shuffle(objects); //Creates a different ordering to collect objects.
        curr_object = GetObjectGetRandomGameObject();
        //Reset all objects and their positions.
        for(int i = 0; i < objects.Count; i++)
        { 
            objects[i].GetComponent<Object_collected>().ResetGameObject(); 
        }
    }

    void CheckExperiment()
    { 
        if(objects.Count > 0)
        { 
            curr_object = GetObjectGetRandomGameObject();
        }
        else
        { 
            Debug.Log("Experiment Over! All objects collected.");
        }
    }


    GameObject GetObjectGetRandomGameObject()
    { 
      return objects[UnityEngine.Random.Range(0,objects.Count)];
    }


//Sets the renderer of the object presented to the user. 
    void SetOwnRenderer(GameObject g)
    {
        switch(g.tag)
        {
            case "Cube":
                renderer = renderers[0];
                break;
            case "Sphere":
                renderer = renderers[1];
                break;
            case "Star":
                renderer = renderers[2];
                break;
            case "Pyramid":
                renderer = renderers[3];
                break;
            case "Cylinder":
                renderer = renderers[4];
                break;
        }
    }


    void OnTriggerEnter(Collider collider)
    { 
        //If we find the tag, just look it up in the array.
        if (tags.Contains(collider.tag))
        {
            CheckExperiment();
        }
    }
}
