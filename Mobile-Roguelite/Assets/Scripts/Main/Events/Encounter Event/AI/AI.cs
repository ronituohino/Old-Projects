using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using UnityEngine.UI;

// Parent script for defined AI behaviours, contains general methods and properties for an entity
public class AI : Interactable, IDamageable
{
    [Header("Settings & References")]
    public Character.CharacterData originalCharacterData;
    public Character.CharacterData characterData;

    public bool enemy;

    [Space]

    public Rigidbody2D rb;
    public BoxCollider2D col;
    public SpriteRenderer sr;
    public NavMeshAgent agent;

    [Space]

    public bool alive = true;
    public bool tiedToBattle = false;

    [Header("Tactics")]

    internal AI target;

    [Header("Visuals")]

    // User movement
    Vector2 startPosInPx = Vector2.zero;
    Vector2 totalDelta = Vector2.zero;
    public Vector2 userSetPosition = Vector2.zero;

    GameObject attackRing = null;
    GameObject moveLine = null;



    public AI FindClosestEnemy()
    {
        float dist = float.MaxValue;
        AI closest = null;
        List<AI> enemies = enemy ? EncounterManager.Instance.heroes : EncounterManager.Instance.enemies;

        foreach (AI ai in enemies)
        {
            if (ai != null && ai.alive)
            {
                float d = Vector2.Distance(ai.transform.position, transform.position);
                if (d < dist)
                {
                    dist = d;
                    closest = ai;
                }
            }
        }

        return closest;
    }

    public List<AI> EnemiesInRange(Vector2 pointToMeasureFrom)
    {
        List<AI> targets = new List<AI>();

        List<AI> enemies = enemy ? EncounterManager.Instance.heroes : EncounterManager.Instance.enemies;

        foreach (AI ai in enemies)
        {
            if (ai != null && ai.alive)
            {
                float d = Vector2.Distance(ai.transform.position, pointToMeasureFrom);
                if (d < characterData.stats.attackRange)
                {
                    targets.Add(ai);
                }
            }
        }

        return targets;
    }

    void IDamageable.AfflictDamage(int damage)
    {
        characterData.stats.currentHealth -= damage;
        if (characterData.stats.currentHealth <= 0)
        {
            EncounterManager.Instance.KillAI(this);
        }
    }



    // Player interaction & visuals
    public override void Enter(int fingerId)
    {
        if (!enemy)
        {
            foreach (Touch t in Input.touches)
            {
                if (t.fingerId == fingerId)
                {
                    startPosInPx = t.position;

                    // Attack ring
                    attackRing = EncounterManager.Instance.visualsPooler.Retrieve(0);

                    SpriteRenderer sr = attackRing.GetComponent<SpriteRenderer>();
                    Material m = sr.sharedMaterial;

                    //0.0335f is a measured constant for the shader
                    m.SetFloat("_Distance", characterData.stats.attackRange * 0.0335f);

                    // Move line
                    moveLine = EncounterManager.Instance.visualsPooler.Retrieve(1);

                    UpdateVisuals(t.position);

                    TimeManager.Instance.LowerTimeScale(GetHashCode(), 0.1f);

                    break;
                }
            }


        }
    }

    public override void Stay(Vector2 delta)
    {
        if (!enemy)
        {
            // Update drawn attack ring & line
            totalDelta += delta;

            UpdateVisuals(startPosInPx + totalDelta);



            // Tag all enemies that are in range
            foreach (AI ai in (enemy ? EncounterManager.Instance.heroes : EncounterManager.Instance.enemies))
            {
                if(ai.alive)
                {
                    ai.sr.sharedMaterial.SetFloat("_Targeted", 0f);
                }
            }

            foreach (AI ai in EnemiesInRange(InputManager.Instance.sceneCamera.ScreenToWorldPoint(startPosInPx + totalDelta)))
            {
                if(ai.alive)
                {
                    ai.sr.sharedMaterial.SetFloat("_Targeted", 1f);
                }
            }
        }
    }

    public override void Complete()
    {
        if (!enemy)
        {
            Vector3 point = InputManager.Instance.sceneCamera.ScreenToWorldPoint(startPosInPx + totalDelta);
            userSetPosition = point;

            // Return visuals
            EncounterManager.Instance.visualsPooler.Return(attackRing.gameObject);
            attackRing = null;

            EncounterManager.Instance.visualsPooler.Return(moveLine.gameObject);
            moveLine = null;

            // Reset params
            totalDelta = Vector2.zero;
            startPosInPx = Vector2.zero;

            // Remove Targeted tag
            foreach (AI ai in (enemy ? EncounterManager.Instance.heroes : EncounterManager.Instance.enemies))
            {
                if(ai.alive)
                {
                    ai.sr.sharedMaterial.SetFloat("_Targeted", 0f);
                }
            }

            TimeManager.Instance.ReturnToNormal(GetHashCode());
        }
    }

    public override void Interrupt() { }

    void UpdateVisuals(Vector2 touchPosition)
    {
        // Attack ring
        Vector2 pos = InputManager.Instance.sceneCamera.ScreenToWorldPoint(touchPosition);
        attackRing.transform.position = pos;

        // Move line
        moveLine.transform.position = (transform.position.ToV2() + pos) / 2f;
        moveLine.transform.localScale = new Vector2(Vector2.Distance(transform.position, pos) * InputManager.Instance.sceneCanvasScaler.referencePixelsPerUnit, 1f);
        moveLine.transform.rotation = Quaternion.Euler(0, 0, Extensions.LookAt(pos, transform.position) + 90f);
    }
}

public interface IDamageable
{
    public void AfflictDamage(int damage);
}
