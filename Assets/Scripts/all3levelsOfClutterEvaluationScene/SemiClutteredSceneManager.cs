using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manage the scene especially the display and layout of the items
/// </summary>
public class SemiClutteredSceneManager : MonoBehaviour
{

    // stores the big gameObject that is the parent of all the sphere groups
    public GameObject AllItemsManager;

    // store each sphere group 
    public GameObject ItemGroup1, ItemGroup2, ItemGroup3, ItemGroup4;

    // set a list to store all the sphere groups
    List<GameObject> sphereGroups;

    public bool moveupAll, movedownAll, expandAll, shrinkAll, expandGrp1, expandGrp2, expandGrp3, expandGrp4,
        shrinkGrp1, shrinkGrp2, shrinkGrp3, shrinkGrp4;

    public float positionshrinkconstant, positionexpandconstant, scaleExpandconstant, scaleShrinkconstant;

    public bool moveUpGrp1,moveUpGrp2,moveUpGrp3,moveUpGrp4,moveDownGrp1,moveDownGrp2,moveDownGrp3,moveDownGrp4;

    // Start is called before the first frame update
    void Start()
    {
        sphereGroups = new List<GameObject>();

        // store all the sphere groups in a list 
        for(int i = 0; i < AllItemsManager.transform.childCount;i++)
        {
            sphereGroups.Add(AllItemsManager.transform.GetChild(i).gameObject);
        }

        positionshrinkconstant = 0.03f;
        positionexpandconstant = 0.03f;
        scaleExpandconstant = 0.03f;
        scaleShrinkconstant = 0.03f;

        moveupAll = false;
        movedownAll = false;
        expandAll = false;
        shrinkAll = false;
        expandGrp1 = false; 
        expandGrp2 = false;
        expandGrp3 = false;
        expandGrp4 = false;
        shrinkGrp1 = false;
        shrinkGrp2 = false;
        shrinkGrp3 = false;
        shrinkGrp4 = false;

        moveUpGrp1 = false;
        moveUpGrp2 = false;
        moveUpGrp3 = false;
        moveUpGrp4 = false;
        moveDownGrp1 = false;
        moveDownGrp2 = false;
        moveDownGrp3 = false;
        moveDownGrp4 = false;

    }

    void moveAllUp()
    {

        for(int i = 0; i < sphereGroups.Count; i++)
        {
            for(int j = 0; j < sphereGroups[i].transform.childCount; j++)
            {
                Vector3 temp = sphereGroups[i].transform.GetChild(j).position;
                sphereGroups[i].transform.GetChild(j).position = new Vector3(temp.x, temp.y + positionexpandconstant, temp.z);
            }
        }

        expandAll = false;
    }

    void moveAllDown()
    {
        for (int i = 0; i < sphereGroups.Count; i++)
        {
            for (int j = 0; j < sphereGroups[i].transform.childCount; j++)
            {
                Vector3 temp = sphereGroups[i].transform.GetChild(j).position;
                sphereGroups[i].transform.GetChild(j).position = new Vector3(temp.x, temp.y - positionshrinkconstant, temp.z);
            }
        }

        shrinkAll = false;

    }

    void expandAllSphere()
    {
        Vector3 temp = AllItemsManager.transform.localScale;
        AllItemsManager.transform.localScale = new Vector3(temp.x + positionexpandconstant, temp.y + positionexpandconstant, temp.z + positionexpandconstant);
    }

    void shrinkAllSphere()
    {
        Vector3 temp = AllItemsManager.transform.localScale;
        AllItemsManager.transform.localScale = new Vector3(temp.x - positionshrinkconstant, temp.y - positionshrinkconstant, temp.z - positionshrinkconstant);
    }
    void expandCurrentGroup(GameObject ItemGroupChosen)
    {
        for (int j = 0; j < ItemGroupChosen.transform.childCount; j++)
        {
            Vector3 temp = ItemGroupChosen.transform.GetChild(j).localScale;
          
                ItemGroupChosen.transform.GetChild(j).localScale = new Vector3(temp.x + scaleExpandconstant, temp.y + scaleExpandconstant, temp.z);
         }
    }
    void shrinkCurrentGroup(GameObject ItemGroupChosen)
    {
        for (int j = 0; j < ItemGroupChosen.transform.childCount; j++)
        {
            Vector3 temp = ItemGroupChosen.transform.GetChild(j).localScale;
            if (temp.x > 0 && temp.y > 0 && temp.z > 0)
                ItemGroupChosen.transform.GetChild(j).localScale = new Vector3(temp.x - scaleShrinkconstant, temp.y - scaleShrinkconstant, temp.z);
            else
                continue;
        
        }
    }

    void MoveDownPositionCurrentGroup(GameObject ItemGroupChosen)
    {
        for (int j = 0; j < ItemGroupChosen.transform.childCount; j++)
        {
            Vector3 temp = ItemGroupChosen.transform.GetChild(j).position;
            ItemGroupChosen.transform.GetChild(j).position = new Vector3(temp.x, temp.y - positionshrinkconstant, temp.z);
        }
    }

    void MoveUpPositionCurrentGroup(GameObject ItemGroupChosen)
    {
        for (int j = 0; j < ItemGroupChosen.transform.childCount; j++)
        {
            Vector3 temp = ItemGroupChosen.transform.GetChild(j).position;
            ItemGroupChosen.transform.GetChild(j).position = new Vector3(temp.x, temp.y + positionexpandconstant, temp.z);
        }

    }

    void Update()
    {
        if(movedownAll == true)
        {
            moveAllDown();
            movedownAll = false;
            return;
        }
        if (moveupAll == true)
        {
            moveAllUp();
            moveupAll = false;
            return;
        }
        if (expandAll)
        {
            expandAllSphere();
            expandAll = false;
            return;
        }
        if (shrinkAll)
        {
            shrinkAllSphere();
            shrinkAll = false;
            return;
        }
        if (expandGrp1)
        {
            expandCurrentGroup(ItemGroup1);
            expandGrp1 = false;
            return;
        }
        if (expandGrp2)
        {
            expandCurrentGroup(ItemGroup2);
            expandGrp2 = false;
            return;
        }
        if (expandGrp3)
        {
            expandCurrentGroup(ItemGroup3);
            expandGrp3 = false;
            return;
        }
        if (expandGrp4)
        {
            expandCurrentGroup(ItemGroup4);
            expandGrp4 = false;
            return;
        }

        if (shrinkGrp1)
        {
            shrinkCurrentGroup(ItemGroup1);
            shrinkGrp1 = false;
            return;
        }
        if (shrinkGrp2)
        {
            shrinkCurrentGroup(ItemGroup2);
            shrinkGrp2 = false;
            return;
        }
        if (shrinkGrp3)
        {
            shrinkCurrentGroup(ItemGroup3);
            shrinkGrp3 = false;
            return;
        }
        if (shrinkGrp4)
        {
            shrinkCurrentGroup(ItemGroup4);
            shrinkGrp4 = false;
            return;
        }



        if (moveUpGrp1)
        {
            MoveUpPositionCurrentGroup(ItemGroup1);
            moveUpGrp1 = false;
            return;
        }
        if (moveUpGrp2)
        {
            MoveUpPositionCurrentGroup(ItemGroup2);
            moveUpGrp2 = false;
            return;
        }
        if (moveUpGrp3)
        {
            MoveUpPositionCurrentGroup(ItemGroup3);
            moveUpGrp3 = false;
            return;
        }
        if (moveUpGrp4)
        {
            MoveUpPositionCurrentGroup(ItemGroup4);
            moveUpGrp4 = false;
            return;
        }

        if (moveDownGrp1)
        {
            MoveDownPositionCurrentGroup(ItemGroup1);
            moveDownGrp1 = false;
            return;
        }
        if (moveDownGrp2)
        {
            MoveDownPositionCurrentGroup(ItemGroup2);
            moveDownGrp2 = false;
            return;
        }
        if (moveDownGrp3)
        {
            MoveDownPositionCurrentGroup(ItemGroup3);
            moveDownGrp3 = false;
            return;
        }
        if (moveDownGrp4)
        {
            MoveDownPositionCurrentGroup(ItemGroup4);
            moveDownGrp4 = false;
            return;
        }

    }
}
