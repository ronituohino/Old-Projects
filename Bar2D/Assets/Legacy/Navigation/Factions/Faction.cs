using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "World/Faction")]
public class Faction : ScriptableObject
{
    // Does this faction attack or avoid this ship on sight 
    public enum FactionAttitude
    {
        Friendly, // Greet and offer help and services
        Neutral, // Ignore
        Hostile, // If a combat ship, attack, else avoid
    }

    public FactionAttitude factionAttitude;

    // Determines what type of faction this is
    public enum FactionTrait
    {
        // Wealth
        // Affects ordered drink price
        Wealthy,
        Poor,
    }

    public FactionTrait[] traits;
}