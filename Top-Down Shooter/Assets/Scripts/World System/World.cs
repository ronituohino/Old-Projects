//Contains the world map data
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class World
{
    public int seed;
    public List<PointOfInterest> pointsOfInterest = new List<PointOfInterest>();

    [System.Serializable]
    public struct PointOfInterest
    {
        public Area area;
        public Vector2 position;

        public PointOfInterest(Area area, Vector2 position)
        {
            this.area = area;
            this.position = position;
        }
    }
}