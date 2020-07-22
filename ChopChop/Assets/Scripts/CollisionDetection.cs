using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : Singleton<CollisionDetection>
{
    public bool collidingWithFruit = false;

    bool colliding = false;

    private void OnTriggerStay()
    {
        collidingWithFruit = true;
        colliding = true;
    }

    private void LateUpdate()
    {
        if (!colliding)
        {
            collidingWithFruit = false;
        }
        colliding = false;
    }
}
