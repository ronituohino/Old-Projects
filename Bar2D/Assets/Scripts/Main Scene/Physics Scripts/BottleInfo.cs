using System.Collections;
using TMPro;
using UnityEngine;
using System.Linq;

public class BottleInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI bottleName;
    [SerializeField] TextMeshProUGUI bottleType;
    [SerializeField] TextMeshProUGUI bottleFullness;

    [SerializeField] RectTransform underline;
    [SerializeField] RectTransform lineMaskRect;
    [SerializeField] RectTransform textMaskRect;

    BottlePhysics bottlePhysics;

    float length = 0f;
    float curLen = 0f;
    WaitForEndOfFrame wait;
    Coroutine maskCoroutine;

    public void Initialize(BottlePhysics bottlePhysics)
    {
        this.bottlePhysics = bottlePhysics;

        bottleName.text = bottlePhysics.bottle.name;
        bottleType.text = Bottle.CleanType(bottlePhysics.bottle.fluidType);
        bottleFullness.text = GetFullnessText();

        wait = new WaitForEndOfFrame();

        float[] lengths = new float[3]
        {
            bottleName.GetPreferredValues().x,
            bottleType.GetPreferredValues().x,
            bottleFullness.GetPreferredValues().x
        };

        length = lengths.Max();

        underline.offsetMax = new Vector2(length + 1, underline.offsetMax.y);
        length += 6 + GlobalReferencesAndSettings.Instance.textMaskOffset;

        maskCoroutine = StartCoroutine(SetMasks(true, length));
    }

    public void Return()
    {
        if(maskCoroutine != null)
        {
            StopCoroutine(maskCoroutine);
        }

        maskCoroutine = StartCoroutine(SetMasks(true, length));
    }

    public void Delete()
    {
        if(maskCoroutine != null)
        {
            StopCoroutine(maskCoroutine);
        }

        maskCoroutine = StartCoroutine(SetMasks(false, length));
    }

    IEnumerator SetMasks(bool open, float length)
    {
        bool complete = false;

        while(!complete)
        {
            if (open)
            {
                curLen += GlobalReferencesAndSettings.Instance.infoOpenSpeedMultiplier;
            }
            else
            {
                curLen -= GlobalReferencesAndSettings.Instance.infoOpenSpeedMultiplier;
            }

            float antiOffset = Mathf.Clamp(curLen, 0f, length - GlobalReferencesAndSettings.Instance.textMaskOffset);
            lineMaskRect.sizeDelta = new Vector2(antiOffset * 2, 26f);

            float withOffset = Mathf.Clamp(curLen - GlobalReferencesAndSettings.Instance.textMaskOffset, 0f, length - GlobalReferencesAndSettings.Instance.textMaskOffset);
            textMaskRect.offsetMax = new Vector2(withOffset, textMaskRect.offsetMax.y);

            if (open)
            {
                if(curLen >= length)
                {
                    curLen = length;
                    complete = true;
                }
            }
            else
            {
                if(curLen <= 0f)
                {
                    curLen = length;
                    complete = true;
                }
            }

            yield return wait;
        }

        if(!open)
        {
            bottlePhysics.shownInfo = null;
            InputManager.Instance.sceneCanvas.RemoveElement(bottlePhysics.transform);
        }
    }

    string GetFullnessText()
    {
        float fullness = (float)bottlePhysics.fluidContained / (float)bottlePhysics.bottle.fluidCapacity;
        string txt = "";

        foreach (GlobalReferencesAndSettings.FullnessText ft in GlobalReferencesAndSettings.Instance.fullnessTexts)
        {
            if (fullness >= ft.fullness)
            {
                txt = ft.text;
                break;
            }
        }

        return txt;
    }

    //public void UpdateTexts()
    //{
    //    bottleFullness.text = GetFullnessText();
    //}

    // Slide mask to show information, looks nice
}