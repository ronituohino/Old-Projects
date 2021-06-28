using UnityEngine;
using System.Collections.Generic;

//Handles cross-faction stuff
public class FactionManager : Singleton<FactionManager>
{
    public List<Faction> existingFactions = new List<Faction>();
    int factionCount;

    public int[][] relations;

    //If relation is at a certain point, attitude between faction changes
    public enum Attitude
    {
        Enemy,
        Neutral,
        Ally
    }

    private void Awake()
    {
        factionCount = existingFactions.Count;
        relations = new int[factionCount][];

        for (int i = 0; i < factionCount; i++)
        {
            relations[i] = new int[factionCount];
        }
    }

    public int GetRelationBetween(Faction a, Faction b)
    {
        int aIndex = 0;
        int bIndex = 0;

        for(int i = 0; i < factionCount; i++)
        {
            if(existingFactions[i] == a)
            {
                aIndex = i;
            }
            if (existingFactions[i] == b)
            {
                bIndex = i;
            }
        }

        return relations[aIndex][bIndex];
    }

    public Attitude GetAttitude(int relation)
    {
        if(relation < -90)
        {
            return Attitude.Enemy;
        } 
        else if(relation < 0)
        {
            return Attitude.Neutral;
        } 
        else
        {
            return Attitude.Ally;
        }
    }
}