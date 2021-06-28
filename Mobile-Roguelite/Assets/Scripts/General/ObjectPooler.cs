using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public GameObject gameObject;
        public Transform parent;
        public int amountToPool;

        [HideInInspector]
        public List<GameObject> pool = new List<GameObject>();
        [HideInInspector]
        public int amount = 0;
    }

    [SerializeField] List<Pool> pools;

    private void Start()
    {
        foreach(Pool p in pools)
        {
            for (int i = 0; i < p.amountToPool; i++)
            {
                GameObject g = Instantiate(p.gameObject, p.parent);
                p.pool.Add(g);
                g.SetActive(false);
            }

            p.amount = p.amountToPool;
        }
    }

    public GameObject Retrieve(int pool)
    {
        Pool p = pools[pool];

        for (int i = 0; i < p.amount; i++)
        {
            GameObject g = p.pool[i];
            if (!g.activeInHierarchy)
            {
                g.SetActive(true);
                return g;
            }
        }

        // If the pool doesn't have enough, make more!
        GameObject g1 = Instantiate(p.gameObject, p.parent);
        p.pool.Add(g1);
        p.amount++;
        return g1;
    }

    public void Return(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }
}