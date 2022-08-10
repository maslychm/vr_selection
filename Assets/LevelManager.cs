using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private int counter = 0;

    //int prior = -1;
    public GameObject ControllerHand;

    public MiniMapInteractor miniMapInteractorMM;
    public MiniMapInteractor miniMapInteractorWO;

    // Start is called before the first frame update
    private void Start()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).gameObject.SetActive(false);
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
                for (int i = 0; i < this.transform.childCount; i++)
                {
                    this.transform.GetChild(i).gameObject.SetActive(false);
                }
            }

            this.transform.GetChild(counter).gameObject.SetActive(true);

            if (counter == 1)
                this.transform.GetChild(counter - 1).gameObject.SetActive(true);
            else if (counter == 2)
            {
                this.transform.GetChild(counter - 1).gameObject.SetActive(true);
                this.transform.GetChild(counter - 2).gameObject.SetActive(true);
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