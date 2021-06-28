using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : HumanController
{
    public Transform weaponTransform;

    [Space]

    //These are set in EquipmentWindow.cs
    public InventoryData.ItemInfo[] equippedItems;

    [Space]

    [SerializeField] Transform targetBack;
    Vector3 targetBackOriginalScale;
    [SerializeField] Transform targetMask;
    Vector3 targetMaskOriginalScale;
    float targetScaleDifference;
    [SerializeField] float targetScaleConstant = 1f;

    [Space]

    float shootingTimer = 0f;
    float accuracyPenaltyAfterShot = 0f;
    public float accuracyRecoverSpeed;


    [Space]

    //The angle from the gun in which the bullet can fly
    [SerializeField] Vector2 accuracyAngle;

    [Range(0, 1)]
    [SerializeField] float playerAccuracy;
    [Range(0, 1)]
    [SerializeField] float movementAccuracyPenalty;

    public bool focusing { get; set; }

    [SerializeField] float focusGainSpeed;
    [SerializeField] float focusLoseSpeed;

    [Range(0, 1)]
    [SerializeField] float focusAccuracyBonus; //Right mouse button

    private void Start()
    {
        Cursor.visible = false;
        targetBackOriginalScale = targetBack.localScale;
        targetMaskOriginalScale = targetMask.localScale;
        targetScaleDifference = targetBackOriginalScale.x - targetMaskOriginalScale.x;
    }

    public void MovePlayer(Vector2 movement)
    {
        this.movement = movement;
    }

    public void FireWeapon()
    {
        if (weaponEquipped)
        {
            if (shootingTimer <= 0f && weaponData.loadedAmount > 0)
            {
                shootingTimer = weapon.rateOfFire;
                Shooting.Instance.Shoot(weapon, weaponData, weaponTransform, GetPlayerAccuracy(), accuracyAngle.y);

                if (accuracyPenaltyAfterShot > 4 * -weapon.accuracyPenaltyAfterShot)
                {
                    accuracyPenaltyAfterShot -= weapon.accuracyPenaltyAfterShot;
                }
                else
                {
                    accuracyPenaltyAfterShot = 4 * -weapon.accuracyPenaltyAfterShot;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Target position
        target.position = InputManager.Instance.mouseInWorld;

        //Target size, accuracy
        if (weaponEquipped)
        {
            float distance = (target.position - (weaponTransform.position + weaponTransform.rotation * weapon.bulletOffset)).magnitude;
            float scaleMultiplier = distance * Mathf.Sin(accuracyAngle.y * (1 - GetPlayerAccuracy()) * (Mathf.PI / 180f)) * targetScaleConstant;

            targetBack.localScale = targetBackOriginalScale * scaleMultiplier;
            targetMask.localScale = targetMaskOriginalScale * scaleMultiplier + ((scaleMultiplier - 1) * new Vector3(targetScaleDifference, targetScaleDifference, 1));
        }
        else
        {
            //??
        }

        //Focus
        if (focusing)
        {
            if (focus < 1f)
            {
                focus += Time.deltaTime * focusGainSpeed;
            }
            else
            {
                focus = 1f;
            }

        }
        else if (focus > 0)
        {
            focus -= Time.deltaTime * focusLoseSpeed;
        }
        else
        {
            focus = 0f;
        }

        //Accuracy penalty backup
        if (accuracyPenaltyAfterShot < 0)
        {
            accuracyPenaltyAfterShot += Time.deltaTime * accuracyRecoverSpeed;
        }
        else
        {
            accuracyPenaltyAfterShot = 0f;
        }

        if (shootingTimer > 0f)
        {
            shootingTimer -= Time.deltaTime;
        }
    }

    /*
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(rb.position, CameraScript.Instance.maxCameraDistance);

        if (weapon != null)
        {
            Vector3 barrelPos = weaponTransform.position + weaponTransform.rotation * weapon.bulletOffset;

            //Inner gizmos take accuracy into account
            Gizmos.DrawRay(new Ray(barrelPos, Quaternion.Euler(0, 0, -accuracyAngle.y * (1 - GetPlayerAccuracy())) * weaponTransform.up));
            Gizmos.DrawRay(new Ray(barrelPos, Quaternion.Euler(0, 0, accuracyAngle.y * (1 - GetPlayerAccuracy())) * weaponTransform.up));

            Gizmos.DrawRay(new Ray(barrelPos, Quaternion.Euler(0, 0, -accuracyAngle.y) * weaponTransform.up));
            Gizmos.DrawRay(new Ray(barrelPos, Quaternion.Euler(0, 0, accuracyAngle.y) * weaponTransform.up));
        }
    }
    */

    float GetPlayerAccuracy()
    {
        float accuracy = playerAccuracy;
        if (movement != Vector2.zero)
        {
            accuracy -= movementAccuracyPenalty;
        }

        accuracy += focusAccuracyBonus * focus;
        accuracy += accuracyPenaltyAfterShot;
        accuracy *= weapon.accuracy;

        return Mathf.Clamp01(accuracy);
    }

    public void EquipItem(int item)
    {
        InventoryData.ItemInfo info = equippedItems[item];

        weapon = (Weapon)info.item;
        weaponData = (WeaponData)info.itemData;

        if (weapon != null)
        {
            weaponEquipped = true;
        }
        else
        {
            weapon = null;
            weaponData = null;

            weaponEquipped = false;
        }

        UpdateEquipment();
    }
}
