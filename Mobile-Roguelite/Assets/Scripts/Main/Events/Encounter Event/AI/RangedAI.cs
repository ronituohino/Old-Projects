using System.Collections;
using UnityEngine;

public class RangedAI : AI
{
    float targetTimer = 0f;
    float attackTimer = 0f;

    private void Start()
    {
        target = FindClosestEnemy();
        targetTimer = Random.Range(0, EncounterManager.Instance.targetUpdateInterval);
        attackTimer = Random.Range(0, characterData.stats.attackSpeed);
    }

    private void Update()
    {
        if (alive)
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
                    if (rb.position != userSetPosition)
                    {
                        agent.SetDestination(userSetPosition);
                    }
                }

                // Attack if close enough
                float distance = Vector2.Distance(target.transform.position, transform.position);
                if (distance < characterData.stats.attackRange)
                {
                    attackTimer += Time.deltaTime;
                    if (attackTimer > characterData.stats.attackSpeed)
                    {
                        Attack();
                        attackTimer = 0f;
                    }
                }
                else
                {
                    attackTimer = 0f;
                }
            }
        }
    }

    void Attack()
    {
        EncounterManager.Instance.SpawnProjectile
            (
                0,
                transform.position,
                target.transform.position,
                (target.transform.position - transform.position).normalized * characterData.projectileVelocity,
                enemy,
                characterData.stats.attack
            );
    }
}