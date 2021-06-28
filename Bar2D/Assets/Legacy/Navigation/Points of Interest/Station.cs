using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "World/PointsOfInterest/Station")]
public class Station : PointOfInterest
{
    public Faction habitedByFaction;

    public GameObject stationObject;
}