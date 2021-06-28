using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ignore collisions between colliders
public class IgnoreCollisions : MonoBehaviour
{
    [System.Serializable]
    public class IgnorePair
    {
        public Collider a;
        public Collider b;
    }

    [SerializeField] IgnorePair[] pairsToIgnoreCollisions;

    void Start()
    {
        foreach(IgnorePair ip in pairsToIgnoreCollisions)
        {
            Physics.IgnoreCollision(ip.a, ip.b);
        }
    }
}
