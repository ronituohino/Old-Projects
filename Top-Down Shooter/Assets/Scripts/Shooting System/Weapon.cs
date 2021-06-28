using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Data that ties to all specific weapons (all M4's for example)
[CreateAssetMenu()]
public class Weapon : Item
{
    //Visuals
    public Sprite inGameSprite;

    [Space]

    public Vector2 weaponPivotPosition;
    public Vector2 weaponPosition; //Where to position this on the player so that it looks good
    public Vector2 bulletOffset; //Basically the position where the bullets come out from

    [Space]

    public Vector2 hingeJointAngleLimits;
    public Vector2 capsuleColliderOffset;
    public Vector2 capsuleColliderSize;

    [Space]

    public Vector2 rightHandPos;
    public Vector2 leftHandPos;

    [Space]

    //Gameplay
    public float rateOfFire; //Time interval between shots
    public int ammoCapacity; //Moddable weapons ? clips?
    public float reloadTime; //In seconds
    [Range(0, 1)]
    public float accuracy;
    [Range(0, 1)]
    public float accuracyPenaltyAfterShot;

    public Ammunition[] usableAmmo;
}

[System.Serializable]
public class WeaponData : ItemData
{
    public Ammunition ammunition;
    public int loadedAmount;
}
