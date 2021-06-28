using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Faction : ScriptableObject
{
    public string factionName;

    public static bool operator ==(Faction a, Faction b)
    {
        if(a.factionName == b.factionName)
        {
            return true;
        } else
        {
            return false;
        }
    }

    public static bool operator !=(Faction a, Faction b)
    {
        if (a.factionName == b.factionName)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public override bool Equals(object obj)
    {
        return obj is Faction faction &&
               base.Equals(obj) &&
               factionName == faction.factionName;
    }

    public override int GetHashCode()
    {
        var hashCode = 1786342774;
        hashCode = hashCode * -1521134295 + base.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(factionName);
        return hashCode;
    }
}