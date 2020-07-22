using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChapterManager : MonoBehaviour
{
    [Header("References")]

    public TextMeshProUGUI topText;
    public TextMeshProUGUI bottomText;

    public Animator lineAnimator;
    public Animator topAnimator;
    public Animator bottomAnimator;

    public RectTransform line;
    public AnimationClip lineEntry;
    public AnimationClip lineExit;

    [Header("Chapter fade settings")]
    public CanvasGroup canvasGroup;
    public AnimationCurve animationCurve;
    float curFade = 0f;
    public float speed;

    [Space]

    public bool showingAnimation;
    public ChapterName curChapter;
    

    [HideInInspector]
    public float animationLength = 17f;

    [Space]

    public ChapterName[] chapters;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            StartTransition(0);
        }
        FadeChapter();
    }

    void FadeChapter()
    {
        if (showingAnimation && curFade < 1)
        {
            curFade += Time.deltaTime * speed;
            if (curFade > 1)
            {
                curFade = 1f;
                PlayAnimations(); //After we have faded in, play the animations
            }
        }
        else if(curFade > 0)
        {
            curFade -= Time.deltaTime * speed;
            if(curFade < 0)
            {
                curFade = 0f;
            }
        } 
        
        canvasGroup.alpha = animationCurve.Evaluate(curFade);
    }

    //chapterNum starts at 0!
    //Starts a chapter transition
    public void StartTransition(int chapterNum)
    {
        curChapter = chapters[chapterNum];
        speed = curChapter.transitionSpeed;
        animationLength = lineEntry.length + lineExit.length;

        //Starts the chapter fadeIn
        showingAnimation = true;
    }

    private void PlayAnimations()
    {
        topText.SetText("CHAPTER " + curChapter.chapterRomanNum.ToUpper());
        bottomText.SetText(curChapter.chapterName.ToUpper());

        lineAnimator.SetBool("LineEnter", true);
        lineAnimator.SetBool("LineExit", true);

        topAnimator.SetBool("TopEnter", true);
        topAnimator.SetBool("TopExit", true);

        bottomAnimator.SetBool("BottomEnter", true);
        bottomAnimator.SetBool("BottomExit", true);

        showingAnimation = true;
        StartCoroutine(StartTimer());
    }

    IEnumerator StartTimer()
    {
        yield return new WaitForSecondsRealtime(animationLength);

        lineAnimator.SetBool("LineEnter", false);
        lineAnimator.SetBool("LineExit", false);

        topAnimator.SetBool("TopEnter", false);
        topAnimator.SetBool("TopExit", false);

        bottomAnimator.SetBool("BottomEnter", false);
        bottomAnimator.SetBool("BottomExit", false);

        

        showingAnimation = false;
    }

    [System.Serializable]
    public class ChapterName
    {
        public string chapterRomanNum;
        public string chapterName;
        public float transitionSpeed;
    }
}
