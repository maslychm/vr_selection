using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rayAdjuster : MonoBehaviour
{

    public GameObject sphere;
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.transform.position = new Vector3(sphere.transform.position.x, sphere.transform.position.y, sphere.transform.position.z + 0.05f);
    }

    // Update is called once per frame
    void Update()
    {
       this.gameObject.transform.position =  new Vector3(sphere.transform.position.x, sphere.transform.position.y , sphere.transform.position.z + 0.05f);
    }
}
