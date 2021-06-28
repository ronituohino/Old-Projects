using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class Swing : MonoBehaviour
{
    [SerializeField] Animator anim;
    AI origin;
    AI target;

    public void SwingAtTarget(AI origin, AI target)
    {
        this.origin = origin;
        this.target = target;

        anim.Play("Swing", 0);
        StartCoroutine(Extensions.AnimationWait(anim, "Swing", ReturnSwingObject));
    }

    // Called from animation event!
    public void Hit()
    {
        ((IDamageable)target).AfflictDamage(origin.characterData.stats.attack);
    }

    public void ReturnSwingObject()
    {
        EncounterManager.Instance.visualsPooler.Return(gameObject);
    }
}