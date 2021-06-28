using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ItemContainer : MonoBehaviour
{
    public ItemObject storedItem = null;
    public bool canStoreItem = true;

    public RectTransform rect;
    public Image img;

    [Space]

    [SerializeField] Hero connectedHero;

    public void HoverWithItemEnter()
    {
        img.color = Color.red;
    }

    public void HoverWithItemExit()
    {
        img.color = Color.white;
    }

    public void Visible()
    {
        InputManager.Instance.visibleItemContainers.Add(this);
    }

    public void Hidden()
    {
        InputManager.Instance.visibleItemContainers.Remove(this);
    }

    public void SetItem(ItemObject itemObject)
    {
        storedItem = itemObject;

        if(connectedHero != null)
        {
            connectedHero.item = itemObject.item;
            connectedHero.UpdateHero();
        }
    }

    public void RemoveItem()
    {
        storedItem = null;

        if (connectedHero != null)
        {
            connectedHero.item = null;
            connectedHero.UpdateHero();
        }
    }
}