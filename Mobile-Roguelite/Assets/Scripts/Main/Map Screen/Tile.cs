using System.Collections;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool triggered = false;

    public enum TileType
    {
        Empty,
        Encounter,
        Trap,
        Loot,
        Shop,

        Start,
        Finish
    }

    public TileType tileType;
}