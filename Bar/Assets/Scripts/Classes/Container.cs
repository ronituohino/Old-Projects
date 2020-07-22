using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Container : MonoBehaviour
{
    [Range(0, 1)]
    public float fullness;
    public float volume;
    public LiquidMix liquidMix;
}

