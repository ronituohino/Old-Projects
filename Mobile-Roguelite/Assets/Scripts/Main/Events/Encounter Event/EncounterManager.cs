using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class EncounterManager : Singleton<EncounterManager>
{
    [Header("References")]

    [SerializeField] ObjectPooler characterPooler;
    public ObjectPooler projectilePooler;
    public ObjectPooler visualsPooler;

    [HideInInspector]
    public List<AI> enemies = new List<AI>();
    [HideInInspector]
    public List<AI> heroes = new List<AI>();

    [Space]

    [SerializeField] GameObject[] objectsToToggleOnTransition;

    [Header("Arena")]

    public List<Transform> arenas = new List<Transform>();
    Transform activeArena = null;



    [Header("Enemies")]

    public EnemyGroup[] enemyGroups;

    [System.Serializable]
    public struct EnemyGroup
    {
        public Character[] enemies;
    }

    public Vector2 enemySpawnAreaBottomLeft;
    public Vector2 enemySpawnAreaTopRight;

    public Vector2 friendlySpawnAreaBottomLeft;
    public Vector2 friendlySpawnAreaTopRight;

    [Header("Settings")]

    public float targetUpdateInterval;
    public float tieToBattleMovementSpeedMultiplier;



    public void TransitionTo()
    {
        FadeManager.Instance.Fade(true);
        StartCoroutine(Extensions.AnimationWait(FadeManager.Instance.animator, "FadeIn", GenerateEncounter));
    }



    // Generates a random encounter and loads encounter
    public void GenerateEncounter()
    {
        foreach(GameObject g in objectsToToggleOnTransition)
        {
            g.SetActive(false);
        }

        // Select random arena
        activeArena = arenas[Random.Range(0, arenas.Count)];
        activeArena.gameObject.SetActive(true);

        // Spawn a random set of enemies
        EnemyGroup eg = enemyGroups[Random.Range(0, enemyGroups.Length)];
        foreach(Character c in eg.enemies)
        {
            SpawnCharacter
                        (
                            c, 
                            c.data,
                            new Vector2
                                    (
                                        Random.Range(enemySpawnAreaBottomLeft.x, enemySpawnAreaTopRight.x), 
                                        Random.Range(enemySpawnAreaBottomLeft.y, enemySpawnAreaTopRight.y)
                                    ),
                            true
                        );
        }

        // Spawn party
        foreach(Hero hp in PartyManager.Instance.heroes)
        {
            if(hp.characterBase != null)
            {
                SpawnCharacter
                        (
                            hp.characterBase,
                            hp.currentData,
                            new Vector2
                                    (
                                        Random.Range(friendlySpawnAreaBottomLeft.x, friendlySpawnAreaTopRight.x),
                                        Random.Range(friendlySpawnAreaBottomLeft.y, friendlySpawnAreaTopRight.y)
                                    ),
                            false
                        );
            }
        }

        FadeManager.Instance.Fade(false);
    }

    

    // General method that spawns characters in field
    public void SpawnCharacter(Character characterBase, Character.CharacterData characterData, Vector2 position, bool enemy)
    {
        if(characterData.stats.currentHealth <= 0)
        {
            return;
        }

        GameObject g;

        if(enemy)
        {
            g = characterPooler.Retrieve(1);
            g.layer = 8;
        }
        else
        {
            g = characterPooler.Retrieve(0);
            g.layer = 7;
        }

        g.transform.position = position;


        AI ai = null;

        switch (characterBase.classType)
        {
            case Character.ClassType.Melee:
                ai = g.AddComponent<MeleeAI>();
                break;
            case Character.ClassType.Ranged:
                ai = g.AddComponent<RangedAI>();
                break;
        }

        ai.enabled = true;



        // Copy stats
        /*ai.characterData = new Character.CharacterData();
        ai.characterData.stats = new Character.CharacterData.Stats();

        ai.characterData.stats.health = characterData.stats.health;
        ai.characterData.stats.attack = characterData.stats.attack;
        ai.characterData.stats.swiftness = characterData.stats.swiftness;

        ai.characterData.stats.attackSpeed = characterData.stats.attackSpeed;
        ai.characterData.stats.attackRange = characterData.stats.attackRange;*/
        ai.originalCharacterData = characterData;
        ai.characterData = characterData;



        // Reference necessary components & tweak them
        ai.enemy = enemy;
        ai.rb = g.GetComponent<Rigidbody2D>();
        
        
        

        // Sprite renderer
        ai.sr = g.GetComponent<SpriteRenderer>();
        ai.sr.sprite = characterBase.sprite;
        ai.sr.material = new Material(ai.sr.material);

        // Box collider
        ai.col = g.GetComponent<BoxCollider2D>();
        ai.col.size = ai.sr.sprite.bounds.size;

        // NavMeshAgent
        ai.agent = g.GetComponent<NavMeshAgent>();
        ai.agent.updateRotation = false;
        ai.agent.updateUpAxis = false;
        ai.agent.speed = characterData.stats.swiftness;

        ai.userSetPosition = position;



        if(enemy)
        {
            enemies.Add(ai);
        }
        else
        {
            heroes.Add(ai);
        }
    }

    public void SpawnProjectile(int projectile, Vector2 position, Vector2 target, Vector2 velocity, bool enemy, int damage)
    {
        GameObject g = projectilePooler.Retrieve(projectile);
        g.layer = 9;
        g.transform.position = position;

        switch(projectile)
        {
            case 0:
                g.transform.rotation = Quaternion.Euler(0, 0, Extensions.LookAt(position, target) + 45f);
                break;
        }

        Projectile p = g.GetComponent<Projectile>();
        p.enemy = enemy;
        p.damage = damage;

        Rigidbody2D rb = g.GetComponent<Rigidbody2D>();
        rb.AddForce(velocity, ForceMode2D.Impulse);
    }

    public void SpawnSwing(AI origin, AI target)
    {
        GameObject g = visualsPooler.Retrieve(2);

        Vector2 toTarget = target.transform.position - origin.transform.position;
        Vector2 position = origin.transform.position.ToV2() + (toTarget.normalized * origin.characterData.stats.attackRange * 0.5f);

        g.transform.position = position;
        g.transform.rotation = Quaternion.Euler(0, 0, Extensions.LookAt(position, target.transform.position));

        g.GetComponent<Swing>().SwingAtTarget(origin, target);
    }



    public void KillAI(AI ai)
    {
        ai.alive = false;

        if (ai.enemy)
        {
            if (enemies.Sum(x => x.alive ? 1 : 0) == 0)
            {
                EndEncounter();
            }
        }
        else
        {
            if(heroes.Sum(x => x.alive ? 1 : 0) == 0)
            {
                EndEncounter();
            }
        }

        characterPooler.Return(ai.gameObject);
    }



    void CleanEncounter()
    {
        foreach (AI ai in enemies)
        {
            characterPooler.Return(ai.gameObject);
            Destroy(ai);
        }

        foreach (AI ai in heroes)
        {
            characterPooler.Return(ai.gameObject);
            Destroy(ai);
        }

        enemies.Clear();
        heroes.Clear();
        activeArena.gameObject.SetActive(false);
    }

    void EndEncounter()
    {
        Dictionary<int, List<Status>> outcomeHeroStatuses = new Dictionary<int, List<Status>>();

        // Generate statuses for heroes from this encounter
        foreach(AI ai in heroes)
        {
            List<Status> statuses = new List<Status>();

            if(ai.originalCharacterData.stats.currentHealth > ai.characterData.stats.currentHealth)
            {
                Status woundedStatus = StatusManager.Instance.FindStatus("Wounded");
                int woundAmount = ai.originalCharacterData.stats.currentHealth - ai.characterData.stats.currentHealth;

                for(int i = 0; i < woundAmount; i++)
                {
                    statuses.Add(woundedStatus);
                }                
            }

            outcomeHeroStatuses.Add(ai.characterData.characterId, statuses);
        }

        GameManager.Instance.TransitionToMain(outcomeHeroStatuses);
        StartCoroutine(Extensions.AnimationWait(FadeManager.Instance.animator, "FadeIn", CleanEncounter));
    }
}