using UnityEngine;

//Component attached to items that have UI behaviour, these talk with ItemSlots to manipulate InventoryData
[System.Serializable]
public class ItemComponent : MonoBehaviour
{
    public RectTransform rect;
    public InventoryData.ItemInfo info;
}