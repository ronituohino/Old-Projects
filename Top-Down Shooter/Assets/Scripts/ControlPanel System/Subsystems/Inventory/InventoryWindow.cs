using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//UI inventory
[System.Serializable]
public class InventoryWindow : Window, IHoverable
{
    public InventoryData inventoryData;

    [SerializeField] List<ItemComponent> storedItems = new List<ItemComponent>();

    [SerializeField] TextMeshProUGUI capacityText;

    [Space]

    public RectTransform inventoryParent;

    [SerializeField] RectTransform capacityFill;
    [SerializeField] RectTransform capacityFillSlider;

    public int maxCapacity = 0;
    int capacityTaken = 0;


    //Events
    public delegate void ItemInteraction();
    public event ItemInteraction OnItemTakeOrPlace;


    new void Awake()
    {
        inventoryData = new InventoryData();

        OnItemTakeOrPlace += ItemAmountChange;
        OnScale += WindowScaleChange;

        UpdateCapacity();
        base.Awake();
    }

    void ItemAmountChange()
    {
        UpdateCapacity();
    }

    void WindowScaleChange()
    {
        UpdateCapacity();

        //Set items to their correct positions
        foreach(ItemComponent ic in storedItems)
        {
            ic.rect.anchoredPosition = GetAnchoredPosition(ic.info.pos);
        }
    }

    //Updates the capacity stuff
    public void UpdateCapacity()
    {
        int newCapacity = 0;
        foreach (InventoryData.ItemInfo info in inventoryData.storedItems)
        {
            newCapacity += info.item.inventorySpace;
        }
        capacityTaken = newCapacity;

        capacityText.text = capacityTaken.ToString() + "/" + maxCapacity.ToString();
        capacityFillSlider.offsetMax = new Vector2(-capacityFill.rect.width * Extensions.Remap(capacityTaken, 0, maxCapacity, 1, 0), 0); //new Vector2(-right, -top)
    }





    //Creates a new UI item object and adds it to inventory
    public void AddItem(InventoryData.ItemInfo itemInfo, ItemComponent comp) //Position is the 0,0 square position in container grid
    {
        inventoryData.AddItem(itemInfo);

        if(comp == null)
        {
            GameObject g = Instantiate(ControlPanel.Instance.item, inventoryParent);
            g.transform.localPosition = new Vector3(0, 0, 0);

            ItemComponent ic = g.GetComponent<ItemComponent>();
            storedItems.Add(ic);

            RectTransform rect = g.GetComponent<RectTransform>();

            ic.info = itemInfo;

            rect.sizeDelta = new Vector2(25, 25);
            rect.anchoredPosition = GetAnchoredPosition(new Vector2(0.5f, 0.5f));

            Image image = g.GetComponent<Image>();
            image.sprite = itemInfo.item.inventoryImage;
        } 
        else
        {
            storedItems.Add(comp);
        }

        OnItemTakeOrPlace?.Invoke();
    }

    public void RemoveItem(Vector2 position)
    {
        int count = storedItems.Count;
        for(int i = 0; i < count; i++)
        {
            ItemComponent ic = storedItems[i];
            if(ic.info.pos == position)
            {
                storedItems.RemoveAt(i);
                break;
            }
        }

        inventoryData.RemoveItem(position);

        OnItemTakeOrPlace?.Invoke();
    }

    public void ClearItems()
    {
        int count = storedItems.Count;
        for (int i = 0; i < count; i++)
        {
            ItemComponent ic = storedItems[i];

            GameObject g = ic.gameObject;
            Destroy(g);
        }

        inventoryData.ClearItems();

        OnItemTakeOrPlace?.Invoke();
    }

    public Vector2 GetAnchoredPosition(Vector2 relativePos)
    {
        Vector2 remappedPos = new Vector2(relativePos.x - 0.5f, relativePos.y - 0.5f);
        return new Vector2(inventoryParent.rect.width * remappedPos.x, inventoryParent.rect.height * remappedPos.y);
    }

    public Vector2 GetRelativePosition(Vector2 anchoredPosition) //anchoredPosition is local point in rect
    {
        return new Vector2(anchoredPosition.x / inventoryParent.rect.width + 0.5f, anchoredPosition.y / inventoryParent.rect.height + 0.5f);
    }

    public ItemComponent GetClosestItemInPosition(Vector2 pos)
    {
        ItemComponent itemComponent = null;
        float closestDist = float.MaxValue;

        foreach(ItemComponent ic in storedItems)
        {
            float dist = Vector2.Distance(ic.rect.anchoredPosition, pos);
            if(dist < closestDist)
            {
                closestDist = dist;
                itemComponent = ic;
            }
        }

        return itemComponent;
    }





    void IHoverable.OnHoverEnter() { }

    void IHoverable.OnHoverStay() { }

    void IHoverable.OnHoverExit() { }
}