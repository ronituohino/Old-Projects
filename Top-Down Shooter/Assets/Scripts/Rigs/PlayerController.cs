using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : HumanController
{
    public CameraScript cameraScript;

    public Transform weaponTransform;

    [Space]

    //These are set in EquipmentWindow.cs
    public ItemInfo[] equippedItems;

    [Space]

    public Transform targetBack;
    Vector3 targetBackOriginalScale;
    public Transform targetMask;
    Vector3 targetMaskOriginalScale;
    float targetScaleDifference;
    public float targetScaleConstant = 1f;

    [Space]

    float shootingTimer = 0f;
    float accuracyPenaltyAfterShot = 0f;
    public float accuracyRecoverSpeed;


    [Space]

    //The angle from the gun in which the bullet can fly
    public Vector2 accuracyAngle;

    [Range(0, 1)]
    public float playerAccuracy;
    [Range(0, 1)]
    public float movementAccuracyPenalty;

    public float focusGainSpeed;
    public float focusLoseSpeed;

    [Range(0, 1)]
    public float focusAccuracyBonus; //Right mouse button

    private void Start()
    {
        Cursor.visible = false;
        targetBackOriginalScale = targetBack.localScale;
        targetMaskOriginalScale = targetMask.localScale;
        targetScaleDifference = targetBackOriginalScale.x - targetMaskOriginalScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        //Movement input

        //Interrupt player input if control panel is open
        movement = Vector2.zero;
        if (ControlPanel.Instance.controlPanelOpen)
        {
            return;
        }

        if (Input.GetKey(KeyCode.W))
        {
            movement.y += 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movement.x -= 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movement.y -= 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movement.x += 1;
        }
        movement = movement.normalized;



        //Target position
        Vector3 pos = cameraScript.cam.ScreenToWorldPoint(ReturnMousePositionInScreen());
        target.position = new Vector3(pos.x, pos.y, 0f);



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
        if (Input.GetMouseButton(1))
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


        //Equipping items
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipItem(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EquipItem(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            EquipItem(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            EquipItem(3);
        }



        //Shooting (left mouse)
        if (weaponEquipped)
        {
            if (shootingTimer > 0f)
            {
                shootingTimer -= Time.deltaTime;
            }

            if (Input.GetMouseButton(0))
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
    }

    

    Vector3 ReturnMousePositionInScreen()
    {
        float x = 0f;
        float y = 0f;

        if (Input.mousePosition.x > Screen.width)
        {
            x = Screen.width;
        }
        else if (Input.mousePosition.x < 0f)
        {
            x = 0f;
        }
        else
        {
            x = Input.mousePosition.x;
        }

        if (Input.mousePosition.y > Screen.height)
        {
            y = Screen.height;
        }
        else if (Input.mousePosition.y < 0f)
        {
            y = 0f;
        }
        else
        {
            y = Input.mousePosition.y;
        }

        return new Vector3(x, y, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(rb.position, cameraScript.maxCameraDistance);

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



    void EquipItem(int item)
    {
        ItemInfo info = equippedItems[item];

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
