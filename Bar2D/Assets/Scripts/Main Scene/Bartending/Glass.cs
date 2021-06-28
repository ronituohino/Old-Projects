using UnityEngine;

[CreateAssetMenu(menuName = "Bartending/Glass")]
public class Glass : BartendingItem
{
    public GlassType glassType;

    public enum GlassType
    {
        Glass,
        ShotGlass,
        WineGlass,
    }

    public int fluidCapacity;

    public float angleForAcceptibleParticle;

    public Vector2 liquidShaderFillRange;

    public AnimationCurve spillCurve;
}