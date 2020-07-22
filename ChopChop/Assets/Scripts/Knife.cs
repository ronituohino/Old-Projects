using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : Singleton<Knife> {

    public bool ableToCut = true;

    public Animator anim;
    AnimatorClipInfo currentClipInfo;

    //Cuts with the knife
    public void Cut()
    {
        if (ableToCut)
        {
            currentClipInfo = anim.GetCurrentAnimatorClipInfo(0)[0];
            if (currentClipInfo.clip.name == "Up")
            {
                anim.SetTrigger("Cut");
            }
        }
    }

    //Called from the animation
    public void SplitFruit()
    {
        SlicingScript.Instance.SplitMesh();
    }
}