using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluidManager : MonoBehaviour
{
    [Header("Particle Settings")]
    [SerializeField] GameObject particleObject;

    [Space]

    [SerializeField] float groundHitForceMultiplier;

    [Space]

    public float additiveHeightToPour;
    [SerializeField] float secondsBeforeDisappear;
    [SerializeField] float endEnabledPercentage;

    [SerializeField] float fadeDuration;
    [SerializeField] int fadeIterations;

    HashSet<Fluid> particles = new HashSet<Fluid>();

    private void Start()
    {
        GlobalReferencesAndSettings.Instance.fluidManager = this;
    }

    public void CreateFluid(Vector2 position, Bottle bottle, float secondsToGroundHit)
    {
        Fluid fluid = Instantiate(particleObject, position, Quaternion.identity, transform).GetComponent<Fluid>();

        fluid.bottle = bottle;

        fluid.groundHitSeconds = secondsToGroundHit;
        fluid.endEnabledPercentage = endEnabledPercentage;
        fluid.hitForceMultiplier = groundHitForceMultiplier;

        fluid.disappearWaitFUI = Mathf.RoundToInt(secondsBeforeDisappear / Time.fixedDeltaTime);

        particles.Add(fluid);
    }

    public void DestroyFluid(Fluid fluid)
    {
        particles.Remove(fluid);
        Destroy(fluid.gameObject);
    }
}
