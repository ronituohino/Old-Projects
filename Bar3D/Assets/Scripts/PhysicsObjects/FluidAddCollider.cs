using System.Collections;
using UnityEngine;

public class FluidAddCollider : MonoBehaviour
{
    [SerializeField] GlassPhysics connectedContainer;

    public void AddFluid(Bottle b, float units)
    {
        connectedContainer.AddFluid(b, units);
    }
}