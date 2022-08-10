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
    //[SerializeField] public bool MiniMapTrigger = false;
    //[SerializeField] public bool FlowerConeTrigger = false;
    //[SerializeField] public bool MiniMapWithoutExpansion = false;

    //public static bool wasDuplicated = false;

    [SerializeField] private GrabbingHand hand;

    private Transform trans;
    //int count = 0;

    public static GameObject currentlySetActiveTechnique = null;

    private string FC, MM, LS, G, MwE;

    public MiniMapInteractor miniMapInteractorMM, miniMapInteractorWO;

    public static Dictionary<string, bool> SelectionTechniqueTriggers;

    // Start is called before the first frame update
    private void Start()
    {
        //MiniMapTrigger = false;
        //FlowerConeTrigger = false;
        //MiniMapWithoutExpansion = false;

        FC = "FlowerCone";
        G = "Grid";
        LS = "LenSelect";
        MM = "MiniMap";
        MwE = "MiniMapWithoutExpansion";

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
            SelectionTechniqueTriggers.Add(MwE, false);
        }

        // at the start go over all the children and set them to false
        // this should turn off all the selection techniques present in the left hand
        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        currentlySetActiveTechnique = null;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            SelectionTechniqueTriggers[LS] = false;
            SelectionTechniqueTriggers[FC] = false;
            SelectionTechniqueTriggers[G] = false;
            SelectionTechniqueTriggers[MM] = false;
            SelectionTechniqueTriggers[MwE] = true;

            if (currentlySetActiveTechnique != null)
                currentlySetActiveTechnique.SetActive(false);

            Transform childTrans = trans.Find(MwE);
            if (childTrans != null)
            {
                print("We assigned MiniMap Without Expansion Technique to the user");
                childTrans.gameObject.SetActive(true);
                currentlySetActiveTechnique = childTrans.gameObject;
                hand.miniMap = miniMapInteractorWO.miniMap;
                ClutterHandler_circumferenceDisplay.miniMapInteractor = miniMapInteractorWO;
                ClutterHandler_circumferenceDisplay.runCircumference = false;
                miniMapInteractorWO.CreateDuplicatesForMiniMap();
            }
            else
            {
                print("We couldn't find MiniMap withotu expansion Selection Assigned Child ->Error in Left Hand Selection Technique Manager");
            }
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            SelectionTechniqueTriggers[LS] = false;
            SelectionTechniqueTriggers[FC] = false;
            SelectionTechniqueTriggers[G] = false;
            SelectionTechniqueTriggers[MM] = true;
            SelectionTechniqueTriggers[MwE] = false;

            if (currentlySetActiveTechnique != null)
                currentlySetActiveTechnique.SetActive(false);

            Transform childTrans = trans.Find(MM);
            if (childTrans != null)
            {
                print("We assigned MiniMap Technique to the user");

                childTrans.gameObject.SetActive(true);
                currentlySetActiveTechnique = childTrans.gameObject;
                hand.miniMap = miniMapInteractorMM.miniMap;
                ClutterHandler_circumferenceDisplay.miniMapInteractor = miniMapInteractorMM;
                ClutterHandler_circumferenceDisplay.runCircumference = true;
                miniMapInteractorMM.CreateDuplicatesForMiniMap();
            }
            else
            {
                print("We couldn't find MiniMap Selection Assigned Child ->Error in Left Hand Selection Technique Manager");
            }
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            SelectionTechniqueTriggers[LS] = false;
            SelectionTechniqueTriggers[FC] = true;
            SelectionTechniqueTriggers[G] = false;
            SelectionTechniqueTriggers[MM] = false;
            SelectionTechniqueTriggers[MwE] = false;

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
            SelectionTechniqueTriggers[MwE] = false;

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