using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIHumanController : HumanController
{
    public Behaviour behaviour;
    public Type aiType;

    public Character character;
    
    public FieldOfView fieldOfView;
    public List<SeenCharacter> seenCharacters = new List<SeenCharacter>();

    [System.Serializable]
    public struct SeenCharacter
    {
        public Collider2D character;
        public float timeSeen;

        public SeenCharacter(Collider2D character, float time)
        {
            this.character = character;
            this.timeSeen = time;
        }
    }

    public float forgetTime;

    List<Collider2D> hostileCharacters = new List<Collider2D>();

    public float hearingRange;
    bool seenPlayer = false;

    bool inCombat = false;

    public enum Behaviour
    {
        Friendly,
        Neutral,
        Hostile
    }

    public enum Type
    {
        Human,
        Robot
    }

    void Awake()
    {
        fieldOfView.OnTargetsScanned += AnalyzeScannedTargets;
    }

    //This is called after FieldOfView has scanned surroundings for new players (Collider2D)
    void AnalyzeScannedTargets()
    {
        List<int> scannedIndexes = new List<int>();

        foreach (Collider2D collider in fieldOfView.visibleTargets)
        {
            //Check if we have seen this person recently
            int index = FindCharacterInSeen(collider);
            if (index == -1)
            {
                //Identify the character
                Character c = collider.GetComponent<Character>();
                int relation = FactionManager.Instance.GetRelationBetween(c.associatedFaction, character.associatedFaction);
                FactionManager.Attitude attitude = FactionManager.Instance.GetAttitude(relation);

                seenCharacters.Add(new SeenCharacter(collider, Time.time));
            }
            else
            {
                scannedIndexes.Add(index);
                seenCharacters[index] = new SeenCharacter(collider, Time.time);
            }
        }

        int count = seenCharacters.Count;
        for(int i = 0; i < count; i++)
        {
            if(!scannedIndexes.Contains(i))
            {
                float timeDifference = Time.time - seenCharacters[i].timeSeen;
                if(timeDifference > forgetTime)
                {
                    seenCharacters.RemoveAt(i);
                    count--;

                    int scannedCount = scannedIndexes.Count;
                    for (int s = i; s < scannedCount; s++)
                    {
                        scannedIndexes[s]--;
                    }
                }
            }
        }
    }

    int FindCharacterInSeen(Collider2D collider)
    {
        int count = seenCharacters.Count;
        for(int i = 0; i < count; i++)
        {
            if (seenCharacters[i].character == collider)
            {
                return i;
            }
        }

        return -1;
    }

    private void Update()
    {
        //AI combat behaviour
        if(inCombat)
        {

        }
        //If not in a combat situation, advance current task
        else
        {

        }
    }
}