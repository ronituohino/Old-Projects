using UnityEngine;

public class Shooting : Singleton<Shooting>
{
    public Transform bullets;

    //Probability of ricochet goes linearily with the angle
    public Vector2 ricochetAngle;

    public float bulletLifeTime;

    //Everything calls this function to shoot weapons
    public void Shoot(Weapon weapon, WeaponData weaponData, Transform weaponTransform, float accuracy, float angle)
    {
        weaponData.loadedAmount--;

        GameObject bullet = Instantiate(weaponData.ammunition.bullet);
        bullet.transform.position = weaponTransform.position + weaponTransform.rotation * weapon.bulletOffset;
        bullet.transform.parent = bullets;
        bullet.layer = 11;

        Rigidbody2D rb = bullet.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.drag = 0f;
        rb.angularDrag = 0f;
        rb.mass = 0.01f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        BulletHitDetector bhd = bullet.AddComponent<BulletHitDetector>();
        bhd.bulletThreat = weaponData.ammunition.threat;
        bhd.rb = rb;
        bhd.cl = bullet.GetComponent<BoxCollider2D>();
        bhd.bulletLifeTime = bulletLifeTime;

        Vector2 trajectory = Quaternion.AngleAxis(Random.Range(-angle, angle) * (1 - accuracy), -Vector3.forward) * weaponTransform.up;

        rb.AddForce(trajectory * weaponData.ammunition.speed, ForceMode2D.Impulse);
    }
}
