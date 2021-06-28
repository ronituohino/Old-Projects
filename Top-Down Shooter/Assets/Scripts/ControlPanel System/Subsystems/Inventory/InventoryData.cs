using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Stores items
[System.Serializable]
public class InventoryData
{
    public List<ItemInfo> storedItems = new List<ItemInfo>();

    //Leave empty to allow all
    public Item.ItemType[] itemsAllowed;

    [System.Serializable]
    public class ItemInfo
    {
        public Item item;
        public ItemData itemData;
        public Vector2 pos;
        public int amount;

        public ItemInfo(Item item, ItemData itemData, Vector2 pos, int amount)
        {
            this.item = item;
            this.itemData = itemData;
            this.pos = pos;
            this.amount = amount;
        }
    }

    public void AddItem(ItemInfo itemInfo)
    {
        if (itemsAllowed == null || itemsAllowed.Length == 0 || itemsAllowed.Contains(itemInfo.item.itemType))
        {
            storedItems.Add(itemInfo);
        }
    }

    public void RemoveItem(Vector2 pos)
    {
        foreach (ItemInfo itemInfo in storedItems)
        {
            if (itemInfo.pos.Equals(pos))
            {
                storedItems.Remove(itemInfo);
                return;
            }
        }
        Debug.Log("Error removing item!");
        return;
    }

    public void ClearItems()
    {
        storedItems.Clear();
    }

    public ItemInfo GetItemInfo(Vector2 pos)
    {
        foreach (ItemInfo itemInfo in storedItems)
        {
            if (itemInfo.pos.Equals(pos))
            {
                return itemInfo;
            }
        }

        return null;
    }

    public void CombineContainers(InventoryData containerData)
    {
        foreach (ItemInfo itemInfo in containerData.storedItems)
        {
            AddItem(itemInfo);
        }
    }
}


