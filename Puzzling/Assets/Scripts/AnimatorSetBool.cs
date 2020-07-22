using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSetBool : MonoBehaviour
{
    public Animator animator;
    public void SetBoolTrue(string name)
    {
        Debug.Log(name);
        animator.SetBool(name, true);
    }

    public void SetBoolFalse(string name)
    {
        animator.SetBool(name, false);
    }
}
