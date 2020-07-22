using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class townScripts : MonoBehaviour {
    private static GameObject townHall;

    public static LineRenderer myArea;

    public static GameObject peopleList;
    public static GameObject prs;

    public static int startingPopulation;
    public static int curPopulation = 0;

    private static Material[] torsoColors;
    private static Material[] feetColors;
    private static Material[] legColors;
    private static Material[] skinColors;
    /*private void Start() //temp
    {
        startTown();
    }*/

    public static void startTown(GameObject townBuilding, int startPop, Material[] tColors, Material[] fColors, Material[] lColors, Material[] sColors, GameObject person) //Called when a city is founded
    {
        torsoColors = tColors;
        feetColors = fColors;
        legColors = lColors;
        skinColors = sColors;
        prs = person;

        GameObject list = Instantiate(new GameObject("TownPeople"), new Vector3(0, 0, 0), Quaternion.identity, townBuilding.transform) as GameObject;
        peopleList = list;

        townHall = townBuilding;
        calculateTownArea();
        growPopulation(startPop);
    }

    public static void growPopulation(int populationGrowth) //Grow population by populationGrowth, instatiate a person at the town hall and give him a tag
    {
        for(int i = 0; i < populationGrowth; i++)
        {
            GameObject personClone = Instantiate(prs, townHall.transform.position, Quaternion.identity, peopleList.transform) as GameObject;
            Person personCloneId = personClone.GetComponent<Person>();

            personCloneId.personTag = curPopulation.ToString(); //Set the tag of this person
            personClone.name = curPopulation.ToString();

            personCloneId.outfit = generateOutfit(); //Set this person's outfit
            Material[] personOutfit = personClone.GetComponentInChildren<MeshRenderer>().sharedMaterials; //Get the materials

            personOutfit[0] = torsoColors[(int)char.GetNumericValue(personCloneId.outfit[0])];
            personOutfit[1] = feetColors[(int)char.GetNumericValue(personCloneId.outfit[1])];
            personOutfit[2] = legColors[(int)char.GetNumericValue(personCloneId.outfit[2])];
            personOutfit[3] = skinColors[(int)char.GetNumericValue(personCloneId.outfit[3])];

            personClone.GetComponentInChildren<MeshRenderer>().sharedMaterials = personOutfit; //Set the materials

            curPopulation++; //Increase total population
        }
    }

    private static string generateOutfit() //Generetasen an outfit code (ie. 1023 or 3213)
    {
        string outfit = "";

        outfit = outfit.Insert(0,Random.Range(0, skinColors.Length).ToString()); //Skin color
        outfit = outfit.Insert(1, Random.Range(0, torsoColors.Length).ToString()); //Torso color
        outfit = outfit.Insert(2, Random.Range(0, legColors.Length).ToString()); //Leg color
        outfit = outfit.Insert(3, Random.Range(0, feetColors.Length).ToString()); //Feet color

        //Debug.Log(outfit);
        return outfit;
    }

    private static void calculateTownArea() //Calculate your area in which you own and can build on
    {
        Vector3 townPos = townHall.transform.position;
        
    }
}
