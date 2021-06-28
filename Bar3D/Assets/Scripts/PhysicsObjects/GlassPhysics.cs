using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

// Custom functionality for all glasses, they are targets to pour alcohol into :D
public class GlassPhysics : PhysicsObject, ILeftDraggable, IRightDraggable, IHoverable
{
    public Glass glass;

    [SerializeField] MeshRenderer liquidRenderer;
    Material liquidMat;

    // Called whenever the fluid amount is updated
    public UnityAction OnFluidUpdate;

    [System.Serializable]
    public struct Contents : IEquatable<Drink.Ingredient>
    {
        public Bottle bottle;
        public float units;

        bool IEquatable<Drink.Ingredient>.Equals(Drink.Ingredient other)
        {
            if (other.specific)
            {
                return other.bottle == bottle;
            }
            else
            {
                return other.bottle.fluidType == bottle.fluidType;
            }
        }
    }

    public List<Contents> containedFluids = new List<Contents>();

    [Range(0f, 1f)]
    public float fullness;

    new void Start()
    {
        liquidMat = liquidRenderer.material;
        base.Start();
    }

    // If this glass falls over, spill contents
    private void FixedUpdate()
    {
        Spill();
    }

    void Spill()
    {

    }

    public void AddFluid(Bottle b, float units)
    {
        float fullnessAdd = units / glass.capacity;
        fullness += fullnessAdd;

        bool full = false;
        if (fullness >= 1f)
        {
            // Overflow
            full = true;
            fullness = 1f;
        }

        bool hasThisFluid = false;
        int fluidCount = containedFluids.Count;
        for (int i = 0; i < fluidCount; i++)
        {
            Contents c = containedFluids[i];
            if (c.bottle == b)
            {
                hasThisFluid = true;

                // If this is already in the list, just update units
                c.units += units;

                containedFluids[i] = c;

                if (full)
                {
                    // If the glass is full, remove units amount of all liquids equally
                    RemoveAllContentsEqually(units);
                }

                break;
            }
        }

        if (!hasThisFluid)
        {
            Contents c = new Contents();
            c.bottle = b;
            c.units = units;

            containedFluids.Add(c);

            if (full)
            {
                // If the glass is full, remove units amount of all liquids equally
                RemoveAllContentsEqually(units);
            }
        }

        // Remap liquidShaderFillRange is inverted here because the shader is odd
        liquidMat.SetFloat("_FillAmount", fullness.Remap(0, 1, glass.liquidShaderFillRange.x, glass.liquidShaderFillRange.y));

        OnFluidUpdate?.Invoke();
    }

    void RemoveAllContentsEqually(float units)
    {
        int fluidCount = containedFluids.Count;
        float removeAmount = units / fluidCount;

        for (int i = 0; i < fluidCount; i++)
        {
            Contents c = containedFluids[i];
            c.units -= removeAmount;
            containedFluids[i] = c;
        }
    }

    // UI stuff
    public void SetAsPourTarget()
    {

    }

    public void UnsetAsPourTarget()
    {

    }
}