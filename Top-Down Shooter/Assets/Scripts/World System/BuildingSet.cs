using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class BuildingSet : ScriptableObject
{
    public List<Set> sets;

    [System.Serializable]
    public struct Set
    {
        public enum BuildingType
        {
            Housing,
        }

        public BuildingType buildingType;

        public List<Building> buildings;
    }

    [System.Serializable]
    public struct Building
    {
        public string buildingName;
        public Vector2Int buildingSize;

        public GameObject[] buildingVariants;
    }
}