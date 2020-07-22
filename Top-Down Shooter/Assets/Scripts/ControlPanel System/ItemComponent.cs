//Component attached to items that have UI behaviour, these talk with ItemSlots to manipulate InventoryData
using UnityEngine;

public class ItemComponent : MonoBehaviour, IHoverable
{
    public RectTransform rect;
    public ItemInfo info;

    void IHoverable.OnHover()
    {

    }

    void IHoverable.OnHoverEnter()
    {

    }

    void IHoverable.OnHoverExit()
    {

    }
}