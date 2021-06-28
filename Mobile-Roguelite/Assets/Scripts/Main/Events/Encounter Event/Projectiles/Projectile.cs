using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    public bool enemy;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleHit(collision.gameObject, collision.gameObject.layer);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleHit(collision.gameObject, collision.gameObject.layer);
    }

    void HandleHit(GameObject g, int layer)
    {
        if (enemy && layer == 7)
        {
            Hit(g.GetComponent<AI>());
        }
        else if (!enemy && layer == 8)
        {
            Hit(g.GetComponent<AI>());
        }
        else if (layer == 10)
        {
            Hit(null);
        }
    }

    internal virtual void Hit(AI target)
    {
        print("Hit!");
    }
}