using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// -> Implements lenselect based on Combined Scaling approach mentioned in their paper
/// -> Implements lenselect based on Linear Scaling too (using a checkbox)
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


    // list to store the original transforms combined 
    private Dictionary<GameObject, Vector3> OriginalTransform;
    private Dictionary<GameObject, float> magnitudes;

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

        n = new Vector3(1, 1, 1);
    }
    // update function that should give the list of the removed items from the cone
    private void Update()
    {
        LinearScaling = true;
        allHighlightedObjects = LenSelectInteractor.getAllHighlighted();
        ObjectsToBeSetBackToOriginalSize = LenSelectInteractor.getAllObjectsExitedTheCone();
       if (LinearScaling == false)
            automaticResizerInCone();
        else
            automaticResizerInCone_LinearScaling();


        OriginalPositionResetter();
    }

    public void OriginalPositionResetter()
    {
        if(ObjectsToBeSetBackToOriginalSize.Count > 0)
        {
            Vector3 originalPosition;

            for(int i = 0; i < ObjectsToBeSetBackToOriginalSize.Count; i++)
            {
                GameObject temp = ObjectsToBeSetBackToOriginalSize[i];
                originalPosition = OriginalTransform[temp];
               
                temp.transform.position = originalPosition;

                temp.transform.localScale = originalPosition;
            }
        }
    }
    // create a function to resiwe our shapes as they are in the cone
    public void automaticResizerInCone()
    {

     /*   // this will get the distances of each object and store it 
        // distance from the object to the centre ray
        CalculateDistanceHelper();

        for(int i = 0; i<allHighlightedObjects.Count; i++)
        {
            if(transformPointReference[allHighlightedObjects[i]].magnitude < n.magnitude)
            {
                continue;
            }
            getConeRadius(allHighlightedObjects[i]);

            // normalize the distance based on the suggested normalization
            float t1, t2, t3;
           /* t1 = directionReference[allHighlightedObjects[i]].x / ConeRadius.x;
            t2 = directionReference[allHighlightedObjects[i]].y / ConeRadius.y;
            t3 = directionReference[allHighlightedObjects[i]].z / ConeRadius.z;
           */
         /*   dn = new Vector3(t1, t2, t3);

            float Sx = 1 + (1 - Mathf.Pow(dn.x, 1.0f / 4)) * (Ss.x - 1);
            float Sy = 1 + (1 - Mathf.Pow(dn.y, 1.0f / 4)) * (Ss.y - 1);
            float Sz = 1 + (1 - Mathf.Pow(dn.z, 1.0f / 4)) * (Ss.z - 1);

            // after updating this we store the values in the appropriate vector3
            S = new Vector3(Sx, Sy, Sz);

            OriginalTransform.Add(allHighlightedObjects[i], allHighlightedObjects[i].transform);

            allHighlightedObjects[i].transform.localScale = S;
        }*/

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
    public void getConeRadius(GameObject o)
    {
        Vector3 temp, helper_1, helper_2, d, u_squared;
        temp = directionReference[o];
        /*helper_1 = Vector3.Scale(temp, temp);
        d = directionReference[o];
        helper_2 = Vector3.Scale(d, d);
        u_squared = helper_1 - helper_2;

        Vector3 u = new Vector3(Mathf.Sqrt(u_squared.x), Mathf.Sqrt(u_squared.y), Mathf.Sqrt(u_squared.z));*/

        ConeRadius = temp.z  * Mathf.Tan(openingAngle);
    }
    public void getConeRadiusv2(GameObject o)
    {

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

                Vector3 temp = objectPositionInFlashlightCoords;
                max = Vector3.Max(max, temp);

                if (!directionReference.ContainsKey(o))
                    directionReference.Add(o, objectPositionInFlashlightCoords);
                else
                    directionReference[o] = objectPositionInFlashlightCoords;
            }
            
        }
        getMaximumScale(max);
    }

    public void automaticResizerInCone_LinearScaling()
    {
        CalculateDistanceHelper();
        for (int i = 0; i < allHighlightedObjects.Count; i++)
        {
           // if (transformPointReference[allHighlightedObjects[i]].magnitude < n.magnitude)
           // {
            //    continue;
          //  }
            getConeRadius(allHighlightedObjects[i]);
            //getConeRadiusv2(allHighlightedObjects[i]);

            // normalize the distance based on the suggested normalization
            float t1, t2, t3;
            t1 = magnitudes[allHighlightedObjects[i]];
            print("radius : " + ConeRadius + "magnitude t1 : " + t1);
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


            //  float Sx = 1 + (1 - dn.x) * (Sm.x - 1);
            //   float Sy = 1 + (1 - dn.y) * (Sm.y - 1);
            // float Sz = 1 + (1 - dn.z) * (Sm.z - 1);

            // after updating this we store the values in the appropriate vector3
            //S = new Vector3(Sx, Sy, Sz);

            //Vector3 S_V2 = new Vector3(Sx1, Sy1, Sz1);
            if (!OriginalTransform.ContainsKey(allHighlightedObjects[i]))
                OriginalTransform.Add(allHighlightedObjects[i], allHighlightedObjects[i].transform.localScale);
           // else
             //   OriginalTransform[allHighlightedObjects[i]] = allHighlightedObjects[i].transform.localScale;

            //allHighlightedObjects[i].transform.localScale = S;
            print("gameObject: " + allHighlightedObjects[i] + " localScale [original] : " + OriginalTransform[allHighlightedObjects[i]]);
            print("gameObject: " + allHighlightedObjects[i] + " localScale [updated] : " + OriginalTransform[allHighlightedObjects[i]] * S_v1);
            allHighlightedObjects[i].transform.localScale = OriginalTransform[allHighlightedObjects[i]] * S_v1 ;
            


        }
    }
}
