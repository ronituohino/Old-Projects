using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class resourceHandler : MonoBehaviour {
    public static GameObject buildingList;
    static Structure childStructure;

    public static buildHandler bh;

    public static double gold;
    public static TextMeshProUGUI goldString;
    public static double wood;
    public static TextMeshProUGUI woodString;
    public static double stone;
    public static TextMeshProUGUI stoneString;
    public static double food;
    public static TextMeshProUGUI foodString;
    //public Resource idontfuckingknow; //Add more idk

    private void Start()
    {
        buildingList = GameObject.Find("Buildings");
        goldString = GameObject.Find("Money").GetComponentInChildren<TextMeshProUGUI>();
        woodString = GameObject.Find("Wood").GetComponentInChildren<TextMeshProUGUI>();
        stoneString = GameObject.Find("Stone").GetComponentInChildren<TextMeshProUGUI>();
        foodString = GameObject.Find("Food").GetComponentInChildren<TextMeshProUGUI>();

        bh = GameObject.Find("Build").GetComponent<buildHandler>();
        resetResources();
    }

    public static void calculateResources() //Called on start of a player's turn
    {
        for(int i = 0; i < buildingList.transform.childCount; i++)
        {
            childStructure = buildingList.transform.GetChild(i).GetComponent<Structure>();

            if (childStructure.produces) //Check if this structure is a production building
            {
                //Debug.Log(childStructure.production);
                if (!childStructure.beingBuilt) //If the structure isn't being built
                {
                    if (childStructure.productionType == "Gold")
                    {
                        gold += childStructure.production;
                    }
                    else if (childStructure.productionType == "Wood")
                    {
                        wood += childStructure.production;
                    }
                    else if (childStructure.productionType == "Stone")
                    {
                        stone += childStructure.production;
                    }
                    else if (childStructure.productionType == "Food")
                    {
                        food += childStructure.production;
                    }
                } 
            }

            if (childStructure.beingBuilt) //If this structure is being built
            {
                countDownBuildTime(childStructure); //Reduce the counter by 1.
            }
        }

        UpdateResources();
    }

    private static void countDownBuildTime(Structure obj) //Count down the buildTimer on a structure that is being built
    {
        obj.buildTime -= 1;
        if(obj.buildTime == 0) //Build time is now 0, change the mesh
        {
            obj.beingBuilt = false;
            string objName = obj.gameObject.name;
            for(int i = 0; i < bh.buildings.Length; i++)
            {
                if(objName == bh.buildings[i].name)
                {
                    obj.GetComponent<MeshFilter>().sharedMesh = bh.buildings[i].GetComponent<MeshFilter>().sharedMesh;
                }
            } 
        }
    }

    public static void loadResources() //Called on the start of the game
    {
        //Debug.Log("Loaded resources");
    }

    public void resetResources()
    {
        gold = 0;
        wood = 0;
        stone = 0;
        food = 0;
    }

    public static void UpdateResources() //Updates the texts on the top
    {
        goldString.text = gold.ToString();
        woodString.text = wood.ToString();
        stoneString.text = stone.ToString();
        foodString.text = food.ToString();
    }
}
