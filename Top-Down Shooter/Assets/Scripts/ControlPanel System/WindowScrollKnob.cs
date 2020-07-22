using UnityEngine;

class WindowScrollKnob : MonoBehaviour, IDraggable
{
    [SerializeField] InventoryWindow container;
    
    Vector2 originalKnobPos;
    Vector2 originalPointInRect = Vector2.zero;

    void IDraggable.OnDrag()
    {
        //Set knob position
        Vector2 point = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(ControlPanel.Instance.controlPanelRect, Input.mousePosition.ToVector2(), ControlPanel.Instance.cam, out point);

        Vector2 diff = point - originalPointInRect;
        Vector2 newPos = new Vector2(0, originalKnobPos.y + diff.y);

        float bound = (container.windowTransform.sizeDelta.y - 35 - container.scrollKnob.sizeDelta.y) / 2f;

        if (newPos.y < -bound)
        {
            newPos = new Vector2(0, -bound);
        }
        else if (newPos.y > bound)
        {
            newPos = new Vector2(0, bound);
        }

        container.scrollKnob.anchoredPosition = newPos;

        //Set slots position
        float maxY = (container.ySlots - Mathf.FloorToInt(container.windowTransform.sizeDelta.y / ControlPanel.Instance.slotSize)) * ControlPanel.Instance.slotSize;
        float yCoord = Extensions.Remap(newPos.y, -bound, bound, maxY, 0);

        container.inventoryScroll.anchoredPosition = new Vector2(0, yCoord);
    }

    void IDraggable.OnPress()
    {
        Vector2 point = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(ControlPanel.Instance.controlPanelRect, Input.mousePosition.ToVector2(), ControlPanel.Instance.cam, out point);
        originalPointInRect = point;

        originalKnobPos = container.scrollKnob.anchoredPosition;

        container.MakeWindowOnTop();
    }

    void IDraggable.OnRelease() { }
}