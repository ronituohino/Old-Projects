using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//An asset, contains info to spawn a ship
[CreateAssetMenu]
public class Ship : ScriptableObject
{
    public GameObject prefab;
    public float shipRadius;

    public bool showPassengersInShip;

    public GameObject[] specificPassengers;
    public bool onlySpecifics;

    public Doorposition doorposition;
    public int dooramount;
}

public enum Doorposition
{
    Right,
    Left,
    Both,
}
