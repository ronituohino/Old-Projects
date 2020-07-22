using System;
using UnityEngine;

//Human-specific component that talks with a rig that's attached to this character
public class HumanController : MonoBehaviour
{
    public Rigidbody2D rb;

    [HideInInspector]
    public Vector2 movement;
    public Transform target;

    [Space]

    public bool weaponEquipped;
    public Weapon weapon;
    public WeaponData weaponData;

    [Space]

    public bool weaponHolstered = false;

    [HideInInspector]
    public float focus = 0f;

    [Range(0, 1)]
    public float focusMovementMultiplier;

    //Reference to UpdateEquippedItem in HumanRig.cs, called when a new item is equipped
    public Action UpdateEquipment;
}