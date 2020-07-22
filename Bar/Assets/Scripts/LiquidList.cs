using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Contains all the liquids in the game
public class LiquidList : Singleton<LiquidList>
{
    public Material[] liquids;

    [HideInInspector]
    public int amount;

    private void Awake()
    {
        amount = liquids.Length;
    }
}
