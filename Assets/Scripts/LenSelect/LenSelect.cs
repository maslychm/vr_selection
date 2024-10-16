﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// -> Implements lenselect based on Linear Scaling too
/// </summary>
public class LenSelect : MonoBehaviour
{

    // store alll the highlighrted objects
    private List<GameObject> allHighlightedObjects;

    // helper for objects that are not highlighted 
    private List<GameObject> ObjectsToBeSetBackToOriginalSize;

    // hold original parameters
    private Dictionary<GameObject, Vector3> directionReference;
    private Dictionary<GameObject, Vector3> transformPointReference;
    private Dictionary<GameObject, Vector3> originalScale;

    // launch linear scaling instead
    [SerializeField] public bool LinearScaling = true;

    // instantiate from the lenseselector
    public LenSelectInteractor LenselectInteractor;

    // main components discussed in the study 
    private float openingAngle = 15 / 2.0f;
    private float ConeRadius;

    // these should be assigned manually 
    public GameObject FlashLightCentreCone_Ray;
    public GameObject FlashLightBigCone;

    // total scalling factor 
    private Vector3 S;

    // maximum scale
    private Vector3 Ss;

    // user defined maximum scaling factor 
    [SerializeField]public Vector3 Sm;

    // n is a user defined value, that describes the distance at which the object keeps its original scale.
    [SerializeField]public Vector3 n; // maybe no need to worry about the frustum if we can just check that the distance (transform point is bigger than n)

    public Vector3 startingScale;
    public Vector3 endingScale;

    public static float ConeRadius_v2;


    // list to store the original transforms combined 
    private Dictionary<GameObject, Vector3> OriginalTransform;
    private Dictionary<GameObject, float> magnitudes;

    private Dictionary<GameObject, Transform> holdTheYOriginalValue;

    // dn is representing the normaliwed distance based on the equation of 
    // distance from the object to the ray / radius of the cone
    private float dn;
    private void Start()
    {
        allHighlightedObjects = new List<GameObject>();
        ObjectsToBeSetBackToOriginalSize = new List<GameObject>();
        directionReference = new Dictionary<GameObject, Vector3>();
        originalScale = new Dictionary<GameObject, Vector3>();
        magnitudes = new Dictionary<GameObject, float>();
        transformPointReference = new Dictionary<GameObject, Vector3>();
        OriginalTransform = new Dictionary<GameObject, Vector3>();
        holdTheYOriginalValue = new Dictionary<GameObject, Transform>();

        n = new Vector3(1, 1, 1);
    }
    // update function that should give the list of the removed items from the cone
    private void Update()
    {
        LinearScaling = true;
        allHighlightedObjects = LenSelectInteractor.getAllHighlighted();
        ObjectsToBeSetBackToOriginalSize = LenSelectInteractor.getAllObjectsExitedTheCone();
    }

    public void OriginalPositionResetter()
    {
        if(ObjectsToBeSetBackToOriginalSize.Count > 0)
        {

            Vector3 originalScale;
            for (int i = 0; i < allHighlightedObjects.Count; i++)
            {

                if (ObjectsToBeSetBackToOriginalSize.Contains(allHighlightedObjects[i]))
                {
                    allHighlightedObjects.Remove(allHighlightedObjects[i]);
                }

            }

            for (int i = 0; i < ObjectsToBeSetBackToOriginalSize.Count; i++)
            {
                GameObject temp = ObjectsToBeSetBackToOriginalSize[i];


                if (OriginalTransform.ContainsKey(temp))
                {
                    originalScale = OriginalTransform[temp];
                    endingScale = originalScale;
                    startingScale = temp.transform.localScale;

                    if (temp.GetComponent<Rigidbody>().useGravity == true)
                    temp.GetComponent<Rigidbody>().AddForce(Physics.gravity * 5f, ForceMode.Acceleration);
                    temp.transform.localScale = originalScale;
                    temp.transform.position = holdTheYOriginalValue[temp].position;
                    temp.GetComponent<Rigidbody>().velocity = Vector3.zero;
                         
                }



            }

        }
    }

    /// <summary>        
    /// need to get fu -> the extent of the frustum at the distance of the object to the camera 
    /// need to get fn -> consider the ratio between the extents of the view frustum at the focus depth
    /// need to divide those 
    /// then multiply with 1 + 1/e
    /// store Ss</summary>
    public void getMaximumScale(Vector3 max)
    {
        // setting an alternative approach to set Ss
        float t1, t2, t3;
        t1 = max.x / n.x;
        t2 = max.y / n.y;
        t3 = max.z / n.z;
        Ss = new Vector3(t1, t2, t3);
    }

    public float getConeRadiusHelper(GameObject o)
    {
        getRadiusHelper(o);

        return ConeRadius_v2;
    }
    public void getRadiusHelper(GameObject o)
    {
        Vector3 temp;
        temp = directionReference[o];

        ConeRadius_v2 = temp.z * Mathf.Tan(openingAngle);

    }
    public void getConeRadius(GameObject o)
    {
        Vector3 temp;
        temp = directionReference[o];

        ConeRadius = temp.z  * Mathf.Tan(openingAngle);
    }
    public void  CalculateDistanceHelper()
    {

        Vector3 max = new Vector3(-1, -1, -1);

        foreach (GameObject o in allHighlightedObjects)
        {
            originalScale[o] = o.transform.localScale;
            var transformPointReference_var = transform.TransformPoint(o.transform.position);
            transformPointReference_var.z = 0f;

            // check for the position relative to n
            if (transformPointReference_var.magnitude > n.magnitude)
            {

                if (!transformPointReference.ContainsKey(o))
                    transformPointReference.Add(o, transformPointReference_var);
                else
                    transformPointReference[o] = transformPointReference_var;

                var objectPositionInFlashlightCoords = transform.InverseTransformPoint(o.transform.position);
                //objectPositionInFlashlightCoords.z = 0f;
                float t1 = objectPositionInFlashlightCoords.magnitude;
                if (!magnitudes.ContainsKey(o))
                    magnitudes.Add(o, t1);
                else
                    magnitudes[o] = t1;

                if (!directionReference.ContainsKey(o))
                    directionReference.Add(o, objectPositionInFlashlightCoords);
                else
                    directionReference[o] = objectPositionInFlashlightCoords;
            }
            
        }

    }

    public void automaticResizerInCone_LinearScaling()
    {
        CalculateDistanceHelper();
        for (int i = 0; i < allHighlightedObjects.Count; i++)
        {
            if (transformPointReference.ContainsKey(allHighlightedObjects[i]) && transformPointReference[allHighlightedObjects[i]].magnitude > n.magnitude)
            {

                // addd some code to stop the interaction between objects same as vidio

                // --------------------------------------------------------------------

                getConeRadius(allHighlightedObjects[i]);

                // normalize the distance based on the suggested normalization
                float t1;
                t1 = magnitudes[allHighlightedObjects[i]];
                //print("radius : " + ConeRadius + "magnitude t1 : " + t1);
                dn = t1 / ConeRadius;

                Vector3 dnV2Helper = (directionReference[allHighlightedObjects[i]]);
                // s, depends linearly on a user-defined maximum scaling factor sm>1.
                // Where 1−dn describes how much of sm is applied to the final scale.
                Sm = new Vector3(2, 2, 2);

                // without dividing by radius 
                float Sx1 = 1 + (1 - dnV2Helper.x) * (Sm.x - 1);
                float Sy1 = 1 + (1 - dnV2Helper.y) * (Sm.y - 1);
                float Sz1 = 1 + (1 - dnV2Helper.z) * (Sm.z - 1);

                float S_v1 = 1 + (1 - dn) * (2 - 1);

                //Vector3 S_V2 = new Vector3(Sx1, Sy1, Sz1);
                if (!OriginalTransform.ContainsKey(allHighlightedObjects[i]))
                    OriginalTransform.Add(allHighlightedObjects[i], allHighlightedObjects[i].transform.localScale);
                if (!holdTheYOriginalValue.ContainsKey(allHighlightedObjects[i]))
                    holdTheYOriginalValue.Add(allHighlightedObjects[i], allHighlightedObjects[i].transform);

                allHighlightedObjects[i].transform.localScale = OriginalTransform[allHighlightedObjects[i]] * S_v1;
                allHighlightedObjects[i].GetComponent<Rigidbody>().velocity = Vector3.zero;

            }

        }
    }
}

