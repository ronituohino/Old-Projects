using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HumanRig : MonoBehaviour
{
    public float movementSpeed;
    public float rotationSpeed;

    public HumanController controller;

    [Space]

    [SerializeField] Rigidbody2D weaponRb;
    [SerializeField] HingeJoint2D weaponHingeJoint;
    [SerializeField] CapsuleCollider2D weaponCollider;
    [SerializeField] SpriteRenderer weaponSprite;
    [SerializeField] Transform weaponPivot;

    [Space]

    [SerializeField] float weaponRotationSpeed;

    [Space]

    [SerializeField] Transform rightHandTarget;
    [SerializeField] Transform leftHandTarget;

    [Space]

    [SerializeField] Vector2 defaultRightHandPosition;
    [SerializeField] Vector2 defaultLeftHandPosition;

    [Space]

    [SerializeField] Transform head;
    [SerializeField] float headRotation;

    private void Start()
    {
        controller.UpdateEquipment = UpdateEquippedItem;
        UpdateEquippedItem();
    }

    public void UpdateEquippedItem()
    {
        if(controller.weaponEquipped)
        {
            weaponSprite.sprite = controller.weapon.inGameSprite;
            weaponPivot.localPosition = controller.weapon.weaponPivotPosition;

            JointAngleLimits2D limits = new JointAngleLimits2D();
            limits.min = controller.weapon.hingeJointAngleLimits.x;
            limits.max = controller.weapon.hingeJointAngleLimits.y;

            weaponHingeJoint.limits = limits;

            weaponCollider.enabled = true;
            weaponHingeJoint.enabled = true;
            weaponRb.simulated = true;

            weaponCollider.offset = controller.weapon.capsuleColliderOffset;
            weaponCollider.size = controller.weapon.capsuleColliderSize;

            rightHandTarget.localPosition = controller.weapon.rightHandPos;
            leftHandTarget.localPosition = controller.weapon.leftHandPos;
        }
        else
        {
            weaponSprite.sprite = null;

            rightHandTarget.localPosition = defaultRightHandPosition;
            leftHandTarget.localPosition = defaultLeftHandPosition;

            weaponCollider.enabled = false;
            weaponHingeJoint.enabled = false;

            weaponRb.simulated = false;
        }
    }

    private void Update()
    {
        //Non-physics animations
        //Head rotation
        Quaternion headRot = Extensions.LookAt(head.position, controller.target.position) * Quaternion.Euler(0,0,90);
        head.rotation = Quaternion.Lerp(head.rotation, headRot, headRotation * Time.deltaTime);
    }

    //Physics animations
    private void FixedUpdate()
    {
        controller.rb.AddForce(new Vector2(controller.movement.x, controller.movement.y) 
            * movementSpeed 
            * controller.focus.Remap(0, 1, 1, controller.focusMovementMultiplier), ForceMode2D.Force);

        controller.rb.AddTorque(-(Vector2.SignedAngle((controller.rb.position - controller.target.position.ToVector2()), -controller.rb.transform.up))
            .Remap(-180, 180, -1, 1) 
            * rotationSpeed);

        //Weapon rotation
        if (controller.weaponEquipped)
        {
            weaponRb.AddTorque(-(Vector2.SignedAngle((weaponRb.transform.position.ToVector2() - controller.target.position.ToVector2()),
                (controller.weaponHolstered ? Quaternion.Euler(0, 0, -60) : Quaternion.identity) * -weaponRb.transform.up))
                .Remap(-180, 180, -1, 1) 
                * weaponRotationSpeed);
        }
    }
}