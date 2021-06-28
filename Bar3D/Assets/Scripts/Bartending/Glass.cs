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

    [Tooltip("In liters")]
    public float capacity;

    public Vector2 liquidShaderFillRange;

    public AnimationCurve spillCurve;
}