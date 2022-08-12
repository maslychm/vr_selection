using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private int densityLevel = 0;
    public static readonly List<int> densityLevelIntegers = new List<int> { 1, 2, 3 };

    [SerializeField] private List<GameObject> densityLevels;
    [SerializeField] private SelectionTechniqueManager techniqueDistributer;

    public GrabbingHand grabbingHand;

    private void Start()
    {
        DisableAllLevels();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            densityLevel = (densityLevel + 1) % 4;
            EnableDensityLevel(densityLevel);
        }
    }

    public void EnableDensityLevel(int lvl)
    {
        techniqueDistributer.DisableAllTechniques();
        ResetSceneInteractables();

        if (lvl == 0)
        {
            DisableAllLevels();
            return;
        }

        if (!densityLevelIntegers.Contains(lvl))
        {
            throw new System.Exception($"Only density levels of {densityLevelIntegers}");
        }

        for (int i = 0; i < lvl; i++)
        {
            densityLevels[i].SetActive(true);
        }
    }

    private void DisableAllLevels()
    {
        densityLevels.ForEach(x => x.SetActive(false));
    }

    private void ResetSceneInteractables()
    {
        List<Interactable> toFlush = grabbingHand.GetListOfToBeFlushedItems();
        toFlush.ForEach(x => x.GetComponent<Object_collected>().ResetGameObject());
        toFlush.Clear();
    }
}