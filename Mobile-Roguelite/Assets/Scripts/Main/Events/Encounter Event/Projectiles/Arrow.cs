using System.Collections;
using UnityEngine;

public class Arrow : Projectile
{
    internal override void Hit(AI target)
    {
        if(target != null)
        {
            ((IDamageable)target).AfflictDamage(damage);
        }

        EncounterManager.Instance.projectilePooler.Return(gameObject);
    }
}