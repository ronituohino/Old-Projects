using UnityEngine;
using UnityEngine.AI;

//Contains info about the ships that are parked in our bar, used in CustomerManager.cs
[System.Serializable]
public class ShipData
{
    public GameObject gameObject;
    public NavMeshAgent navigation;

    public GameObject[] passengers;
    public bool showPassengers;

    public Transform[] seats;

    public bool parked;
    public int spawnInt;

    public ShipData(GameObject gameObject, NavMeshAgent navigation, GameObject[] passengers, bool showPassengers, Transform[] seats, bool parked, int spawnInt)
    {
        this.gameObject = gameObject;
        this.navigation = navigation;
        this.passengers = passengers;
        this.showPassengers = showPassengers;
        this.seats = seats;
        this.parked = parked;
        this.spawnInt = spawnInt;
    }
}
