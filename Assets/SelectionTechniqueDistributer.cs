using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// assign the right selection technique in real time upon click of button 
/// G -> Grid
/// LS -> LenSelect
/// FC -> Flower Cone
/// MM -> MiniMap
/// </summary>

public class SelectionTechniqueDistributer : MonoBehaviour
{

    [SerializeField] public  bool MiniMapTrigger = false; 
    [SerializeField] public  bool GridTrigger = false; 
    [SerializeField] public  bool LenSelectTrigger = false; 
    [SerializeField] public  bool FlowerConeTrigger = false;

    public static bool wasDuplicated = false;

    Transform trans;
    int count = 0;

    public static GameObject currentlySetActiveTechnique;

    string FC, MM, LS, G;

    public static Dictionary<string, bool> SelectionTechniqueTriggers;
    // Start is called before the first frame update
    void Start()
    {

        MiniMapTrigger = false;
        GridTrigger = false;
        LenSelectTrigger = false;
        FlowerConeTrigger = false;

        FC = "FlowerCone";
        G = "Grid";
        LS = "LenSelect";
        MM = "MiniMap";

        SelectionTechniqueTriggers = new Dictionary<string, bool>();

        trans = this.transform;
        // just check if we actually have a selection technique manager or if an issue happened
        if (this.gameObject == null)
        {
            print("Couldn't find the selection technique assigner.... -> error issue from XROrigin/LeftHandController");
            return;
        }

        // make sure that every technique trigger is set to false 
        {
            SelectionTechniqueTriggers.Add(MM, false);
            SelectionTechniqueTriggers.Add(G, false);
            SelectionTechniqueTriggers.Add(FC, false);
            SelectionTechniqueTriggers.Add(LS, false);
        }

        // at the start go over all the children and set them to false 
        // this should turn off all the selection techniques present in the left hand 
        for (int i  = 0; i < this.gameObject.transform.childCount; i++ )
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        currentlySetActiveTechnique = null;
    }

    // Update is called once per frame
    void Update()
    {
        //if (count == 0)
        //{
        //    if (Input.GetKeyDown(KeyCode.M))
        //        count = 1;
        //    else 
        //        return;
        //}
        if (Input.GetKeyDown(KeyCode.L))
        {
            SelectionTechniqueTriggers[LS] = true;
            SelectionTechniqueTriggers[FC] = false;
            SelectionTechniqueTriggers[G] = false;
            SelectionTechniqueTriggers[MM] = false;


            if(currentlySetActiveTechnique != null)
                currentlySetActiveTechnique.SetActive(false);

            Transform childTrans = trans.Find(LS);
            if (childTrans != null)
            {

                print("We assigned LenSelect Technique to the user");
                childTrans.gameObject.SetActive(true);
                currentlySetActiveTechnique = childTrans.gameObject;
            }
            else
            {
                print("We couldn't find LenSelect Assigned Child ->Error in Left Hand Selection Technique Manager");
            }
        }

        else if (Input.GetKeyDown(KeyCode.M))
        {
            SelectionTechniqueTriggers[LS] = false;
            SelectionTechniqueTriggers[FC] = false;
            SelectionTechniqueTriggers[G] = false;
            SelectionTechniqueTriggers[MM] = true;

            if (currentlySetActiveTechnique != null)
                currentlySetActiveTechnique.SetActive(false);

            Transform childTrans = trans.Find(MM);
            if (childTrans != null)
            {

                print("We assigned MiniMap Technique to the user");
                childTrans.gameObject.SetActive(true);
                currentlySetActiveTechnique = childTrans.gameObject;
            }
            else
            {
                print("We couldn't find MiniMap Selection Assigned Child ->Error in Left Hand Selection Technique Manager");
            }
        }

        else if (Input.GetKeyDown(KeyCode.G))
        {
            SelectionTechniqueTriggers[LS] = false;
            SelectionTechniqueTriggers[FC] = false;
            SelectionTechniqueTriggers[G] = true;
            SelectionTechniqueTriggers[MM] = false;

            if (currentlySetActiveTechnique != null)
                currentlySetActiveTechnique.SetActive(false);

            Transform childTrans = trans.Find(G);
            if (childTrans != null)
            {

                print("We assigned Grid Selection Technique to the user");
                childTrans.gameObject.SetActive(true);
                currentlySetActiveTechnique = childTrans.gameObject;
            }
            else
            {
                print("We couldn't find Grid selection Assigned Child ->Error in Left Hand Selection Technique Manager");
            }
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            SelectionTechniqueTriggers[LS] = false;
            SelectionTechniqueTriggers[FC] = true;
            SelectionTechniqueTriggers[G] = false;
            SelectionTechniqueTriggers[MM] = false;

            if (currentlySetActiveTechnique != null)
                currentlySetActiveTechnique.SetActive(false);

            Transform childTrans = trans.Find(FC);
            if (childTrans != null)
            {

                print("We assigned Flower Cone Technique to the user");
                childTrans.gameObject.SetActive(true);
                currentlySetActiveTechnique = childTrans.gameObject;
            }
            else
            {
                print("We couldn't find Flower Cone Technique Assigned Child ->Error in Left Hand Selection Technique Manager");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {

            SelectionTechniqueTriggers[LS] = false;
            SelectionTechniqueTriggers[FC] = false;
            SelectionTechniqueTriggers[G] = false;
            SelectionTechniqueTriggers[MM] = false;

            // simply turn off the ongoing selection technique if there is any 
            if (currentlySetActiveTechnique != null)
                currentlySetActiveTechnique.SetActive(false);
            currentlySetActiveTechnique = null;
            print("End of the experiment, thank you, back to starting position :D");

        }

        //else
        //{
        //    print("This Key has no matching behaviour -- ERROR -- chose either F, L, M, G or esc");
        //}
        
    }
}
