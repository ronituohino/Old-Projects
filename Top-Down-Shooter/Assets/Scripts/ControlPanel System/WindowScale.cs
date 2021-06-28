using UnityEngine;

class WindowScale : MonoBehaviour, IDraggable, IHoverable
{
    Window window;

    RectTransform windowTransform;
    Vector2 originalMousePos;

    Vector2 originalPanelPos;
    Vector2 originalSize;

    bool hovering = false;

    public enum Scaling
    {
        TOPR,
        BOTR,
        TOPL,
        BOTL,
        R,
        B,
        L,
        T
    }

    public Scaling scaling;

    private void Awake()
    {
        windowTransform = transform.parent.parent.GetComponent<RectTransform>();
        window = windowTransform.GetComponent<Window>();
    }

    void IDraggable.OnDrag(Vector2 movement)
    {
        if(window.scalable)
        {
            Vector2 diff = InputManager.Instance.mouseInScreen - originalMousePos;

            if (window.scaleStep == Vector2.zero)
            {
                SetNewScale(diff);
            }
            else
            {
                Vector2 steps = diff / window.scaleStep;
                SetNewScale(new Vector2(Mathf.FloorToInt(steps.x) * window.scaleStep.x, Mathf.FloorToInt(steps.y) * window.scaleStep.y));
            }
        }
    }

    void SetNewScale(Vector2 diff)
    {
        bool xPosition = false;
        bool yPosition = false;

        bool ignoreX = false;
        bool ignoreY = false;

        if (scaling == Scaling.TOPR)
        {
            xPosition = true;
            yPosition = true;
        }
        else if (scaling == Scaling.BOTR)
        {
            xPosition = true;
            yPosition = false;
        }
        else if (scaling == Scaling.TOPL)
        {
            xPosition = false;
            yPosition = true;
        }
        else if (scaling == Scaling.BOTL)
        {
            xPosition = false;
            yPosition = false;
        }
        else if (scaling == Scaling.R)
        {
            xPosition = true;

            ignoreY = true;
        }
        else if (scaling == Scaling.B)
        {
            yPosition = false;

            ignoreX = true;
        }
        else if (scaling == Scaling.L)
        {
            xPosition = false;

            ignoreY = true;
        }
        else if (scaling == Scaling.T)
        {
            yPosition = true;

            ignoreX = true;
        }

        //Calculate new values
        Vector2 newSize = new Vector2
            (
                ignoreX ? originalSize.x : (xPosition ? originalSize.x + diff.x : originalSize.x - diff.x),
                ignoreY ? originalSize.y : (yPosition ? originalSize.y + diff.y : originalSize.y - diff.y)
            );

        bool validX = false;
        bool validY = false;

        if (newSize.x > window.minSize.x)
        {
            validX = true;
        }
        if (newSize.y > window.minSize.y)
        {
            validY = true;
        }

        //Get and update settings
        Vector2 location;
        Vector2 scale;

        Vector2 maxDiff = originalSize - window.minSize;

        if (validX && validY)
        {
            windowTransform.sizeDelta = newSize;
            scale = newSize / window.defaultSize;

            location = new Vector2(ignoreX ? originalPanelPos.x : (originalPanelPos.x + (diff.x * 0.5f)),
                                      ignoreY ? originalPanelPos.y : (originalPanelPos.y + (diff.y * 0.5f)));
        }
        else if (validX && !validY)
        {
            Vector2 v = new Vector2(newSize.x, window.minSize.y);
            windowTransform.sizeDelta = v;
            scale = v / window.defaultSize;

            location = new Vector2(ignoreX ? originalPanelPos.x : (originalPanelPos.x + (diff.x * 0.5f)),
                                      ignoreY ? originalPanelPos.y : (originalPanelPos.y + (maxDiff.y * 0.5f * (yPosition ? -1 : 1))));
        }
        else if (!validX && validY)
        {
            Vector2 v = new Vector2(window.minSize.x, newSize.y);
            windowTransform.sizeDelta = v;
            scale = v / window.defaultSize;

            location = new Vector2(ignoreX ? originalPanelPos.x : (originalPanelPos.x + (maxDiff.x * 0.5f * (xPosition ? -1 : 1))),
                                      ignoreY ? originalPanelPos.y : (originalPanelPos.y + (diff.y * 0.5f)));
        }
        else
        {
            windowTransform.sizeDelta = window.minSize;
            scale = window.minSize / window.defaultSize;

            location = new Vector2(ignoreX ? originalPanelPos.x : (originalPanelPos.x + (maxDiff.x * 0.5f * (xPosition ? -1 : 1))),
                                      ignoreY ? originalPanelPos.y : (originalPanelPos.y + (maxDiff.y * 0.5f * (yPosition ? -1 : 1))));
        }

        window.WindowLocation = location;
        window.WindowScale = scale;
    }

    void IHoverable.OnHoverExit()
    {
        CursorManager.Instance.UpdateCursor(CursorManager.CursorStyle.Normal, false);
        hovering = false;
    }

    void IDraggable.OnDragPress()
    {
        if(window.scalable)
        {
            window.MakeWindowOnTop();

            CursorManager.Instance.lockCursorTexture = true;

            originalMousePos = Input.mousePosition.ToVector2();
            originalPanelPos = windowTransform.anchoredPosition;

            //Get settings
            originalSize = window.defaultSize * window.WindowScale;
        }
    }

    void IHoverable.OnHoverEnter()
    {
        hovering = true;

        if(window.scalable)
        {
            if (scaling == Scaling.BOTR || scaling == Scaling.TOPL)
            {
                CursorManager.Instance.UpdateCursor(CursorManager.CursorStyle.CornerScale, false);
            }
            else if (scaling == Scaling.BOTL || scaling == Scaling.TOPR)
            {
                CursorManager.Instance.UpdateCursor(CursorManager.CursorStyle.CornerScale, true);
            }
            else if (scaling == Scaling.B || scaling == Scaling.T)
            {
                CursorManager.Instance.UpdateCursor(CursorManager.CursorStyle.SideScale, true);
            }
            else
            {
                CursorManager.Instance.UpdateCursor(CursorManager.CursorStyle.SideScale, false);
            }
        }
    }

    void IDraggable.OnDragRelease()
    {
        if(window.scalable)
        {
            CursorManager.Instance.lockCursorTexture = false;

            if(!hovering)
            {
                CursorManager.Instance.UpdateCursor(CursorManager.CursorStyle.Normal, false);
            }
        }
    }

    void IHoverable.OnHoverStay() { }
}

