using NVIDIA.Flex;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Hooks to the NVIDIA Flex system
public class FluidManager : Singleton<FluidManager>
{
    public int supportedFluidAmount;

    [HideInInspector]
    public GameObject[] fluidObjects;
    [HideInInspector]
    public int pourObjectPointer = 0;
    [HideInInspector]
    public _auxFlexDrawFluid[] fluids;

    public int GetPointer()
    {
        pourObjectPointer++;
        if(pourObjectPointer >= supportedFluidAmount)
        {
            pourObjectPointer = 0;
        }
        
        return pourObjectPointer;
    }

    void Start()
    {
        fluidObjects = new GameObject[supportedFluidAmount];
        fluids = new _auxFlexDrawFluid[supportedFluidAmount];

        for (int i = 0; i < supportedFluidAmount; i++)
        {
            GameObject g = transform.GetChild(i).gameObject;
            fluidObjects[i] = g;
            g.SetActive(true);
        }
    }
}
