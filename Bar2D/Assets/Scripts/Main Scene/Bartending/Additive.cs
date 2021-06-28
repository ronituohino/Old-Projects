using UnityEngine;

[CreateAssetMenu(menuName = "Bartending/Additive")]
public class Additive : BartendingItem
{
    public AdditiveType additiveType;
    
    public enum AdditiveType
    {
        Ice,
        Lemon,
        Lime,
    }
}