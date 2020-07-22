using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class actionsList : MonoBehaviour {
    public void raze() //Destroy the building immediately/in one turn
    {
        Destroy(buildingSelection.selectedObject, 0.01f);
    }
    public void dismantle() //Dismantle the building in a few turns but you get some of the resources back
    {

    }
    public void openMarket() //Open the market panel
    {

    }
}
