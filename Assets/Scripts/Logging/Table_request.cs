using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Table_request : MonoBehaviour
{
    public List<GameObject> objects = new List<GameObject>(); //Objects in the space.
    private List<Renderer> renderers = new List<Renderer>();
    private Renderer renderer; //Renderer
    private Mesh _mesh;
    public GameObject expectedObject;
    private GameObject expectedObjectCopy;
    private List<string> interactableTags = new List<string>() { "cube", "sphere", "star", "pyramid", "cylinder", "infinity" };

    private Logging_XR logger;

    private int objects_collected;

    private void AddTaggesObjectsToList(string name)
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag(name);
        items.ToList().ForEach(item => objects.Add(item));
    }

    public List<T> Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }

    private void Start()
    {
        _mesh = GetComponent<MeshFilter>().mesh;
        logger = GameObject.Find("XR_Logging_Obj").GetComponent<Logging_XR>();
        StartExperiment();
    }

    public void StartExperiment()
    {
        interactableTags.ForEach(item => AddTaggesObjectsToList(item));

        objects = Shuffle(objects); //Creates a different ordering to collect objects.

        //Reset all objects and their positions.
        foreach (var o in objects)
            o.GetComponent<Object_collected>().ResetGameObject();

        objects_collected = 0;
        expectedObject = objects[objects_collected]; //SelectRandomExpectedObject();
        SetOwnRenderer(expectedObject);
        logger.ResetStatistics();
    }

    private void CheckExperiment()
    {
        Debug.Log("Num. of world objects " + objects.Count + " Collected items: " + objects_collected);

        if (objects.Count != objects_collected)
        {
            expectedObject = objects[objects_collected]; // SelectRandomExpectedObject();
            SetOwnRenderer(expectedObject);
        }
        else
        {
            SetOwnRenderer(null);
            logger.SaveToFile();
            Debug.Log("Experiment Over! Statistics collected and saved.");
        }
    }

    private GameObject SelectRandomExpectedObject()
    {
        return objects[UnityEngine.Random.Range(0, objects.Count)];
    }

    //Sets the renderer of the object presented to the user.
    private void SetOwnRenderer(GameObject g)
    {
        if (expectedObjectCopy != null)
            Destroy(expectedObjectCopy);

        if (g == null)
            return;

        expectedObjectCopy = Instantiate(g, transform.position, transform.rotation);
        expectedObjectCopy.tag = "Untagged";

        Destroy(expectedObjectCopy.GetComponent<Object_collected>());
        Destroy(expectedObjectCopy.GetComponent<XRGestureInteractable>());
    }

    private void OnTriggerEnter(Collider collider)
    {
        //Debug.Log($"Table triggered by: {collider.tag}");
        //Debug.Log(interactableTags.Contains(collider.tag));
        //Debug.Log(expectedObject.CompareTag(collider.tag));

        // If tag is among possible and is expected for selection - record success
        if (interactableTags.Contains(collider.tag) && expectedObject.CompareTag(collider.tag))
        {
            //Debug.Log("Collected Object");
            objects_collected++;
            CheckExperiment();
            collider.gameObject.GetComponent<Object_collected>().StopCountdownAndFreeze();
        }
        else
        {
            //Send the gameobject back to the map.
            collider.gameObject.GetComponent<Object_collected>().ResetGameObject();
        }
    }
}