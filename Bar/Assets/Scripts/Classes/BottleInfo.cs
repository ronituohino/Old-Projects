using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BottleInfo : ScriptableObject
{
    public GameObject prefab;
    
    [Space]

    public string drinkName;
    public float physicalCapacity;

    [Space]

    public Material liquid;
}


