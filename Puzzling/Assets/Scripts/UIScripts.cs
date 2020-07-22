using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using System.Linq;

public class UIScripts : MonoBehaviour
{
    public PostProcessProfile postProcessProfile;
    private DepthOfField depthOfField;

    bool inMenu = false;
    public GameObject pauseScreen;
    public CanvasGroup pauseScreenGroup;

    public Image shadow;

    public GameObject pointer;
    public CanvasGroup pointerGroup;

    bool changing = false;
    float trans = 0f;
    public float pauseMenuSpeed = 1f;
    public AnimationCurve pauseMenuFade;


    [Header("Pointer Stuff")]

    public float pointerFadeChangeSpeed = 4f;
    public AnimationCurve fadeChangeCurve;

    [Space]

    public float normalVisibility = 0.1f;
    public float objectVisibility = 0.9f;
    

    float curPointerVis = 0f;

    [Space]

    public float pointerScaleChangeSpeed = 4f;
    public AnimationCurve scaleChangeCurve;

    public float normalScale = 1f;
    public float pinchedScale = 0.5f;

    [Space]

    public float completeFadeSpeed = 1f;
    public float completeFadeVisibility = 0f;
    public float completeFadeWaitTime = 5f;

    [Space]

    public float shadowNormalVisibility = 1f;
    public float shadowObjectVisibility = 0.75f;
    public float completeFadeShadowVisibility = 0.5f;

    float curShadowVis;


    float curPointerScale = 1f;
    float secondsTillCompleteFade = 0f;
    bool completeFade = false;

    [Header("Cursor settings")]
    public Texture2D texture;

    private void Start()
    {
        Cursor.SetCursor(texture, new Vector2(texture.width / 2f, texture.height / 2f), CursorMode.ForceSoftware);

        postProcessProfile.TryGetSettings(out depthOfField);
        depthOfField.focalLength.value = 1f;
        depthOfField.focusDistance.value = 1000f;
        depthOfField.aperture.value = 32f;
        depthOfField.kernelSize.value = KernelSize.Large;
        depthOfField.active = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) //Pause screen
        {
            inMenu = !inMenu;
            pointer.SetActive(!inMenu);

            if (inMenu)
            {
                depthOfField.active = true;
            }
            
            Cursor.visible = inMenu;

            PlayerControls.disconnectedMouse = inMenu;

            if (inMenu)
            {
                Cursor.lockState = CursorLockMode.None;
            } else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            changing = true;
        }

        ApplyPauseMenuChanges();
    }

    //Transitions to/from pause
    void ApplyPauseMenuChanges()
    {
        if (changing)
        {
            trans += (inMenu ? pauseMenuSpeed * Time.unscaledDeltaTime : -pauseMenuSpeed * Time.unscaledDeltaTime);

            if (trans > 1)
            {
                trans = 1;
            } else if(trans < 0)
            {
                trans = 0;
            }

            //Pause screen fade in/out
            pauseScreenGroup.alpha = pauseMenuFade.Evaluate(trans);

            //Time slowdown
            Time.timeScale = 1 - pauseMenuFade.Evaluate(trans);

            depthOfField.focalLength.value = pauseMenuFade.Evaluate(trans).Map(0, 1, 1, 300);

            //Absolute state locks
            if(inMenu && trans >= 1f)
            {
                changing = false;
                trans = 1f;

                pauseScreenGroup.alpha = 1f;
                Time.timeScale = 0f;
                if (!inMenu)
                {
                    depthOfField.active = false;
                }
            }
            else if(!inMenu && trans <= 0f)
            {
                changing = false;
                trans = 0f;

                pauseScreenGroup.alpha = 0f;
                Time.timeScale = 1f;
                if (!inMenu)
                {
                    depthOfField.active = false;
                }
            }
        }
    }

    //Transitions the pointer from one state to another, depending on what we are looking at
    public void PointerTransitions(ObjectClass objectClass)
    {
        //Pointer pinch effect
        if (PlayerControls.hoveringStuff || PlayerControls.grabbingStuff)
        {
            if(curPointerScale > pinchedScale)
            {
                curPointerScale -= pointerScaleChangeSpeed * Time.deltaTime;
            }
        }
        else
        {
            if (curPointerScale < normalScale)
            {
                curPointerScale += pointerScaleChangeSpeed * Time.deltaTime;
            }
        }
        float val = scaleChangeCurve.Evaluate(curPointerScale);
        pointer.transform.localScale = new Vector3(val, val, val);

        switch (objectClass)
        {
            case ObjectClass.Pickable:
                completeFade = false;
                secondsTillCompleteFade = 0f;

                //Fade pointer
                if (!ValuesClose(curPointerVis, objectVisibility))
                {
                    if (curPointerVis < objectVisibility)
                    {
                        curPointerVis += pointerFadeChangeSpeed * Time.deltaTime;
                        if (curPointerVis > objectVisibility)
                        {
                            curPointerVis = objectVisibility;
                        }
                    }
                    else
                    {
                        curPointerVis -= pointerFadeChangeSpeed * Time.deltaTime;
                        if (curPointerVis < objectVisibility)
                        {
                            curPointerVis = objectVisibility;
                        }
                    }
                    pointerGroup.alpha = fadeChangeCurve.Evaluate(curPointerVis);
                }

                //Fade shadow
                if (!ValuesClose(curShadowVis, shadowObjectVisibility))
                {
                    if (curShadowVis < shadowObjectVisibility)
                    {
                        curShadowVis += pointerFadeChangeSpeed * Time.deltaTime;
                        if (curShadowVis > shadowObjectVisibility)
                        {
                            curShadowVis = shadowObjectVisibility;
                        }
                    }
                    else
                    {
                        curShadowVis -= pointerFadeChangeSpeed * Time.deltaTime;
                        if (curShadowVis < shadowObjectVisibility)
                        {
                            curShadowVis = shadowObjectVisibility;
                        }
                    }
                    shadow.color = new Color(shadow.color.r, shadow.color.g, shadow.color.b, fadeChangeCurve.Evaluate(curShadowVis));
                }
                break;
            case ObjectClass.Grabbable:
                completeFade = false;
                secondsTillCompleteFade = 0f;

                //Fade pointer
                if (!ValuesClose(curPointerVis, objectVisibility))
                {
                    if (curPointerVis < objectVisibility)
                    {
                        curPointerVis += pointerFadeChangeSpeed * Time.deltaTime;
                        if (curPointerVis > objectVisibility)
                        {
                            curPointerVis = objectVisibility;
                        }
                    }
                    else
                    {
                        curPointerVis -= pointerFadeChangeSpeed * Time.deltaTime;
                        if (curPointerVis < objectVisibility)
                        {
                            curPointerVis = objectVisibility;
                        }
                    }
                    pointerGroup.alpha = fadeChangeCurve.Evaluate(curPointerVis);
                }

                //Fade shadow
                if (!ValuesClose(curShadowVis, shadowObjectVisibility))
                {
                    if (curShadowVis < shadowObjectVisibility)
                    {
                        curShadowVis += pointerFadeChangeSpeed * Time.deltaTime;
                        if (curShadowVis > shadowObjectVisibility)
                        {
                            curShadowVis = shadowObjectVisibility;
                        }
                    }
                    else
                    {
                        curShadowVis -= pointerFadeChangeSpeed * Time.deltaTime;
                        if (curShadowVis < shadowObjectVisibility)
                        {
                            curShadowVis = shadowObjectVisibility;
                        }
                    }
                    shadow.color = new Color(shadow.color.r, shadow.color.g, shadow.color.b, fadeChangeCurve.Evaluate(curShadowVis));
                }
                break;

            case ObjectClass.Interactable:
                completeFade = false;
                secondsTillCompleteFade = 0f;

                //Fade pointer
                if (!ValuesClose(curPointerVis, objectVisibility))
                {
                    if (curPointerVis < objectVisibility)
                    {
                        curPointerVis += pointerFadeChangeSpeed * Time.deltaTime;
                        if (curPointerVis > objectVisibility)
                        {
                            curPointerVis = objectVisibility;
                        }
                    }
                    else
                    {
                        curPointerVis -= pointerFadeChangeSpeed * Time.deltaTime;
                        if (curPointerVis < objectVisibility)
                        {
                            curPointerVis = objectVisibility;
                        }
                    }
                    pointerGroup.alpha = fadeChangeCurve.Evaluate(curPointerVis);
                }

                //Fade shadow
                if (!ValuesClose(curShadowVis, shadowObjectVisibility))
                {
                    if (curShadowVis < shadowObjectVisibility)
                    {
                        curShadowVis += pointerFadeChangeSpeed * Time.deltaTime;
                        if (curShadowVis > shadowObjectVisibility)
                        {
                            curShadowVis = shadowObjectVisibility;
                        }
                    }
                    else
                    {
                        curShadowVis -= pointerFadeChangeSpeed * Time.deltaTime;
                        if (curShadowVis < shadowObjectVisibility)
                        {
                            curShadowVis = shadowObjectVisibility;
                        }
                    }
                    shadow.color = new Color(shadow.color.r, shadow.color.g, shadow.color.b, fadeChangeCurve.Evaluate(curShadowVis));
                }
                break;
            case ObjectClass.None:
                //Fade pointer
                if(!ValuesClose(curPointerVis, normalVisibility) && !completeFade)
                {
                    if (curPointerVis < normalVisibility)
                    {
                        curPointerVis += pointerFadeChangeSpeed * Time.deltaTime;
                        if(curPointerVis > normalVisibility)
                        {
                            curPointerVis = normalVisibility;
                        }
                    }
                    else
                    {
                        curPointerVis -= pointerFadeChangeSpeed * Time.deltaTime;
                        if (curPointerVis < normalVisibility)
                        {
                            curPointerVis = normalVisibility;
                        }
                    }
                    pointerGroup.alpha = fadeChangeCurve.Evaluate(curPointerVis);
                }
                else 
                {
                    secondsTillCompleteFade += Time.deltaTime;
                    if(secondsTillCompleteFade >= completeFadeWaitTime)
                    {
                        completeFade = true;

                        curPointerVis -= completeFadeSpeed * Time.deltaTime;
                        pointerGroup.alpha = fadeChangeCurve.Evaluate(curPointerVis);
                        if(curPointerVis <= completeFadeVisibility)
                        {
                            curPointerVis = completeFadeVisibility;
                            secondsTillCompleteFade = 0f;
                        }
                    }
                }

                //Fade shadow
                if (!ValuesClose(curShadowVis, shadowNormalVisibility) && !completeFade)
                {
                    if (curShadowVis < shadowNormalVisibility)
                    {
                        curShadowVis += pointerFadeChangeSpeed * Time.deltaTime;
                        if (curShadowVis > shadowNormalVisibility)
                        {
                            curShadowVis = shadowNormalVisibility;
                        }
                    }
                    else
                    {
                        curShadowVis -= pointerFadeChangeSpeed * Time.deltaTime;
                        if (curShadowVis < shadowNormalVisibility)
                        {
                            curShadowVis = shadowNormalVisibility;
                        }
                    }
                    shadow.color = new Color(shadow.color.r, shadow.color.g, shadow.color.b, fadeChangeCurve.Evaluate(curShadowVis));
                } else
                {
                    curShadowVis -= completeFadeSpeed * Time.deltaTime;
                    shadow.color = new Color(shadow.color.r, shadow.color.g, shadow.color.b, fadeChangeCurve.Evaluate(curShadowVis));
                    if(curShadowVis < completeFadeShadowVisibility)
                    {
                        curShadowVis = completeFadeShadowVisibility;
                    }
                }
                break;
        }
    }

    private bool ValuesClose(float a, float b)
    {
        if(Mathf.Round(a * 50) == Mathf.Round(b * 50))
        {
            return true;
        } else
        {
            return false;
        }
    }
}
