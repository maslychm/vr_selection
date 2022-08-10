using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private int densityLevel = 0;

    [SerializeField] private GameObject densityLevel1, densityLevel2, densityLevel3;
    [SerializeField] private SelectionTechniqueDistributer techniqueDistributer;

    private void Start()
    {
        DisableAllLevels();
    }

    private void Update()
    {
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
                densityLevel1.SetActive(true);
            }
            else if (densityLevel == 2)
            {
                densityLevel2.SetActive(true);
            }
            else if (densityLevel == 3)
            {
                densityLevel3.SetActive(true);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Q))
            Application.Quit();
    }

    private void DisableAllLevels()
    {
        densityLevel1.SetActive(false);
        densityLevel2.SetActive(false);
        densityLevel3.SetActive(false);

        //System.Collections.Generic.List<Object_collected> ResetAllObjectsBackToOriginal = FindObjectsOfType<Object_collected>().To;


    }
}