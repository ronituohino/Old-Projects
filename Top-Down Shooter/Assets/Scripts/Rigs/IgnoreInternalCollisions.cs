using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IgnoreInternalCollisions : MonoBehaviour
{
    private void Awake()
    {
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();

        int length = colliders.Length;
        for (int a = 0; a < length - 1; a++)
        {
            for (int b = a + 1; b < length; b++)
            {
                Physics2D.IgnoreCollision(colliders[a], colliders[b], true);
            }
        }
    }
}