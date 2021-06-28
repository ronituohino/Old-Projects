using System.Collections.Generic;
using UnityEngine;

// Contains the data necessary to load an area
[System.Serializable]
public class Area
{
    public string name;

    public int seed = 0;

    public Vector2Int size;

    //Types of buildings
    public enum Type
    {
        Empty,
        Road,
        City,
        Building,
    }

    //[System.Serializable]
    public struct Building
    {
        public Type buildingType;
        public int buildingCategory;
        public int buildingVariant;

        public int debug;
        public Vector2 debugScale;

        public Vector2 position;
        public int rotation; //(0-3 * 90 degrees)
    }

    public List<Building> buildings;

    public enum AreaType
    {
        Unknown,
        Town,
        Market,
    }

    public AreaType areaType = AreaType.Unknown;
}