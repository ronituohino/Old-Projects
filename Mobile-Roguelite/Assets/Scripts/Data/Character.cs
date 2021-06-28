using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Character")]
// Serves as a base for a character, hero or an enmey
public class Character : ScriptableObject
{
    public enum ClassType
    {
        Melee,
        Ranged
    }

    public ClassType classType;

    public Sprite sprite;

    // Detachable data from the Character -class, will be modified at runtime
    [System.Serializable]
    public struct CharacterData
    {
        // The position the character has in Party
        public int characterId;

        [System.Serializable]
        public struct Stats
        {
            [Header("Universal")]
            public int maxHealth;
            public int currentHealth;

            public int attack;
            public float swiftness; // Agent.speed

            public float attackSpeed; // Time interval between attacks
            public float attackRange; // In world units

            public Stats Sum(Stats a, Stats b)
            {
                Stats s = new Stats();
                s.maxHealth         =   a.maxHealth + b.maxHealth;
                s.currentHealth     =   a.currentHealth + b.currentHealth;
                s.attack            =   a.attack + b.attack;
                s.swiftness         =   a.swiftness + b.swiftness;
                s.attackSpeed       =   a.attackSpeed + b.attackSpeed;
                s.attackRange       =   a.attackRange + b.attackRange;
                return s;
            }
        }

        public Stats stats;

        [Header("Melee Only")]

        public float attackAdvantageTime;

        [Header("Ranged Only")]

        public float projectileVelocity;
    }

    public CharacterData data;
}