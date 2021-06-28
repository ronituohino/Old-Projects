using UnityEngine;

public class ScrollBarFade : MonoBehaviour, IHoverable
{
    [SerializeField] CanvasGroup scrollBarGroup;
    [SerializeField] float fadeMultiplier;

    public bool lockFade = false;

    bool hovering = false;
    bool fadingDone = false;

    void IHoverable.OnHoverEnter()
    {
        hovering = true;
        fadingDone = false;
    }

    void IHoverable.OnHoverExit()
    {
        hovering = false;
        fadingDone = false;
    }

    void IHoverable.OnHoverStay() { }

    private void Update()
    {
        if((hovering && !fadingDone) || lockFade)
        {
            scrollBarGroup.alpha += Time.deltaTime * fadeMultiplier;
            if(scrollBarGroup.alpha > 1f)
            {
                scrollBarGroup.alpha = 1f;
                fadingDone = true;
            }
        } 
        else if(!fadingDone)
        {
            scrollBarGroup.alpha -= Time.deltaTime * fadeMultiplier;
            if (scrollBarGroup.alpha < 0.15f)
            {
                scrollBarGroup.alpha = 0.15f;
                fadingDone = true;
            }
        }
    }
}