using System.Collections;
using UnityEngine;

public class Loot : Interactable
{
    public struct LootData
    {
        public enum LootType
        {
            Item,
            Coin,
            Supply,
        }

        public LootType lootType;

        // Used with item
        public Item item;
    }

    public LootData data;

    public override void Enter(int fingerId)
    {
        throw new System.NotImplementedException();
    }

    public override void Stay(Vector2 delta)
    {
        throw new System.NotImplementedException();
    }

    public override void Complete()
    {
        throw new System.NotImplementedException();
    }

    public override void Interrupt()
    {
        throw new System.NotImplementedException();
    }
}