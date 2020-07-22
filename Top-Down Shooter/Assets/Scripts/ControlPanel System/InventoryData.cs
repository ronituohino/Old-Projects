using System;
using System.Collections.Generic;
using UnityEngine;

//Stores items
[System.Serializable]
public class InventoryData
{
    public List<ItemInfo> storedItems = new List<ItemInfo>();

    public ItemInfo AddItem(Item i, ItemData itemData, int index)
    {
        if (Occupied(index))
        {
            return null;
        }
        else
        {
            ItemInfo info = new ItemInfo(i, itemData, index);
            storedItems.Add(info);
            return info;
        }
    }

    public bool AddItem(ItemInfo info, int index)
    {
        if (Occupied(index))
        {
            return false;
        }
        else
        {
            info.index = index;
            storedItems.Add(info);
            return true;
        }
    }

    public void RemoveItem(int index)
    {
        foreach (ItemInfo itemInfo in storedItems)
        {
            if (itemInfo.index == index)
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

    public ItemInfo GetItemInfo(int index)
    {
        foreach (ItemInfo itemInfo in storedItems)
        {
            if (itemInfo.index == index)
            {
                return itemInfo;
            }
        }

        Debug.Log("Can't find item!");
        return null;
    }

    public bool Occupied(int index)
    {
        foreach (ItemInfo itemInfo in storedItems)
        {
            if (itemInfo.index == index)
            {
                return true;
            }
        }

        return false;
    }

    public void CombineContainers(InventoryData containerData)
    {
        foreach (ItemInfo itemInfo in containerData.storedItems)
        {
            ItemInfo info = AddItem(itemInfo.item, itemInfo.itemData, itemInfo.index);
        }
    }

    //Return a free spot for an item
    public int GetFreePositionInContainer()
    {
        int index = 0;
        while (true)
        {
            if (!Occupied(index))
            {
                return index;
            }
            index++;
        }
    }
}


