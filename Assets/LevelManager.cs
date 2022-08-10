using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private int counter = 0;

    public MiniMapInteractor miniMapInteractorMM;
    public MiniMapInteractor miniMapInteractorWO;

    // Start is called before the first frame update
    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (counter == 3)
            {
                counter = 0;
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
            }

            transform.GetChild(counter).gameObject.SetActive(true);

            if (counter == 1)
                transform.GetChild(counter - 1).gameObject.SetActive(true);

            else if (counter == 2)
            {
                transform.GetChild(counter - 1).gameObject.SetActive(true);
                transform.GetChild(counter - 2).gameObject.SetActive(true);
            }
            if (SelectionTechniqueDistributer.currentlySetActiveTechnique != null)
            {
                if (SelectionTechniqueDistributer.currentlySetActiveTechnique.name == "MiniMap")
                    miniMapInteractorMM.CreateDuplicatesForMiniMap();
                else if (SelectionTechniqueDistributer.currentlySetActiveTechnique.name == "MiniMapWithoutExpansion")
                    miniMapInteractorWO.CreateDuplicatesForMiniMap();
            }

            counter++;
        }
        else if (Input.GetKeyDown(KeyCode.Q))
            Application.Quit();
    }
}