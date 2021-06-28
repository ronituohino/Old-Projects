using UnityEngine;
using TMPro;
using System.Collections;
using System;
using UnityEngine.UI;

public class DialogueBubble : MonoBehaviour
{
    public TextMeshProUGUI text;
    public string bubbleText;
    string shownText = "";

    Vector2 textSize = Vector2.zero;

    [HideInInspector]
    public Transform npcTransform;

    [SerializeField] Animator thinkBubbleAnimator;
    [SerializeField] Animator cornerAnimator;

    [SerializeField] RectTransform bubbleRect;

    [SerializeField] Image bubbleImage;

    [SerializeField] float textBubbleLerp = 0.3f;
    [SerializeField] float charactersPerSecondAppear = 15f;
    [SerializeField] float charactersPerSecondDisappear = 150f;

    bool inTextBoxMode = false;
    Vector2 targetSize = new Vector2(9, 9);
    Coroutine txtBoxCoroutine;

    bool deleting = false;

    bool updateText = true;
    WaitForSeconds wfs1;
    WaitForSeconds wfs2;

    private void Start()
    {
        cornerAnimator.SetBool("Visible", true);
        thinkBubbleAnimator.SetBool("Visible", true);

        textSize = text.GetPreferredValues() + new Vector2(9, 5);

        text.text = "";
        wfs1 = new WaitForSeconds(1f / charactersPerSecondAppear);
        wfs2 = new WaitForSeconds(1f / charactersPerSecondDisappear);
        StartCoroutine(TextUpdater());

    }

    private void Update()
    {
        if(inTextBoxMode)
        {
            bubbleRect.offsetMax = Vector2.Lerp
                                        (
                                            bubbleRect.offsetMax, 
                                            targetSize, 
                                            textBubbleLerp
                                        );
        }
    }

    IEnumerator TextUpdater()
    {
        while (updateText)
        {
            // Shown text
            if (Extensions.RoundEqualsVector(bubbleRect.offsetMax, textSize, 2))
            {
                if (!shownText.Equals(bubbleText))
                {
                    char nextChar = bubbleText[shownText.Length];
                    shownText += nextChar;

                    text.text = shownText;
                }
                yield return wfs1;
            }
            else
            {
                if (!shownText.Equals(""))
                {
                    shownText = shownText.Substring(0, shownText.Length - 1);

                    text.text = shownText;
                }
                yield return wfs2;
            }
        }
    }

    public void SetHover(bool hovered)
    {
        if(!deleting)
        {
            thinkBubbleAnimator.SetBool("Hovered", hovered);

            if (hovered)
            {
                targetSize = textSize;
                txtBoxCoroutine = StartCoroutine(Extensions.AnimationWait(thinkBubbleAnimator, "BubbleToThink", ChangeToTextBox));
            }
            else
            {
                targetSize = new Vector2(9, 9);

                if (inTextBoxMode)
                {
                    txtBoxCoroutine = StartCoroutine(Extensions.ConditionWait(IsBoxScaledDown, ChangeToBubble));
                }
                else
                {
                    if (txtBoxCoroutine != null)
                    {
                        StopCoroutine(txtBoxCoroutine);
                        txtBoxCoroutine = null;
                    }
                }
            }
        }
    }

    bool IsBoxScaledDown()
    {
        return Extensions.RoundEqualsVector(bubbleRect.offsetMax, targetSize, 2);
    }

    void ChangeToTextBox()
    {
        bubbleImage.enabled = false;
        bubbleRect.gameObject.SetActive(true);

        inTextBoxMode = true;
    }

    void ChangeToBubble()
    {
        bubbleImage.enabled = true;
        thinkBubbleAnimator.SetBool("Visible", true);

        bubbleRect.gameObject.SetActive(false);

        inTextBoxMode = false;
    }


    // Deletion
    public void Delete()
    {
        StopAllCoroutines();

        SetHover(false);
        deleting = true;

        if (inTextBoxMode)
        {
            Func<bool> wait = () => !inTextBoxMode;
            StartCoroutine(Extensions.ConditionWait(wait, HideEverything));
        }
        else
        {
            HideEverything();
        }
    }

    void HideEverything()
    {
        updateText = false;

        cornerAnimator.SetBool("Visible", false);
        thinkBubbleAnimator.SetBool("Visible", false);

        StartCoroutine(Extensions.AnimationWait(thinkBubbleAnimator, "Empty", DeleteBubble));
    }

    void DeleteBubble()
    {
        InputManager.Instance.sceneCanvas.RemoveElement(npcTransform);
    }
}