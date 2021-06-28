using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootSpawner : Singleton<LootSpawner>
{
    [SerializeField] ObjectPooler lootPooler;
    [SerializeField] GameObject lootObject;

    [SerializeField] Vector2 spawnIntervalRange;

    Queue<Loot.LootData> lootQueue = new Queue<Loot.LootData>();
    Coroutine spawnCoroutine = null;

    public void SpawnLoot(Item[] items, int coinAmount, int supplyAmount)
    {
        foreach (Item i in items)
        {
            Loot.LootData ld = new Loot.LootData();
            ld.lootType = Loot.LootData.LootType.Item;
            ld.item = i;

            lootQueue.Enqueue(ld);
        }

        for (int i = 0; i < coinAmount; i++)
        {
            Loot.LootData ld = new Loot.LootData();
            ld.lootType = Loot.LootData.LootType.Coin;

            lootQueue.Enqueue(ld);
        }

        for (int i = 0; i < supplyAmount; i++)
        {
            Loot.LootData ld = new Loot.LootData();
            ld.lootType = Loot.LootData.LootType.Supply;

            lootQueue.Enqueue(ld);
        }

        if (spawnCoroutine == null)
        {
            spawnCoroutine = StartCoroutine(Spawn());
        }
    }



    IEnumerator Spawn()
    {
        while (lootQueue.Count > 0)
        {
            float time = Random.Range(spawnIntervalRange.x, spawnIntervalRange.y);
            yield return new WaitForSeconds(time);

            Loot.LootData ld = lootQueue.Peek();
            GameObject g = lootPooler.Retrieve(0);

            Loot l = g.GetComponent<Loot>();
            l.data = ld;

            // Spawn loot...
        }

        spawnCoroutine = null;
    }
}
