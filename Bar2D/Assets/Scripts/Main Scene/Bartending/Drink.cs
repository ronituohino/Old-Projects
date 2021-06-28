using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
public class Drink : ScriptableObject
{
    [System.Serializable]
    public struct Ingredient
    {
        //Drinks the customer wants
        public Bottle bottle;
        //True means customer wants specific drink (Chardnae, Red wine), false means they want generally something (Red wine)
        public bool specific;
        //Ratios of the entire drink
        [Range(0, 1)]
        public float ratio;
    }

    public Ingredient[] ingredients;

    //Glass type they want it served in
    public Glass.GlassType glassType;

    [Space]

    //Does it need to be a certain color?
    public bool hasToBeSpecificColor = false;
    public Color color;

    //What additives should the drink have?
    [System.Serializable]
    public class Additives
    {
        public Additive.AdditiveType additive;
        public int amount;
    }

    [Space]

    public List<Additives> additives;

    // Used to define which drinks have the most requirements,
    // this makes sure that the ones that have the most requirements
    // are checked first when comparing glasses and drinks (prevents bugs)
    public float complexity = 0f;
}