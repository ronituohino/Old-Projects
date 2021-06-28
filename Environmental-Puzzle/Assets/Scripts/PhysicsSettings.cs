using UnityEngine;

public class PhysicsSettings : Singleton<PhysicsSettings>
{
    public float hoverMovementMulitplier;

    [Space]

    public float minHoverDistance;
    public float maxHoverDistance;
    public float distanceScrollLerp;
    public float distanceScrollMultiplier;

    [Space]

    public float maximumVelocity;
    public float maximumAngularVelocity;

    [Space]

    public float maximumForce;

    [Space]

    public float drag;
    public float angularDrag;

    public float noSmoothingVelocityThreshold;
    public float noSmoothingDistanceThreshold;
}