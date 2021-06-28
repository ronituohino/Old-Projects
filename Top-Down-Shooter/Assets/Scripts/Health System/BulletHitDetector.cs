using System.Collections.Generic;
using UnityEngine;

//Attached to bullets, includes data that is carried over to the health system, and methods for bullet behaviour
public class BulletHitDetector : MonoBehaviour
{
    public Rigidbody2D rb;
    public Collider2D cl;

    public float bulletThreat;
    Vector2 previousVelocity;

    bool destroyBullet = false;

    float timeInstantiated;
    float timer = 0f;
    public float bulletLifeTime;

    private void FixedUpdate()
    {
        previousVelocity = rb.velocity;
        timer += Time.fixedDeltaTime;

        if(timer >= bulletLifeTime)
        {
            destroyBullet = true;
        }

        if(destroyBullet)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!destroyBullet)
        {
            GameObject g = collision.collider.gameObject;
            HitArea ha = g.GetComponent<HitArea>();
            bool hitCharacter = ha != null;

            if (hitCharacter)
            {
                float isHit = Random.Range(0, 1f);
                //It's a hit
                if (isHit < ha.chancesOfHitting)
                {
                    ha.CallHit(bulletThreat);

                    destroyBullet = true;
                    Destroy(gameObject);
                }
                else
                //Not a hit, some sound effect?
                {
                    Physics2D.IgnoreCollision(cl, collision.collider);
                    rb.velocity = previousVelocity;
                }
            }
            else //Hit a wall maybe
            {
                Vector2 normal = collision.contacts[0].normal;
                float angle = Mathf.Abs(Vector2.Angle(normal, -previousVelocity) - 90); //90 degrees is directly at the wall, 1 is almost alongside wall
                if (angle < Shooting.Instance.ricochetAngle.y)
                {
                    float probability = Mathf.Abs((angle / Shooting.Instance.ricochetAngle.y) - 1);
                    if (probability < Random.Range(0f, 1f))
                    {
                        //Ricochet
                        Vector2 maxRicochet = Vector2.Reflect(previousVelocity, -normal).normalized;
                        float maxAngle = Vector2.Angle(normal, maxRicochet);

                        Vector2 dir1 = Quaternion.Euler(0, 0, 90) * normal;
                        float dot1 = Vector2.Dot(dir1, previousVelocity);
                        bool positiveRotation = dot1 > 0;

                        Vector2 dir2 = Quaternion.Euler(0, 0, -90) * normal;
                        Vector2 minRicochet = (positiveRotation ? dir1 : dir2).normalized;

                        Vector2 direction = new Vector2(Random.Range(minRicochet.x, maxRicochet.x), Random.Range(minRicochet.y, maxRicochet.y)).normalized;

                        rb.velocity = direction * previousVelocity.magnitude;
                    }
                    else
                    {
                        destroyBullet = true;
                        Destroy(gameObject);
                    }
                }
                else
                {
                    destroyBullet = true;
                    Destroy(gameObject);
                }
            }
        }
    }
}