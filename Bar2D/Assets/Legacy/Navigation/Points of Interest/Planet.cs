using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "World/PointsOfInterest/Planet")]
public class Planet : PointOfInterest
{
    public enum PlanetType
    {
        // Perfect, good for anything
        // Usually lots of population and people to pick up, also offer a lot of items == Tier 4
        Earth,

        // Lively, good for life
        // Tier 3
        Jungle,
        Water,

        // Arid, good for industry and small amounts of life
        // Tier 2
        Ice,
        Magma,
        Desert,

        // Dead, good for nothing
        // Tier 1
        Rock,
    }

    public PlanetType planetType;

    public bool atmosphere;
    public Faction habitedByFaction = null;

    public enum PlanetTraits
    {
        
    }

    public PlanetTraits[] planetTraits;
}
