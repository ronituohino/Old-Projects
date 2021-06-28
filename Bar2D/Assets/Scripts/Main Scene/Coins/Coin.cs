using System.Collections;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Vector2 gravityScale;

    [SerializeField] Vector2 startSpeed;
    [SerializeField] Vector2 hitSpeed;
    Vector2 dir;

    [Space]

    [SerializeField] float timeUntilPickable;

    bool impact = false;
    float timer = 0f;

    void Start()
    {
        gameObject.layer = 0;

        dir = Random.insideUnitCircle.normalized;

        rb.AddForce(dir * Random.Range(startSpeed.x, startSpeed.y), ForceMode2D.Impulse);
        rb.gravityScale = Random.Range(gravityScale.x, gravityScale.y);
    }

    private void FixedUpdate()
    {
        if(!impact)
        {
            timer += Time.fixedDeltaTime;

            if (timer >= timeUntilPickable)
            {
                impact = true;
                gameObject.layer = 13;

                rb.velocity = Vector2.zero;
                rb.gravityScale = 0f;

                rb.AddForce(dir * Random.Range(hitSpeed.x, hitSpeed.y), ForceMode2D.Impulse);
            }
        }
    }
}