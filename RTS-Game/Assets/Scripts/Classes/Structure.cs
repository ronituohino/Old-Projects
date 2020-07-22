using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour {
    public string owner;


    [System.Serializable]
    public class ResourceCost
    {
        public string resource;
        public int amount;
    }
    public ResourceCost[] resourceCostType;    //How much and what does it cost?

    public bool produces;   //Does the building produce anything
    public string productionType;  //What resource does it produce?
    public int production;     //If so, how much?     

    public int buildTime; //Build time in turns
    public bool beingBuilt; //Is the structure currently being built?

    //public string buildingType;    //What kind of structure is this?
    //Production (Raw resources; wood, metal etc..), Housing (Income), Science (Discoveries and advancing), Military (Warfare)

    public bool hasActions = true; //Does the building have actions


    [System.Serializable]
    public class Action //Action class and constructor
    {
        [HideInInspector]
        public string _name;
        public bool _enabled;

        public Action(string name, bool enabled)
        {
            _name = name;
            _enabled = enabled;
        }
        
    }
    
    public Action[] actions = { new Action("Raze", true), new Action("Dismantle", true), new Action("Market", false) }; //All the actions are listed here

    //public bool raze = true;
    //public bool dismantle = true;
    //public bool market = false;
}
