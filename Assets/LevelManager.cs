using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private int densityLevel = 0;

    [SerializeField] private GameObject densityLevel1, densityLevel2, densityLevel3;
    [SerializeField] private SelectionTechniqueDistributer techniqueDistributer;

    public GrabbingHand instanceOfRightHand;

    private List<GameObject> helperListForFlush = new List<GameObject>(); 

    private void Start()
    {
        DisableAllLevels();
        helperListForFlush = new List<GameObject>();
    }

    private void Update()
    {

        helperListForFlush = instanceOfRightHand.GetListOfToBeFlushedItems();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            densityLevel = (densityLevel + 1) % 4;
            techniqueDistributer.DisableAllTechniques();

            if (densityLevel == 0)
            {
                
                DisableAllLevels();
            }
            else if (densityLevel == 1)
            {
                flushThisNow();
                densityLevel1.SetActive(true);
            }
            else if (densityLevel == 2)
            {
                flushThisNow();
                densityLevel2.SetActive(true);
            }
            else if (densityLevel == 3)
            {
                flushThisNow();
                densityLevel3.SetActive(true);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Q))
            Application.Quit();
    }

    private void DisableAllLevels()
    {
        

        flushThisNow();
        //GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        //foreach (object go in allObjects)
        //    if (go.activeInHierarchy)
                
        densityLevel1.SetActive(false);
        densityLevel2.SetActive(false);
        densityLevel3.SetActive(false);
    }

    private void flushThisNow()
    {
        if (helperListForFlush.Count == 0)
            return;
        else
        {
            helperListForFlush.ForEach(x => x.GetComponent<Object_collected>().ResetGameObject());
            helperListForFlush.ForEach(x => x.gameObject.SetActive(false));
        }
        helperListForFlush.Clear();
        GrabbingHand.flushOrNo = true;

    }
}