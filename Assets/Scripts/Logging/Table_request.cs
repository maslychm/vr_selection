using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table_request : MonoBehaviour
{
    List<GameObject> objects = new List<GameObject>(); //Objects in the space.
    List<Renderer> renderers = new List<Renderer>();
    Renderer renderer; //Renderer
    Mesh _mesh; 
    GameObject curr_object; 
    List<string> tags = new List<string>(){ "Cube", "Sphere", "Star", "Pyramid", "Cylinder"};

    Logging_XR logger;

    int objects_collected; 
    void AddObjectsToList(string name)
    { 
        // Debug.Log(name);
        GameObject[] items = GameObject.FindGameObjectsWithTag(name);
        // string[4] tomato = new string[] {6,5,5,5};
        // Debug.Log(items.Length);

        // objects.AddRange(items);

        for (int i = 0; i < items.Length; i++)
        { 
            objects.Add(items[i]);
        }
        // if (items.Length > 0)
        //     renderers.Add(items[0].GetComponent<Renderer>());
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
        StartExperiment();   
        _mesh = GetComponent<MeshFilter>().mesh;
        logger = GameObject.Find("XR_Logging_Obj").GetComponent<Logging_XR>();
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
        objects_collected = 0;
        SetOwnRenderer(curr_object);
        logger.ResetStatistics();
    }

    void CheckExperiment()
    { 
        Debug.Log("Amount of world objects " + objects.Count + " Collected items: " +objects_collected );
        if(objects.Count != objects_collected)
        { 
            curr_object = GetObjectGetRandomGameObject();
            SetOwnRenderer(curr_object);
        }
        else
        { 
            logger.SaveToFile();
            Debug.Log("Experiment Over! Statistics collected and saved.");
        }
    }


    GameObject GetObjectGetRandomGameObject()
    { 
      return objects[UnityEngine.Random.Range(0,objects.Count)];
    }


//Sets the renderer of the object presented to the user. 
    void SetOwnRenderer(GameObject g)
    {
        GetComponent<MeshFilter>().mesh = g.GetComponent<MeshFilter>().mesh;
        GetComponent<Renderer>().material = g.GetComponent<Renderer>().material;
    }


    void OnTriggerEnter(Collider collider)
    { 
        Debug.Log(collider.tag);
        //If we find the tag, just look it up in the array.
        if (tags.Contains(collider.tag) && curr_object.tag == collider.tag)
        {
            objects_collected++;
            CheckExperiment();
            collider.gameObject.GetComponent<Object_collected>().StopCountdownAndFreeze();
        }
        else{
            //Send the gameobject back to the map. 
            collider.gameObject.GetComponent<Object_collected>().ResetGameObject();
        }
    }
}
