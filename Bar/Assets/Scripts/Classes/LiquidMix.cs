using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LiquidMix
{
    public List<Material> liquids;

    [Range(0f, 1f)]
    public List<float> ratios;

    public LiquidMix(List<Material> liquids, List<float> ratios)
    {
        this.liquids = liquids;
        this.ratios = ratios;
    }
    //Return mix color


}