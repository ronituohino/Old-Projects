using UnityEngine;

[CreateAssetMenu()]
public class HealthProfile : ScriptableObject
{
    [Header("Initial damage")]

    public Vector2 minorWoundInitialDamage;
    public Vector2 severeWoundInitialDamage;
    public Vector2 extremeWoundInitialDamage;

    public float[] limbInitialDamageMultipliers;

    [Space]

    [Header("Bleeding")]

    public Vector2 minorWoundBleed;
    public Vector2 severeWoundBleed;
    public Vector2 extremeWoundBleed;

    public float[] limbHealthBleedMultipliers;

    [Space]

    [Header("Regenerating")]

    public Vector2 minorWoundRegenerate;
    public Vector2 severeWoundRegenerate;
    public Vector2 extremeWoundRegenerate;

    [Space]

    [Header("Wound severity")]

    [Range(0, 1f)]
    public float severitySway;
    public float severeWoundThreshold;
    public float extremeWoundThreshold;
}