using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : Singleton<FadeManager>
{
    public Animator animator;

    public void Fade(bool fadeIn)
    {
        animator.SetBool("Fade", fadeIn);
    }
}