using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    int counter = 0;
    int prior = -1;
    public GameObject ControllerHand;
    public MiniMapInteractor miniMapInteractor;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (counter == 3)
                counter = 0;
            if (prior != -1)
                this.transform.GetChild(prior).gameObject.SetActive(false);

            this.transform.GetChild(counter).gameObject.SetActive(true);
            miniMapInteractor.helperForLevelsUpdate();
            counter++;
            prior = counter - 1;
        }
        else if (Input.GetKeyDown(KeyCode.Q))
            Application.Quit();
        
    }
}
