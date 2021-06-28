using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ScrollBarHandle : MonoBehaviour, IHoverable, ILeftDraggable
{
    RectTransform currentSiteRect;

    [SerializeField] ColorChangeDriver colorChangeDriver;
    [SerializeField] ScrollBarFade fade;

    [SerializeField] RectTransform scrollHandle;
    [SerializeField] RectTransform topBar;

    bool dragging = false;
    [SerializeField] float smoothDamp;

    [SerializeField] float mouseMovementMultiplier;
    [SerializeField] float scrollMovementMultiplier;
    float previousMovement;

    float pageHeight;
    float scrollBarHeight;

    private void Start()
    {
        //scrollBarHeight = (GlobalReferences.Instance.canvasScaleRect.sizeDelta.y - topBar.sizeDelta.y);
    }

    void ILeftDraggable.OnDragPress() 
    {
        dragging = true;
        fade.lockFade = true;
    }

    void ILeftDraggable.OnDrag()
    {
        //MoveScrollBar((movement.y - previousMovement) * mouseMovementMultiplier, -1f);
        //previousMovement = movement.y;
    }

    void ILeftDraggable.OnDragRelease() 
    {
        dragging = false;
        fade.lockFade = false;
        colorChangeDriver.SetHover(false);
        previousMovement = 0f;
    }

    private void Update()
    {
        if(!dragging)
        {
            //MoveScrollBar(InputManager.Instance.mouseScroll * scrollMovementMultiplier, -1f);
        }
    }

    public void SetPageAndHandle(float percentagePageScrolled)
    {
        ResizeHandle();
        MoveScrollBar(0f, percentagePageScrolled);
    }

    void ResizeHandle()
    {
        scrollHandle.anchoredPosition = new Vector2(0, 0);

        currentSiteRect = ComputerBrowser.Instance.currentTab.currentWebsite.siteObject.GetComponent<RectTransform>();
        pageHeight = currentSiteRect.sizeDelta.y / scrollBarHeight;

        scrollHandle.sizeDelta = new Vector2(0, scrollBarHeight / pageHeight);
    }

    void MoveScrollBar(float movement, float percentagePageScrolled)
    {
        bool hasMovement = movement != 0f;
        bool directSet = percentagePageScrolled != -1f; //Use -1, because 0 could mean "place the page at the top"

        if (hasMovement || directSet)
        {
            Vector2 target = Vector2.zero;
            if (hasMovement)
            {
                target = new Vector2(scrollHandle.anchoredPosition.x, scrollHandle.anchoredPosition.y + movement * scrollBarHeight);
            }
            else if(directSet)
            {
                Vector2 directTarget = new Vector2(0, percentagePageScrolled * (scrollBarHeight - scrollHandle.sizeDelta.y));
                target = -directTarget;
            }

            //Site thresholds, the scrollHandle can't come out of the side bar
            float threshold = scrollBarHeight - scrollHandle.sizeDelta.y;
            if (target.y > 0)
            {
                target = new Vector2(0, 0);
            }
            else if (target.y < -threshold)
            {
                target = new Vector2(0, -threshold);
            }

            scrollHandle.anchoredPosition = target;

            //Exception for when the site is fully shown at no scroll
            if (Mathf.RoundToInt(scrollHandle.sizeDelta.y) == Mathf.RoundToInt(scrollBarHeight))
            {
                SetPagePosition(0f);
            }
            else
            {
                SetPagePosition(-target.y / (scrollBarHeight - scrollHandle.sizeDelta.y));
            }
        }
    }

    void SetPagePosition(float percentage)
    {
        currentSiteRect.anchoredPosition = new Vector2(0, percentage * (currentSiteRect.sizeDelta.y - scrollBarHeight));
        ComputerBrowser.Instance.currentTab.percentagePageScrolled = percentage;
    }

    void IHoverable.OnHoverEnter()
    {
        colorChangeDriver.SetHover(true);
    }

    void IHoverable.OnHoverExit()
    {
        if(!dragging)
        {
            colorChangeDriver.SetHover(false);
        }
    }

    void IHoverable.OnHoverStay() { }
}