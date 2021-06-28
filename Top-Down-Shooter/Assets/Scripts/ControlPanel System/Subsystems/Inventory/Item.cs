using UnityEngine;

[CreateAssetMenu()]
[System.Serializable]
//Global item base class, all items have these fields
public class Item : ScriptableObject
{
    public string itemName;

    [Space]

    public Sprite inventoryImage;
    public int inventorySpace;
    public int monetaryValue;

    public ItemType itemType;

    public enum ItemType
    {
        Item,
        Weapon,
    }

    public int maxStack = 1;
}