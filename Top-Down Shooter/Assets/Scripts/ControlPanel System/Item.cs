using UnityEngine;

[CreateAssetMenu()]
[System.Serializable]
//Global item base class, all items have these fields
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite inventoryImage;
    public int inventorySpace;
    public int monetaryValue;

    public ItemType itemType;

    public enum ItemType
    {
        Item,
        Weapon,
    }
}

[System.Serializable]
//Used to store items in InventoryData.cs
public class ItemInfo
{
    public Item item;
    public ItemData itemData;
    public int index;

    public ItemInfo(Item item, ItemData itemData, int index)
    {
        this.item = item;
        this.itemData = itemData;
        this.index = index;
    }
}

//Individual item data base class, all individual instances of items have this additional data attached to them
public class ItemData { }