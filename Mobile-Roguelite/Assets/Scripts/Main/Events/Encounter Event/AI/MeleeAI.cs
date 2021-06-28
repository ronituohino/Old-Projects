using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAI : AI
{
    float targetTimer = 0f;
    public float attackTimer = 0f;

    private void Start()
    {
        target = FindClosestEnemy();
        targetTimer = Random.Range(0, EncounterManager.Instance.targetUpdateInterval);
    }

    private void Update()
    {
        if(alive)
        {
            // Update target every now and then
            targetTimer += Time.deltaTime;
            if (targetTimer >= EncounterManager.Instance.targetUpdateInterval)
            {
                target = FindClosestEnemy();
                targetTimer = 0f;
            }

            
            if (target == null || !target.gameObject.activeInHierarchy)
            {
                // If doesn't have target, find a target
                target = FindClosestEnemy();
            }
            else
            {
                // Movement
                if(enemy)
                {
                    // AI movement
                    Vector2 backVector = transform.position - target.transform.position;
                    Vector2 closePos = (backVector.normalized * (characterData.stats.attackRange * 0.9f)) + target.transform.position.ToV2();
                    agent.SetDestination(closePos);
                }
                else
                {
                    // User movement
                    if(rb.position != userSetPosition)
                    {
                        agent.SetDestination(userSetPosition);
                    }
                }
                

                // Attack if close enough
                float distance = Vector2.Distance(target.transform.position, transform.position);
                if (distance < characterData.stats.attackRange)
                {
                    TieTargetToBattle(target);

                    attackTimer += Time.deltaTime;
                    if (attackTimer > characterData.stats.attackSpeed)
                    {
                        Attack();
                        attackTimer = 0f;
                    }
                }
                else
                {
                    tiedToBattle = false;
                    attackTimer = characterData.stats.attackSpeed * 0.5f;
                }
            }
        }
    }

    public void TieToBattle()
    {

    }

    public void UntieFromBattle()
    {

    }

    // MeleeAI can tie enemies into battle, this halves both parties' movement speed
    // this prevents "teasing"
    public void TieTargetToBattle(AI target)
    {
        agent.SetDestination(transform.position);
        tiedToBattle = true;

        if (!target.tiedToBattle)
        {
            target.agent.SetDestination(target.transform.position);
            target.tiedToBattle = true;
            target.target = this;
        }
    }

    void Attack()
    {
        EncounterManager.Instance.SpawnSwing(this, target);
    }
}