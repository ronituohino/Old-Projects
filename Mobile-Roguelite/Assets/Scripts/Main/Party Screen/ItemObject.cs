using System.Collections;
using UnityEngine;

public class ItemObject : Interactable
{
    public Item item;

    [HideInInspector]
    public int fingerId;
    public RectTransform rect;

    [Space]

    Vector2 totalDrag = Vector2.zero;
    bool dragging = false;
    Vector2 vel = Vector2.zero;

    public ItemContainer containerStoredIn = null;
    ItemContainer previousContainer;

    ItemContainer closestContainer = null;
    public ItemContainer closestAvailableContainer = null;

    public override void Enter(int fingerId)
    {
        this.fingerId = fingerId;

        if(MoreInfo.Instance.focusedObjectRect == rect)
        {
            MoreInfo.Instance.ResetParams();
        }
    }

    public override void Stay(Vector2 delta)
    {
        totalDrag += delta;

        if (!dragging && totalDrag.magnitude.IsDrag())
        {
            dragging = true;

            previousContainer = containerStoredIn;

            rect.SetParent(InputManager.Instance.sceneCanvas);

            containerStoredIn.HoverWithItemEnter();

            containerStoredIn.RemoveItem();
            containerStoredIn = null;
        }

        // Item dragging
        if (dragging)
        {
            Touch touch = new Touch();
            foreach (Touch t in Input.touches)
            {
                if (t.fingerId == fingerId)
                {
                    touch = t;
                }
            }

            Extensions.TranslateRect(rect, touch.position, ref vel, InputManager.Instance.dragSmoothTime);

            // Check if hovering over inventoryContainer
            ItemContainer closest = null;
            float dist = float.MaxValue;

            foreach (ItemContainer ic in InputManager.Instance.visibleItemContainers)
            {
                float d = Vector2.Distance(ic.rect.position, rect.position);
                if (d < dist)
                {
                    dist = d;
                    closest = ic;
                }
            }

            if (dist <= InputManager.Instance.containerHoverRange)
            {
                if (closest != closestContainer)
                {
                    if (closestContainer != null)
                    {
                        closestContainer.HoverWithItemExit();
                    }

                    closestContainer = closest;
                    if (closestContainer.storedItem == null && closestContainer.canStoreItem)
                    {
                        closestContainer.HoverWithItemEnter();
                        closestAvailableContainer = closestContainer;
                    }
                    else
                    {
                        closestAvailableContainer = null;
                    }
                }
            }
            else
            {
                if (closestContainer != null)
                {
                    closestContainer.HoverWithItemExit();
                    closestContainer = null;
                    closestAvailableContainer = null;
                }
            }
        }
    }



    public override void Interrupt()
    {
        if (dragging)
        {
            Complete();
        }
    }

    public override void Complete()
    {
        if (dragging)
        {
            // Drag, move object

            dragging = false;
            totalDrag = Vector2.zero;

            if (closestAvailableContainer != null)
            {
                containerStoredIn = closestAvailableContainer;
                containerStoredIn.SetItem(this);
            }
            else
            {
                containerStoredIn = previousContainer;
                containerStoredIn.SetItem(this);
            }

            containerStoredIn.HoverWithItemExit();

            rect.SetParent(containerStoredIn.rect);
            rect.anchoredPosition = Vector2.zero;
        }
        else
        {
            // Press, show item info

            MoreInfo.Instance.ShowMoreInfo(null, this);
        }
    }
}